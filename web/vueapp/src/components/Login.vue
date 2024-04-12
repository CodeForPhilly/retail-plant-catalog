<template>
    <div class="post">
         <h1>Login</h1>
        <div id="api-holder">
            <input type="text" placeholder="Email" v-model="user.email" v-on:keyup.enter="login"/>
            <input type="password" placeholder="Password" v-model="user.password" v-on:keyup.enter="login" /><br />
            <div class="error-holder"  v-if="error">
                {{ error }} 
                
                <span class="material-symbols-outlined cancel" @click="closeError">
                cancel
                </span>
            </div>
            <a class="forgot-password" @click="passwordReset">Forgot Password?</a><br />
            <input type="submit" class="primary-btn" value="Login" @click="login" />
             
        </div>
    </div>
</template>

<script lang="js">
    import Vue from 'vue';
    import utils from '../utils'
    
    import { GlobalEventEmitter } from '@/events'


    export default Vue.extend({
        data() {
            return {
                user:{
                    email:"",
                    password:""
                },
                error:""
            };
        },
        created(){
            this.user.email = localStorage.getItem('email');
        },
        methods:{
            async passwordReset(){
                if (!this.user.email){
                    alert("Enter your email before clicking this link")
                    return;
                }
                await utils.postData("/user/ForgotPassword?email=" + encodeURIComponent(this.user.email))
                alert("If your email is found then you will receive a password reset request")
            },
            async login(){
                var hash = window.location.hash;
                var query = hash.split("?")[1];
                if (query){
                    var redirectUrl = query.split("=")[1]
                }
                this.error = "";
                var result = await utils.postData("/user/login", this.user)
                if (result.success){
                    //get the role and store it in local storage or memory
                    console.log(result)
                    if (result.verified){
                        localStorage.setItem('role', result.role)
                        localStorage.setItem('email', result.email)
                        GlobalEventEmitter.$emit('userLoggedIn')
                        if (redirectUrl){
                            window.location = decodeURIComponent(redirectUrl);
                            return;
                        }
                        if (result.role == 'Admin'){
                            window.location = "/#/vendors"
                        }else{
                            window.location = "/#/";
                        }
                    }else{
                        this.error = "Please check your email for the verification email before your first login";
                    }
                }else{
                    this.error = "Authentication attempt failed";
                }
            },
            closeError(){
                this.error = ""
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
    padding: 30px;
  }
  .primary-btn{
    margin-top:10px;
  }
  .forgot-password{
    cursor:pointer;
    float:none;
  }
  a{
    float: right;
    color: #01573E;
    text-decoration:none;
  }
  a:hover{
    text-decoration:underline;
  }
  .cancel{
    float:right;
  }
</style>