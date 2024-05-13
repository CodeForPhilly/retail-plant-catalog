<template>
    <div class="post">
       
        <h1 v-if="vendor.storeName == ''">Register as a Vendor</h1>
        <h1 v-if="vendor.storeName !== ''">Edit Vendor Information</h1>
        <div class="form-holder">
            <p><em class="info">May take up to 72 hours before your listing is approved or denied</em>
            </p>
            <label><input type="checkbox" v-model="vendor.allNative" />All Native Nursery?</label><br />
            <label> <input type="text" placeholder="Store Name" v-model="vendor.storeName"/></label><br /> 
           <label>
            <GooglePlacesInput :address="vendor.address" v-on:placeChange="vendor.lat=$event.lat;vendor.lng=$event.lng;vendor.address=$event.address;vendor.state=$event.state"/>
            <span class="location">({{ vendor.lat }}, {{ vendor.lng }})</span>
         
           </label>  <br />  
           <label> <input type="text" placeholder="Store Url" v-model="vendor.storeUrl"/></label>   <br /> 
           <label> <input type="text" placeholder="Public Email" v-model="vendor.publicEmail"/></label>   <br /> 
           <label><VuePhoneNumberInput v-model="vendor.publicPhone" placeholder="Public Phone" :only-countries="countries"/></label>
           <label> <input type="text" placeholder="Plant Listing Url" v-model="plantListingUrl"/></label>    
           <a @click="addUrl">
              <span class="material-symbols-outlined">
              add_box
             </span>
            </a>
            <br />
            <div class="error-holder" v-if="error">
                {{ error }}
                <span class="material-symbols-outlined cancel" @click="closeError">
                cancel
                </span>
            </div>
           <div class="urls" v-for="(v,k) in vendor.plantListingUrls" v-bind:key="k">
                {{ v }} <span class="material-symbols-outlined" @click="removeUrl(v)">
                                disabled_by_default
                                </span>
            </div>
          
           <div class="error-box" v-if="errors.length">
                <ul class="errors">
                    <li v-for="error in errors" v-bind:key="error"> {{ error }}</li>
                </ul>
           </div>
           <label class="tos" v-if="!vendor.id">
            <input type="checkbox"  v-model="agreeToTerms" v-if="role != 'Admin'" />I agree to the <a href="#">Terms of Service</a>
           </label>
           <div id="plants">
            Native Plants Detected [{{ plants.length }}]
            <ul>

                <li v-for="plant in plants" v-bind:key="plant.id">
                {{ plant.commonName }}
                </li>
                </ul>
           </div>
           <textarea v-model="vendor.notes" placeholder="Notes">
           </textarea><br />
           <input type="button" class="primary-btn" @click="submit()" v-if="!(vendor.id || role == 'Admin')" value="Submit for Approval" />
           <input type="button" class="primary-btn" @click="crawl()" v-if="role == 'Admin' && vendor.id" :disabled="crawlInProgress" value="Crawl Site(s)" />
           <img src="/loading.gif" alt="crawl in progress" v-if="crawlInProgress" width="30" height="30" style="position:relative;top:5px;left:5px"/>
           <input type="button" class="primary-btn save" :disabled="crawlInProgress" @click="submit()" v-if="vendor.id || role == 'Admin'" value="Save" />
           <br /><em v-if="crawlInProgress">Crawl in progress.  May take up to 5 minutes.  Save disabled while crawl in progress</em>
        </div>

       
    </div>
</template>

<script lang="js">
    import Vue from 'vue';
    import VuePhoneNumberInput from 'vue-phone-number-input';
    import 'vue-phone-number-input/dist/vue-phone-number-input.css';
    import GooglePlacesInput from './GooglePlacesInput.vue';
    import utils from '../utils'
   
    export default Vue.extend({
        components:{
            VuePhoneNumberInput,
            GooglePlacesInput
        },
        data() {
            return {
               vendor:{
                approved:false,
                storeName: "",
                address: "",
                storeUrl: "",
                publicEmail: "", 
                publicPhone: "",
                state:"",
                plantListingUrls: [
                ]
               },
               crawlInProgress: false,
               lat:0,
               role:"",
               plants:[],
               plantListingUrl: "",
               countries: ["US"],
               agreeToTerms: false,
               error:"",
               errors:[]
            };
        },
        async created() {
            //get vendor for current user, if exists
            var hash = window.location.hash.split('?')
            var id = hash.length > 1 ?  hash[1].split("=")[1] : null;
            this.role = localStorage.getItem('role');
            if (this.role == 'Admin' && id != null)
            {
                await utils.getData(`/vendor/get?id=${id}`)
                .then(json => {
                    console.log(json)
                    this.vendor = json;
                });
            }else{
                await utils.getData("/vendor/current")
                .then(json => {
                    console.log(json)
                    this.vendor = json;
                });
            }
            if (id){
                await utils.getData("/plant/FindByVendorInternal?vendorId=" + id)
                .then(json => {
                    console.log(json)
                    this.plants = json
                })
            }
        },
        methods: {
            async submit(){
                var didValidate = await this.validate();
                if (!didValidate) return;
                console.log(this.vendor)
                if (this.vendor.id == undefined){
                    var result = await utils.postData("/vendor/create", this.vendor)
                    if (result.success)
                        window.location = result.redirectUrl
                }else{
                    var updateResult = await utils.postData("/vendor/update", this.vendor  )
                    console.log(updateResult)
                    if (updateResult.success)
                        window.location = updateResult.redirectUrl
                }
             
            }, 
            async crawl(){
                this.crawlInProgress = true;
                await utils.postData("/vendor/crawl?id=" + this.vendor.id)
                this.crawlInProgress = false;
                window.location = "/#/vendors"
            },
            closeError(){
                this.error = ""
            },
            async validate(){
                console.log("validate", this.vendor)
                this.errors = [];
                //determine if fields are invalid
                 if (this.vendor.storeName?.trim().length === 0){
                    this.errors.push("Store Name is a required field")
                }else if (!/^[a-zA-Z 0-9']+$/.test(this.vendor.storeName)){
                    this.errors.push("Store Name may only contain alpha numeric characters")
                }
                if (this.vendor.address?.trim().length === 0){
                    this.errors.push("Address is a required field")
                }else if (this.vendor.address.length > 500){
                    this.errors.push("Address must be less than 500 characters")
                }
                if (this.vendor.storeUrl?.trim().length === 0){
                    this.errors.push("Store Url is a required field")
                }else if (this.vendor.storeUrl.length > 500){
                    this.errors.push("Store URL may not be longer than 500 characters")
                }else if (!/https?:\/\/(www\.)?[-a-zA-Z0-9@:%._+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_+.~#?&//=]*)/.test(this.vendor.storeUrl)){
                    this.errors.push("Store URL is not a valid URL")
                }
                if (this.vendor.publicEmail?.trim().length === 0)
                {
                    this.errors.push("Public Email is a required field")
                // eslint-disable-next-line no-control-regex
                }else if (!/(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])/.test(this.vendor.publicEmail)){
                    this.errors.push("Public Email must be a valid email")
                }
                if (this.vendor.publicPhone?.trim().length === 0){
                    this.errors.push("Public Phone is a required field")
                }else if (!/^\(\d{3}\) \d{3}-\d{4}$/.test(this.vendor.publicPhone)){
                    this.errors.push("Public Phone must be a valid US number")
                }
                if (this.vendor.plantListingUrls == null || this.vendor.plantListingUrls.length === 0){
                    this.errors.push("There must be at least one Plant Listing URL.  Be sure to hit the add button")
                }else{
                    console.log("listing urls breakpoint")
                    for (var uri of this.vendor.plantListingUrls){
                        console.log("Evaluating url: " + uri)
                        var result = await utils.getData("/vendor/IsAllowed?Url=" + encodeURIComponent(uri))
                        if (!result.success){
                            this.errors.push(`Unfortunately due to robots.txt policy we cannot spider this url [${uri}].  Please remove it`)
                        }else{
                            console.log("Allowed:" + uri)
                        }
                    }
                }
                if (!this.agreeToTerms && !this.vendor.id){
                    this.errors.push("You must agree to the terms of service")
                }
                return (this.errors.length === 0) 
            },
            addUrl(){
               this.error = "";
               if (!this.plantListingUrl){
                this.error = "Must enter url before adding!"
                return;
               }
               var testUrl = /((([A-Za-z]{3,9}:(?:\/\/)?)(?:[-;:&=+$,\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=+$,\w]+@)[A-Za-z0-9.-]+)((?:\/[+~%/.\w-_]*)?\??(?:[-+=&;%@.\w_]*)#?(?:[\w]*))?)/.test(this.plantListingUrl);
               if (!testUrl){
                this.error = "Invalid url.  Please try again."
                return;
               }
               var dup = this.vendor.plantListingUrls.filter(u => u == this.plantListingUrl);
               if (dup.length > 0){
                this.error = "Cannot enter a duplicate url."
                return;
               }
               this.vendor.plantListingUrls.push(this.plantListingUrl) 
               this.plantListingUrl = "";
            },
            removeUrl(v){
                if (confirm("Are you sure you want to remove this url? " + v)){
                    this.vendor.plantListingUrls = this.vendor.plantListingUrls.filter(u => u != v)
                }
            }
        },
    });
</script>
<style scoped>
input[type="checkbox"]{
    margin-left:20px;
}
.form-holder{
    background: #EBECF0 0% 0% no-repeat padding-box;
    border-radius: 10px;
    opacity: 1;
    text-align:left;
    padding:30px;
    width:700px;
}
#plants{
    float: right;
    width: 187px;
    padding: 20px;
    border: dashed 2px #00f;
    border-radius: 18px;
    list-style-type: none;
    position: absolute;
    top: 0;
    right: 0px;
    height: 351px;
    overflow-y: scroll;
    margin:5px;
}
#plants ul{
    list-style-type:none;
}
.cancel{
    float:right;
}
.tos{
    display:inline-block;
    padding-right:25px;
    margin-left:12px;
    font-size: 1.2em;
    color: #6A6A6A;
}
.tos a{
    color: #6A6A6A;
    text-decoration: none;
}
.tos a:hover{
    text-decoration:underline;
}
.info{
    margin:13px;
}
#MazPhoneNumberInput{
    width: 455px;
    margin-left: 13px;
    padding-top: 10px;
    margin-bottom: 10px;
}
.location{
    padding-left:30px;
    font-size:smaller;
}
input[type="button"]{
    cursor:pointer;
}
input[type="button"]:disabled{
    background-color:#666;
    cursor: wait;
}
span.material-symbols-outlined {
    cursor: pointer;
    font-size: 24px;
    top: 5px;
    position: relative;
}
#MazPhoneNumberInput input[type=text]{
    font-size:1.2em;
}
.error-holder{
    margin:10px;
}
.urls{
    padding-left: 30px;
    padding-bottom: 5px;
}
.save{
   position:absolute;
   right:15px;
   bottom:15px;
}
.form-holder{
    position:relative;
}
</style>