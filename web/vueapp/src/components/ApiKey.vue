<template>
  <div class="post">
    <h1>API Key</h1>
    <div id="api-holder">
      <input type="text" placeholder="API Key" v-model="apiKey" />
      <span class="material-symbols-outlined" v-if="apiKey" @click="clipboard">
        content_copy
      </span>
      <span class="material-symbols-outlined" @click="genKey"> recycling </span
      ><br />
      {{ message }}
      <div class="error-holder" v-if="error">
        <span v-html="error"></span>

        <span class="material-symbols-outlined cancel" @click="closeError">
          cancel
        </span>
      </div>
      <a id="api-link" target="_blank" href="/swagger/index.html">
        View API Documentation</a
      >
    </div>
  </div>
</template>

<script lang="js">
import Vue from 'vue';
import utils from '../utils'

export default Vue.extend({
    data() {
        return {
            apiKey:"",
            message:"",
            error:""
        };
    },
    async created(){
        // Check authentication first
        if (!localStorage.getItem('loggedIn') || !localStorage.getItem('role')){
            window.location = '/#/login';
            return;
        }

        await this.getKey();
    },
    methods:{
        async getKey(){
            await utils.getData("/user/GetApiKey")
            .then(json => {
                console.log(json)
                this.apiKey = json.id
            });
        },
        async genKey(){
            this.message = "";
            if (confirm("Are you sure you want to regenerate an api key?  This will invalidate prior keys.")){
                var resp = await utils.postData("/user/GenApi")
                if (resp.success){
                    this.apiKey = resp.value;
                }else{
                    this.error = resp.message;
                }
            }
        },
        clipboard(){
            this.message = "";
            if (this.apiKey){
                navigator.clipboard.writeText(this.apiKey);
                this.message="copied!"
            }else{
                this.error="Must generate a key first!"
            }
        },
        closeError(){
            this.error = ""
        }
    }

});
</script>
<style scoped>
#api-link {
  margin: auto;
  display: block;
  width: 200px;
  color: #01573e;
}
#api-holder {
  background: #ede6d6 0% 0% no-repeat padding-box;
  border-radius: 10px;
  width: 686px;
  text-align: left;
  padding: 30px;
}

.error-holder a {
  color: #fff !important;
}
.cancel {
  float: right;
}
</style>
