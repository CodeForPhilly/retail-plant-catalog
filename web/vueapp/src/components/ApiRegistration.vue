<template>
    <div class="post">
       
        <h1>Register as an API User</h1>
        <div class="form-holder">
            <label> <input type="text" placeholder="Name" v-model="user.name"/></label><br /> 
           <label> <input type="text" placeholder="Organization Name" v-model="user.organizationName"/></label>  <br />  
           <label><VuePhoneNumberInput v-model="user.phone" placeholder="Phone" :only-countries="countries"/></label>
           <label>
            <GooglePlacesInput :address="user.address" v-on:placeChange="user.lat=$event.lat;user.lng=$event.lng;user.address=$event.address"/>
            <span class="location">({{ user.lat }}, {{ user.lng }})</span>
           </label>
           <textarea v-model="user.intendedUse" placeholder="Summary of Intended Use">
        
            </textarea>
            <div class="error-holder" v-if="error">
                {{ error }}
                <span class="material-symbols-outlined cancel" @click="closeError">
                cancel
                </span>
            </div>
            
           <div class="error-box" v-if="errors.length">
                <ul class="errors">
                    <li v-for="error in errors" v-bind:key="error"> {{ error }}</li>
                </ul>
           </div>
           <label class="tos">
            <input type="checkbox" v-model="agreeToTerms" />I agree to the <a href="#">Terms of Service</a>
           </label>
           <input type="button" class="primary-btn" @click="submit()" value="Register" />
        </div>
    </div>
</template>

<script lang="js">
    import Vue from 'vue';
    import VuePhoneNumberInput from 'vue-phone-number-input';
    import GooglePlacesInput from './GooglePlacesInput.vue';
    import 'vue-phone-number-input/dist/vue-phone-number-input.css';
    import utils from '../utils'

    export default Vue.extend({
        components:{
            VuePhoneNumberInput,
            GooglePlacesInput
        },
        data() {
            return {
               user:{
                 name:"",
                 organizationName:"",
                 phone: "",
                 address:"",
                 intendedUse:"",
               },
               countries:["US"],
               agreeToTerms:false,
               error:"",
               errors:[]
            };
        },
        created() {
            
        },
        methods: {
            async submit(){
                if (!this.validate()) return;
                var res = await utils.postData("/apiInfo/create", this.user)
                if (res.success){
                    localStorage.setItem("flash", "API application successful, Click recycle below, to get your key. Enjoy!")
                    window.location = "#/api-key"
                }
            }, 
            closeError(){
                this.error = ""
            },
            validate(){
                this.errors = [];
                //determine if fields are invalid
                if (this.user.name?.trim().length === 0){
                    this.errors.push("Name is a required field")
                }
                if (this.user.organizationName?.trim().length === 0){
                    this.errors.push("Organization Name is a required field")
                }
                if (this.user.phone?.trim().length === 0){
                    this.errors.push("Phone is a required field")
                }else if (!/^\(\d{3}\) \d{3}-\d{4}$/.test(this.user.phone)){
                    this.errors.push("Phone must be a valid US number")
                }
                if (this.user.address?.trim().length === 0){
                    this.errors.push("Address is a required field")
                }
                console.log("AgreeToTerms", this.agreeToTerms)
                if (!this.agreeToTerms){
                    this.errors.push("You must agree to the terms of service")
                }
                return (this.errors.length === 0) 
            }
        },
    });
</script>
<style  scoped>
.form-holder{
    background: #EBECF0 0% 0% no-repeat padding-box;
    border-radius: 10px;
    opacity: 1;
    text-align:left;
    padding:30px;
    width:700px;
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
#MazPhoneNumberInput input[type=text]{
    font-size:1.2em;
}
.error-holder{
    margin:10px;
}

.location{
    padding-left:30px;
    font-size:smaller;
}
.urls{
    padding-left: 30px;
    padding-bottom: 5px;
}
textarea{
    width:90%;
}
</style>