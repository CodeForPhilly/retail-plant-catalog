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
           <label> <input type="text" placeholder="Store Url" v-on:blur="prependHttp()" v-model="vendor.storeUrl"/></label>   <br /> 
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
           <div class="urls" v-for="(v,k) in vendor.plantListingUris" v-bind:key="k">
                {{ v.uri }} <span class="material-symbols-outlined" @click="removeUrl(v)">
                                disabled_by_default
                                </span> 
                                <span class="material-symbols-outlined" @click="testExistingUrl(v)" title="Test URL">
                                task_alt
                                </span>
                                <span v-if="v.lastStatus == 'None' || v.lastStatus == 'Ok'" class="success-tag" :title="prettyDate('Last successful crawl:', v.lastSucceeded)"><span class="material-symbols-outlined">check_circle</span> Success</span>
                                <span v-else class="fail-tag" :title="prettyDate('Last failed crawl:', v.lastFailed) + '\n'+  prettyDate('Last successed crawl:', v.lastSucceeded)">
                                    <span class="material-symbols-outlined">error</span> {{ v.lastStatus }}</span>
                                   
                               
            </div>
          
           <div class="error-box" v-if="errors.length">
                <ul class="errors">
                    <li v-for="error in errors" v-bind:key="error"> {{ error }}</li>
                </ul>
           </div>
           <label class="tos" v-if="!vendor.id && role != 'Admin'">
            <input type="checkbox"  v-model="agreeToTerms" />I agree to the <a href="#">Terms of Service</a>
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
        async mounted() {
            //get vendor for current user, if exists
            console.log("Mounted!")
            var hash = window.location.hash.split('?')
            var id = hash.length > 1 ?  hash[1].split("=")[1] : null;
            this.role = localStorage.getItem('role');
            console.log("Role", this.role)
          
            if (this.role == 'Admin' )
            {
                if (id != null){
                    await utils.getData(`/vendor/get?id=${id}`)
                    .then(json => {
                        console.log(json)
                        this.vendor = json;
                    });
                }
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
            prettyDate(prefix, date){
                var d = new Date(date)
                if (d.getFullYear() == 1){
                    return prefix + " Never"
                }
                return prefix + " " + d.toLocaleDateString() + " " + d.toLocaleTimeString()
            },
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
            prependHttp(){
                if (this.vendor.storeUrl && this.vendor.storeUrl.indexOf("http") < 0)
                    this.vendor.storeUrl = "https://" + this.vendor.storeUrl;
            },
            async validate(){
                console.log("validate", this.vendor)
                this.errors = [];
                //determine if fields are invalid
                 if (this.vendor.storeName?.trim().length === 0){
                    this.errors.push("Store Name is a required field")
                }else if (!/^[a-zA-Z 0-9'\-/]+$/.test(this.vendor.storeName)){
                    this.errors.push("Store Name may only contain alpha numeric characters and -, /, '")
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
                if (this.vendor.plantListingUris == null || this.vendor.plantListingUris.length === 0){
                    this.errors.push("There must be at least one Plant Listing URL.  Be sure to hit the add button")
                }else{
                    console.log("listing urls breakpoint")
                    for (var uri of this.vendor.plantListingUris){
                        console.log("Evaluating url: " + uri)
                        var result = await utils.getData("/vendor/IsAllowed?Url=" + encodeURIComponent(uri))
                        if (!result.success){
                            this.errors.push(`Unfortunately due to robots.txt policy we cannot spider this url [${uri}].  Please remove it`)
                        }else{
                            console.log("Allowed:" + uri)
                        }
                    }
                }
                if (this.role != 'Admin' && !this.agreeToTerms && !this.vendor.id){
                    this.errors.push("You must agree to the terms of service")
                }
                return (this.errors.length === 0) 
            },
            async addUrl(){
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
               var dup = this.vendor.plantListingUris.filter(u => u.uri == this.plantListingUrl);
               if (dup.length > 0){
                this.error = "Cannot enter a duplicate url."
                return;
               }
               
               // Test the URL before adding it
               if (this.vendor.id) {
                  try {
                    this.error = "Testing URL...";
                    const testResult = await utils.postData("/vendor/TestUrl", {
                      url: this.plantListingUrl,
                      vendorId: this.vendor.id
                    });
                    
                    console.log("Test result:", testResult);
                    
                    // Add URL with test result status
                    const newUrl = {
                      id: testResult.id,
                      uri: this.plantListingUrl,
                      lastStatus: testResult.message
                    };
                    
                    // Add the URL to the list regardless of success/failure
                    if (!this.vendor.plantListingUris) {
                      this.vendor.plantListingUris = [];
                    }
                    this.vendor.plantListingUris.push(newUrl);
                    
                    if (testResult.success) {
                      this.error = "URL test successful! URL added.";
                    } else {
                      this.error = `URL test warning: ${testResult.message}. URL added but may have issues.`;
                    }
                    
                    // Refresh the vendor data to get updated crawlErrors count
                    if (this.role == 'Admin') {
                      await utils.getData(`/vendor/get?id=${this.vendor.id}`)
                      .then(json => {
                        // Preserve the newly added URL while updating
                        const existingUrls = this.vendor.plantListingUris || [];
                        this.vendor = json;
                        
                        // If the new URL is not in the refreshed data, add it back
                        if (this.vendor.plantListingUris && !this.vendor.plantListingUris.some(u => u.id === newUrl.id)) {
                          this.vendor.plantListingUris.push(newUrl);
                        }
                      });
                    } else {
                      await utils.getData("/vendor/current")
                      .then(json => {
                        // Preserve the newly added URL while updating
                        const existingUrls = this.vendor.plantListingUris || [];
                        this.vendor = json;
                        
                        // If the new URL is not in the refreshed data, add it back
                        if (this.vendor.plantListingUris && !this.vendor.plantListingUris.some(u => u.id === newUrl.id)) {
                          this.vendor.plantListingUris.push(newUrl);
                        }
                      });
                    }
                  } catch (error) {
                    console.error("URL test error:", error);
                    this.error = "Error testing URL. Added URL but couldn't verify status.";
                    this.vendor.plantListingUris.push({uri: this.plantListingUrl});
                  }
               } else {
                 // If vendor doesn't have an ID yet (new vendor), just add the URL without testing
                 if (!this.vendor.plantListingUris) {
                   this.vendor.plantListingUris = [];
                 }
                 this.vendor.plantListingUris.push({uri: this.plantListingUrl});
               }
               
               this.plantListingUrl = "";
            },
            removeUrl(v){
                if (confirm("Are you sure you want to remove this url? " + v)){
                    this.vendor.plantListingUris = this.vendor.plantListingUris.filter(u => u.uri != v.uri)
                }
            },
            async testExistingUrl(v){
                if (!this.vendor.id) {
                    this.error = "Cannot test URL until vendor is saved.";
                    return;
                }
                
                try {
                    this.error = "Testing URL...";
                    const testResult = await utils.postData("/vendor/TestUrl", {
                        url: v.uri,
                        vendorId: this.vendor.id,
                        urlId: v.id
                    });
                    
                    // Update the URL status
                    v.lastStatus = testResult.message;
                    if (testResult.success) {
                        this.error = "URL test successful!";
                    } else {
                        this.error = `URL test warning: ${testResult.message}`;
                    }
                    
                    // Refresh the vendor data to get updated crawlErrors count
                    if (this.role == 'Admin') {
                        await utils.getData(`/vendor/get?id=${this.vendor.id}`)
                        .then(json => {
                            this.vendor = json;
                        });
                    } else {
                        await utils.getData("/vendor/current")
                        .then(json => {
                            this.vendor = json;
                        });
                    }
                } catch (error) {
                    this.error = "Error testing URL: " + (error.message || "Unknown error");
                }
            }
        },
    });
</script>
<style scoped>
input[type="checkbox"]{
    margin-left:20px;
}
.fail-tag{
    color: red;
    margin-left: 10px;
}
.success-tag{
    color: green;
    margin-left: 10px;
}
.error-holder{
    margin: 10px;
    padding: 8px;
    background-color: #fff3cd;
    border: 1px solid #ffeeba;
    border-radius: 4px;
    color: #856404;
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