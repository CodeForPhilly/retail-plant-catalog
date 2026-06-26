<template>
  <div class="post">
    <div v-if="loading" class="loading">Loading...</div>
    <h1>
      Volunteers
      <a @click="add()" title="Add Volunteer"
        ><span class="material-symbols-outlined"> add_box </span></a
      >
      <a @click="openExportDialog()" class="export-csv" title="Bulk Export"
        ><span class="material-symbols-outlined"> download </span></a
      >
    </h1>

    <div id="controls">
      <input
        type="text"
        id="search"
        v-model="storeName"
        @keyup="fetchData()"
        placeholder="Search..."
      />

      <label
        ><input
          type="checkbox"
          v-model="unapprovedOnly"
          @change="fetchData()"
        />Show Only Unapproved?</label
      >
      <label style="padding-left: 10px"
        ><input
          type="checkbox"
          v-model="showDeleted"
          @change="fetchData()"
        />Show Deleted?</label
      ><br />
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
      <select v-model="partnerFilter" @change="pagenumber = 0; fetchData()" id="partnerFilter">
        <option value="ALL">Filter by Partner</option>
        <option value="__UNAFFILIATED__">Unaffiliated</option>
        <option v-for="p in partners" :key="p.id" :value="p.id">{{ p.displayName }}</option>
      </select>
    </div>
    <br style="clear: both" />
    <div v-if="post" class="content">
      <table class="grid">
        <thead>
          <tr>
            <th>
              <a @click="sort('StoreName')">Name</a>
              {{ sortBy != "StoreName" ? "" : sortAsc ? "v" : "^" }}
            </th>
            <th>
              <a @click="sort('PlantCount')">Detected Plants</a>
              {{ sortBy != "PlantCount" ? "" : sortAsc ? "v" : "^" }}
            </th>
            <th>Partner</th>
            <th>
              <a @click="sort('CreatedAt')">Created At</a>
              {{ sortBy != "CreatedAt" ? "" : sortAsc ? "v" : "^" }}
            </th>
            <th>Last Crawled</th>
            <th>Approved?</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="vendor in post" :key="vendor.id">
            <td>
              {{ vendor.storeName }}
              <span v-if="vendor.crawlInProgress" class="material-symbols-outlined spinner" title="Crawl in progress">sync</span>
              <span v-if="vendor.crawlErrors > 0" class="error-badge">{{
                vendor.crawlErrors
              }}</span>
            </td>
            <td>{{ vendor.plantCount }}</td>
            <td>
              <span v-if="vendor.partner" class="partner-tag">{{ vendor.partner }}</span>
              <span v-else class="partner-none">—</span>
            </td>
            <td>{{ vendor.createdAt }}</td>
            <td>{{ vendor.lastCrawledDisplay }}</td>
            <td>{{ vendor.approved }}</td>
            <td>
              <span class="material-symbols-outlined" :class="{ 'action-disabled': vendor.crawlInProgress }" @click="!vendor.crawlInProgress && edit(vendor.id)">
                edit
              </span>
              <span class="material-symbols-outlined" :class="{ 'action-disabled': vendor.crawlInProgress }" @click="!vendor.crawlInProgress && del(vendor.id)">
                delete
              </span>
              <span
                class="material-symbols-outlined"
                :class="{ 'action-disabled': vendor.crawlInProgress }"
                v-if="!vendor.approved"
                @click="!vendor.crawlInProgress && approve(vendor.id)"
              >
                thumb_up
              </span>
              <span
                class="material-symbols-outlined"
                :class="{ 'action-disabled': vendor.crawlInProgress }"
                @click="!vendor.crawlInProgress && reject(vendor.id)"
              >
                thumb_down
              </span>
            </td>
          </tr>
        </tbody>
      </table>

      <a class="skipnext" @click="prev()" v-if="pagenumber > 0">
        <span class="material-symbols-outlined"> skip_previous </span>
      </a>
      <a class="skipnext" @click="next()" v-if="count == paging">
        <span class="material-symbols-outlined"> skip_next </span>
      </a>
    </div>

    <div v-if="exportDialogOpen" class="export-overlay" @click.self="closeExportDialog">
      <div class="export-modal">
        <h2>Bulk Export</h2>
        <p class="export-filters-note">Nurseries, Nursery Plant URLs, and Plant–Nursery use current list filters: State={{ state }}, Unapproved only={{ unapprovedOnly }}, Partner={{ partnerFilter }}</p>
        <div class="export-checkboxes">
          <label><input type="checkbox" v-model="exportPlants" /> Plants (Plants.csv)</label>
          <label><input type="checkbox" v-model="exportNurseries" /> Nurseries (Nurseries.csv)</label>
          <label><input type="checkbox" v-model="exportNurseryPlantUrls" /> Nursery Plant URLs (Nursery_Plant_URLs.csv)</label>
          <label><input type="checkbox" v-model="exportPlantNursery" /> Plant–Nursery junction (Plant_Nursery.csv)</label>
        </div>
        <div v-if="exportStatus" class="export-status">{{ exportStatus }}</div>
        <div class="export-actions">
          <button type="button" class="export-btn" :disabled="exportInProgress || !hasExportSelection" @click="runExport">
            {{ exportInProgress ? 'Exporting…' : 'Export' }}
          </button>
          <button type="button" class="export-btn secondary" @click="closeExportDialog">Close</button>
        </div>
      </div>
    </div>
  </div>
</template>
<style>
#stateFilter {
  float: left !important;
  font-family: sans-serif;
  padding: 5px 15px;
  border-radius: 5px;
}
#partnerFilter {
  font-family: sans-serif;
  padding: 5px 15px;
  border-radius: 5px;
  margin-right: 10px;
}
.partner-tag {
  display: inline-block;
  background-color: #01573e;
  color: #fff;
  border-radius: 12px;
  padding: 2px 10px;
  font-size: 0.85em;
}
.partner-none {
  color: #aaa;
}
#search {
  float: right;
}
th a:hover {
  text-decoration: underline;
  cursor: pointer;
}
#controls {
  width: 80%;
}
#controls label {
  float: left;
}
#controls select {
  float: right;
}
.spinner {
  margin-left: 6px;
  vertical-align: middle;
  animation: spin 1s linear infinite;
}
@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}
.action-disabled {
  opacity: 0.5;
  cursor: not-allowed;
  pointer-events: none;
}
.error-badge {
  display: inline-block;
  background-color: #e53935;
  color: white;
  border-radius: 50%;
  min-width: 20px;
  height: 20px;
  text-align: center;
  font-size: 12px;
  line-height: 20px;
  margin-left: 8px;
  font-weight: bold;
  padding: 0 4px;
}
.export-csv {
  margin-left: 12px;
  cursor: pointer;
  text-decoration: none;
  color: inherit;
}
.export-csv:hover {
  text-decoration: underline;
}
.export-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0,0,0,0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}
.export-modal {
  background: white;
  padding: 20px;
  border-radius: 8px;
  min-width: 380px;
  box-shadow: 0 4px 20px rgba(0,0,0,0.2);
}
.export-modal h2 {
  margin-top: 0;
}
.export-filters-note {
  font-size: 12px;
  color: #555;
  margin: 8px 0 16px 0;
}
.export-checkboxes {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 16px;
}
.export-checkboxes label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}
.export-status {
  min-height: 20px;
  margin-bottom: 12px;
  font-size: 13px;
  color: #333;
}
.export-actions {
  display: flex;
  gap: 10px;
}
.export-btn {
  padding: 8px 16px;
  cursor: pointer;
  border-radius: 4px;
  border: 1px solid #ccc;
  background: #f5f5f5;
}
.export-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
.export-btn.secondary {
  background: #fff;
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
            partnerFilter: "ALL",
            partners: [],
            storeName: "",
            sortBy: "StoreName",
            sortAsc: true,
            pagenumber:0,
            count:0,
            paging: 20,
            exportDialogOpen: false,
            exportPlants: true,
            exportNurseries: true,
            exportNurseryPlantUrls: false,
            exportPlantNursery: false,
            exportStatus: '',
            exportInProgress: false,
            crawlPollTimer: null
        };
    },
    computed: {
        hasExportSelection() {
            return this.exportPlants || this.exportNurseries || this.exportNurseryPlantUrls || this.exportPlantNursery;
        }
    },
    created() {
        // fetch the data when the view is created and the data is
        // already being observed
        var stateFilter = localStorage.getItem("stateFilter")
        if (stateFilter)
            this.state = stateFilter;
        this.loadPartners();
        this.fetchData();
    },
    watch: {
        // call again the method if the route changes
        '$route': 'fetchData'
    },
    beforeDestroy() {
        if (this.crawlPollTimer) {
            clearInterval(this.crawlPollTimer);
            this.crawlPollTimer = null;
        }
    },
    methods: {
        async fetchData(silentRefresh) {
            localStorage.setItem("stateFilter", this.state);
            if (!silentRefresh) {
                this.post = null;
                this.loading = true;
            }
            var skip = this.pagenumber * this.paging;

            var partnerParam = '';
            if (this.partnerFilter === '__UNAFFILIATED__') partnerParam = '&partner=';
            else if (this.partnerFilter && this.partnerFilter !== 'ALL') partnerParam = '&partner=' + encodeURIComponent(this.partnerFilter);

            await utils.getData(`vendor/search?storeName=${this.storeName}&skip=${skip}&take=${this.paging}&state=${this.state}&showDeleted=${this.showDeleted}&unapprovedOnly=${this.unapprovedOnly}&sortBy=${this.sortBy}&sortAsc=${this.sortAsc}${partnerParam}`)
                .then(json => {
                    this.post = json;
                    this.count = this.post.length;
                    this.post = this.post.map((p) => {
                        p.createdAt = DateTime.fromISO(p.createdAt + 'Z').toLocaleString(DateTime.DATETIME_SHORT);
                        var lc = p.lastCrawled;
                        p.lastCrawledDisplay = !lc ? 'Never' : DateTime.fromISO((lc + '').endsWith('Z') ? lc : lc + 'Z').toLocaleString(DateTime.DATETIME_SHORT);
                        return p;
                    });
                    this.loading = false;

                    var anyInProgress = this.post.some((p) => p.crawlInProgress);
                    if (anyInProgress && !this.crawlPollTimer) {
                        this.crawlPollTimer = setInterval(() => this.fetchData(true), 5000);
                    } else if (!anyInProgress && this.crawlPollTimer) {
                        clearInterval(this.crawlPollTimer);
                        this.crawlPollTimer = null;
                    }
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
        },
        openExportDialog() {
            this.exportDialogOpen = true;
            this.exportStatus = '';
        },
        closeExportDialog() {
            if (!this.exportInProgress) this.exportDialogOpen = false;
        },
        async loadPartners() {
            try {
                const list = await utils.getData('/vendor/Partners');
                this.partners = Array.isArray(list) ? list : [];
            } catch (e) {
                this.partners = [];
            }
        },
        buildExportQuery() {
            const params = new URLSearchParams();
            if (this.state) params.set('state', this.state);
            if (this.unapprovedOnly) params.set('approved', 'false');
            if (this.partnerFilter === '__UNAFFILIATED__') params.set('partner', '');
            else if (this.partnerFilter && this.partnerFilter !== 'ALL') params.set('partner', this.partnerFilter);
            const q = params.toString();
            return q ? '?' + q : '';
        },
        async runExport() {
            if (!this.hasExportSelection || this.exportInProgress) return;
            this.exportInProgress = true;
            const query = this.buildExportQuery();
            try {
                if (this.exportPlants) {
                    this.exportStatus = 'Exporting Plants…';
                    await utils.downloadExport('/Export/Plants', 'Plants.csv');
                    await new Promise((r) => setTimeout(r, 400));
                }
                if (this.exportNurseries) {
                    this.exportStatus = 'Exporting Nurseries…';
                    await utils.downloadExport('/Export/Nurseries' + query, 'Nurseries.csv');
                    await new Promise((r) => setTimeout(r, 400));
                }
                if (this.exportNurseryPlantUrls) {
                    this.exportStatus = 'Exporting Nursery Plant URLs…';
                    await utils.downloadExport('/Export/NurseryPlantUrls' + query, 'Nursery_Plant_URLs.csv');
                    await new Promise((r) => setTimeout(r, 400));
                }
                if (this.exportPlantNursery) {
                    this.exportStatus = 'Exporting Plant–Nursery…';
                    await utils.downloadExport('/Export/PlantNursery' + query, 'Plant_Nursery.csv');
                }
                this.exportStatus = 'Export complete.';
            } catch (e) {
                this.exportStatus = 'Error: ' + (e.message || 'Export failed.');
            } finally {
                this.exportInProgress = false;
            }
        }
    },
});
</script>
