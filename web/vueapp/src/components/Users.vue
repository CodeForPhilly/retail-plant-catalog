<template>
    <div class="post">
        <div v-if="loading" class="loading">
            Loading... 
        </div>
        <h1>Users</h1>
        <div v-if="post" class="content">
            <label class="show-admin"><input type="checkbox" v-model="showAdminOnly" />Show Admin Only?</label>
            <a @click="exportCSV" class="export-csv">Export as CSV</a>
            <table class="grid">
                <thead>
                    <tr>
                        <th>Email</th>
                        <th>Role</th>
                        <th>Verified</th>
                        <th>API Key</th>
                        <th colspan="3">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="user in post" :key="user.id">
                        
                        <td>{{ user.email }}</td>
                        <td>{{ user.role }}</td>
                       
                        <td>{{ user.verified }}</td>
                        <td class="pointer" :title="user.intendedUse"> {{  user.apiKey ? "yes" : "no" }}</td>
                        <td>
                            <span class="material-symbols-outlined" @click="promote(user.id)" title="Promote to Admin">
                            admin_panel_settings
                            </span>
                        </td>
                        <td>
                           <span class="material-symbols-outlined" @click="del(user.id)">
                            delete
                            </span>
                        </td>
                        <td>
                           <a class="resend" @click="resend(user.id)">Resend</a>
                        </td>
                    </tr>
                </tbody>
            </table>
            
            <a @click="prev()" v-if="pagenumber > 0">Prev</a>
            <a @click="next()" v-if="count == paging">Next</a>
        </div>
    </div>
</template>

<script lang="js">
    import Vue from 'vue';
    import utils from '../utils'

    function download(filename, text) {
        var element = document.createElement('a');
        element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
        element.setAttribute('download', filename);

        element.style.display = 'none';
        document.body.appendChild(element);

        element.click();

        document.body.removeChild(element);
    }
    export default Vue.extend({
        data() {
            return {
                loading: false,
                showAdminOnly:false,
                post: null,
                pagenumber:0,
                count:0,
                paging: 20
            };
        },
        created() {
            // fetch the data when the view is created and the data is
            // already being observed
            this.fetchData();
        },
        watch: {
            // call again the method if the route changes
            '$route': 'fetchData',
            'showAdminOnly': 'fetchData'
        },
        methods: {
            async fetchData() {
                this.post = null;
                this.loading = true;
                var skip = this.pagenumber * this.paging

                await utils.getData(`user/search?showAdminOnly=${this.showAdminOnly}&skip=${skip}&take=${this.paging}`, {redirect: "manual"})
                    .then(json => {
                        this.post = json;
                        this.count = this.post.length;
                        this.loading = false;
                        return;
                    })
            },
            async exportCSV(){
                this.loading = true;
                const d = new Date();
                let offset = d.getTimezoneOffset();

                await utils.getData(`user/export?showAdminOnly=${this.showAdminOnly}&offset=${offset}`)
                    .then(json => {
                        this.post = json;
                        var lineArray = ["Id,Email,Role, Verified, Intended Use,CreatedAt, ModifiedAt"];
                        this.post.forEach(function (infoArray) {
                            var line = infoArray.join(",");
                            lineArray.push(line);
                        });
                        var csvContent = lineArray.join("\n");
                        download("users.csv", csvContent);
                        this.count = this.post.length;
                        this.loading = false;
                        this.fetchData()
                        return;
                    });
            },
            async promote(id){
                if (confirm("Are you sure you want to promote this user to Admin?")){
                    await utils.postData(`/user/promote?id=${id}`)
                }
                this.fetchData();
            },
            async del(id){
                if (confirm("Are you sure?")){
                    await utils.postData(`/user/delete?id=${id}`)
                    await this.fetchData();
                }
            },
            async resend(id){
                await utils.postData(`/user/resend?id=${id}`)
                alert("Verification email has been resent")
                return;
            },
            next(){
                this.pagenumber++;
                this.fetchData();
            },
            prev(){
                this.pagenumber--;
                this.fetchData();
            }
        },
    });
</script>
<style scoped>
   .resend, .pointer{
    cursor:pointer;
   }
   .content{
    text-align:left;
    width:1000px;
   }
   .export-csv{
    float:right;
   }
   .export-csv:hover{
    text-decoration: underline;
    cursor:pointer;
   }
   .grid{
    width:100%;
   }
   .show-admin{
    cursor:pointer;
   }
</style>