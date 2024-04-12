<template>
    <div class="post">
         <h1>Create User <span v-if="type">({{ type }})</span></h1>
        <div id="api-holder">
            <div class="fields">
                <input type="text" placeholder="Email" v-model="user.email" v-on:keyup="validateIfErrors"/><br />
              <input type="password" placeholder="Password" v-model="user.password" v-on:keyup="validateIfErrors" /><br />
        
            </div>
            
            <div id="error-holder" class="error-holder" v-if="errors && errors.length > 0">
                <span class="material-symbols-outlined cancel" @click="closeError">
                cancel
                </span>
                <div v-for="error in errors" v-bind:key="error">
                    {{ error }}
                </div>
               
            </div>
        </div>
        <input type="button" class="primary-btn submit" @click="createUser" value="Submit"/>
       
    </div>
</template>

<script lang="js">
    import Vue from 'vue';
    import utils from '../utils'

    export default Vue.extend({
        data() {
            return {
                user:{
                    email:'',
                    password:''
                },
                type: "",
                shake:true,
                message:"",
                errors:[]
            };
        },
        created(){
            try{
                var hash = window.location.hash
                if (hash.indexOf("?")){
                    const loc = hash.split('?')[1]
                    const params = loc.split('=')
                    console.log("Params", params)
                    this.type = params[1]
                }else{
                    this.type = "Vendor"
                }
            }catch(err){
                this.type = "Vendor"
            }
        },
        methods:{
            closeError(){
                this.errors = []
            },
            validateIfErrors(){
                if (this.errors.length) this.validate()
            },
            validate(){
                this.errors=[]
                if (!this.user.email)
                    this.errors.push("Email is a required field")
                if (!this.user.password){
                    this.errors.push("Password is a required field")
                    return false;
                }
                 // eslint-disable-next-line no-control-regex
                if (!/(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])/.test(this.user.email))
                    this.errors.push("Email must be valid")
                if (this.user.password.length < 10)
                    this.errors.push("Password must be at least 10 characters")
                if (!/[a-z]/.test(this.user.password))
                    this.errors.push("Password must have a least one lowercase letter")
                if (!/[A-Z]/.test(this.user.password))
                    this.errors.push("Password must have at least one UPPERCASE letter")
                if (!/[0-9]/.test(this.user.password))
                    this.errors.push("Password must have at least one digit")
                if (!/[!@$#%^&*()]/.test(this.user.password))
                    this.errors.push("Password must contain at least one symbol")
                var valid = this.errors.length == 0
                if (!valid) {
                    var el = document.getElementById('error-holder')
                    el.style.animationName = 'none';
                    setTimeout(() => {
                        el.style.animationName = '';
                    }, 0)
               }
                return valid;
            },
            async createUser(){
                if (!this.validate()) return ;
                var redirectUrl;
                if (this.type.toLowerCase() == "api"){
                    redirectUrl = "#/login?redirectUrl=%2F%23%2Fapi-registration"
                }else{
                    redirectUrl = "#/login?redirectUrl=%2F%23%2Fvendor-registration"
                }
                var result = await utils.postData("/user/create", {user:this.user, redirectUrl})
                console.log("Result", result)
                localStorage.setItem('email', this.user.email)
                localStorage.setItem('flash', "Check your email for confirmation link")
                if (result.success){
                   window.location = "/#/thank-you"
                }else{
                    this.errors.push(result.message)
                }
            }
        }

    });
</script>
<style scoped>
  #api-holder{
    background: #EDE6D6 0% 0% no-repeat padding-box;
    border-radius: 10px;
    width: 686px;
    text-align: left;
    padding-bottom:10px;
  }
  .post{
    width:686px;
    text-align:right;
  }
  .cancel{
    float:right;
  }
  .submit{
    margin:10px;
  }
  .fields{
    padding:10px 30px;
  }
  #error-holder{
    margin:10px 40px !important;
    animation: shake 0.82s cubic-bezier(.36,.07,.19,.97) both;
  }

  @keyframes shake {
    10%, 90% {
        transform: translate3d(-1px, 0, 0);
    }
    
    20%, 80% {
        transform: translate3d(2px, 0, 0);
    }

    30%, 50%, 70% {
        transform: translate3d(-4px, 0, 0);
    }

    40%, 60% {
        transform: translate3d(4px, 0, 0);
    }
    }
</style>