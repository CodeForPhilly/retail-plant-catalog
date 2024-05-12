<template>
    <div class="post">
        <div v-if="loading" class="loading">
            Loading... 
        </div>
        <h1>Vendors  <a @click="add()" title="Add Vendor"><span class="material-symbols-outlined">
              add_box
             </span></a></h1>
       
        <div id="controls">
            <input type="text" id="search" v-model="storeName" @keyup="fetchData()" placeholder="Search..." />
          
            <label><input type="checkbox" v-model="unapprovedOnly" @change="fetchData()"/>Show Only Unapproved?</label>
            <label style="padding-left:10px"><input type="checkbox" v-model="showDeleted" @change="fetchData()"/>Show Deleted?</label><br />
            <select v-model="state" @change="fetchData()" id="stateFilter">
                <option value="ALL">Filter by State</option>
                <option value="AL">Alabama</option>
                <option value="AK">Alaska</option>
                <option value="AZ">Arizona</option>
                <option value="AR">Arkansas</option>
                <option value="CA">California</option>
                <option value="CO">Colorado</option>
                <option value="CT">Connecticut</option>
                <option value="DE">Delaware</option>
                <option value="DC">District Of Columbia</option>
                <option value="FL">Florida</option>
                <option value="GA">Georgia</option>
                <option value="HI">Hawaii</option>
                <option value="ID">Idaho</option>
                <option value="IL">Illinois</option>
                <option value="IN">Indiana</option>
                <option value="IA">Iowa</option>
                <option value="KS">Kansas</option>
                <option value="KY">Kentucky</option>
                <option value="LA">Louisiana</option>
                <option value="ME">Maine</option>
                <option value="MD">Maryland</option>
                <option value="MA">Massachusetts</option>
                <option value="MI">Michigan</option>
                <option value="MN">Minnesota</option>
                <option value="MS">Mississippi</option>
                <option value="MO">Missouri</option>
                <option value="MT">Montana</option>
                <option value="NE">Nebraska</option>
                <option value="NV">Nevada</option>
                <option value="NH">New Hampshire</option>
                <option value="NJ">New Jersey</option>
                <option value="NM">New Mexico</option>
                <option value="NY">New York</option>
                <option value="NC">North Carolina</option>
                <option value="ND">North Dakota</option>
                <option value="OH">Ohio</option>
                <option value="OK">Oklahoma</option>
                <option value="OR">Oregon</option>
                <option value="PA">Pennsylvania</option>
                <option value="RI">Rhode Island</option>
                <option value="SC">South Carolina</option>
                <option value="SD">South Dakota</option>
                <option value="TN">Tennessee</option>
                <option value="TX">Texas</option>
                <option value="UT">Utah</option>
                <option value="VT">Vermont</option>
                <option value="VA">Virginia</option>
                <option value="WA">Washington</option>
                <option value="WV">West Virginia</option>
                <option value="WI">Wisconsin</option>
                <option value="WY">Wyoming</option>
            </select>
        </div>
        <br style="clear:both"/>
        <div v-if="post" class="content">
            <table class="grid">
                <thead>
                    <tr>
                        <th><a @click="sort('StoreName')">Name</a> {{ sortBy != "StoreName" ? "" : (sortAsc ? "v" : "^") }}</th>
                        <th><a @click="sort('PlantCount')">Detected Plants</a> {{ sortBy != "PlantCount" ? "" : (sortAsc ? "v" : "^") }}</th>
                        <th><a @click="sort('CreatedAt')">Created At</a> {{ sortBy != "CreatedAt" ? "": (sortAsc ? "v" : "^")  }}</th>
                        <th>Approved?</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="vendor in post" :key="vendor.id">
                        <td>{{ vendor.storeName }}</td>
                        <td>{{ vendor.plantCount }}</td>
                        <td>{{ vendor.createdAt }}</td>
                        <td>{{ vendor.approved }}</td>
                        <td>
                            <span class="material-symbols-outlined" @click="edit(vendor.id)">
                                edit
                            </span>
                            <span class="material-symbols-outlined" @click="del(vendor.id)">
                               delete
                            </span>
                            <span class="material-symbols-outlined" v-if="!vendor.approved" @click="approve(vendor.id)">
                                thumb_up
                            </span>
                            <span class="material-symbols-outlined" @click="reject(vendor.id)">
                                thumb_down
                            </span>
                        </td>
                    </tr>
                </tbody>
            </table>
            
            <a class="skipnext" @click="prev()" v-if="pagenumber > 0">
                <span class="material-symbols-outlined">
                 skip_previous
                </span>
            </a>
            <a class="skipnext" @click="next()" v-if="count == paging">
                <span class="material-symbols-outlined">
                    skip_next
                </span>
            </a>
        </div>
    </div>
</template>
<style>
    #stateFilter{
        float:left !important;
        font-family: sans-serif;
        padding:5px 15px;
        border-radius:5px;

    }
    #search{
        float:right;
    }
    th a:hover{
        text-decoration: underline;
        cursor: pointer;
    }
    #controls{
        width:80%;
    }
    #controls label{
        float:left;
    }
    #controls select{
        float:right;
    }
    /* .content{
        clear:both;
        float:left;
    } */
</style>
<script lang="js">
    import Vue from 'vue';
    import {DateTime} from 'luxon'
    import utils from '../utils';

    export default Vue.extend({
        data() {
            return {
                loading: false,
                post: null,
                unapprovedOnly: false,
                showDeleted: false,
                state: "ALL",
                storeName: "",
                sortBy: "StoreName",
                sortAsc: true,
                pagenumber:0,
                count:0,
                paging: 20
            };
        },
        created() {
            // fetch the data when the view is created and the data is
            // already being observed
            var stateFilter = localStorage.getItem("stateFilter")
            if (stateFilter)
                this.state = stateFilter;
            this.fetchData();
        },
        watch: {
            // call again the method if the route changes
            '$route': 'fetchData'
        },
        methods: {
            async fetchData() {
                localStorage.setItem("stateFilter", this.state)
                this.post = null;
                this.loading = true;
                var skip = this.pagenumber * this.paging

                await utils.getData(`vendor/search?storeName=${this.storeName}&skip=${skip}&take=${this.paging}&state=${this.state}&showDeleted=${this.showDeleted}&unapprovedOnly=${this.unapprovedOnly}&sortBy=${this.sortBy}&sortAsc=${this.sortAsc}`)
                    .then(json => {
                        this.post = json;
                        this.count = this.post.length;
                        this.post = this.post.map((p) => {
                            p.createdAt = DateTime.fromISO(p.createdAt + 'Z').toLocaleString(DateTime.DATETIME_SHORT)
                            return p;
                        })
                        this.loading = false;
                        return;
                    });
            },
            add(){
                window.location = `/#/vendor-registration`
            },
            edit(id){
                //figure out how to load an id into vendor registration
                window.location = `/#/vendor-registration?id=${id}`
            },
            async sort(toSort){
                if (toSort == this.sortBy)
                    this.sortAsc = !this.sortAsc; //toggle
                else
                    this.sortAsc = true;
                this.sortBy = toSort;
                await this.fetchData();
            },
            async del(id){
                if (confirm('Are you sure?')){
                    await utils.postData(`/vendor/delete?id=${id}`)
                    await this.fetchData();
                }
            },
            async approve(id){
                await utils.postData('/vendor/approve', {id, approved:true})
                await this.fetchData();
            },
            async reject(id){
                var denialReason = prompt("What reason would you like to give them for your rejection?")
                await utils.postData('/vendor/approve', {id, approved:false, denialReason:denialReason})
                await this.fetchData();
            },
            async next(){
                this.pagenumber++;
                this.fetchData();
            },
            async prev(){
                this.pagenumber--;
                this.fetchData();
            }
        },
    });
</script>