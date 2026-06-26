<template>
  <div class="post">
    <div v-if="loading" class="loading">Loading...</div>
    <h1>Users</h1>
    <div v-if="post" class="content">
      <div class="invite-panel">
        <h2>Invite user</h2>
        <p class="invite-help">
          Send a 3-day invitation by email and get a shareable link. Choose the role the
          person will have after they accept. Expired or accepted invites cannot be reused.
        </p>
        <div class="invite-row">
          <input
            type="email"
            v-model="inviteEmail"
            placeholder="Invitee email"
            class="invite-email"
          />
          <select v-model="inviteRole" class="invite-role" aria-label="Role for new user">
            <option value="User">User</option>
            <option value="Admin">Admin</option>
            <option value="Volunteer">Volunteer</option>
            <option value="VolunteerPlus">VolunteerPlus</option>
          </select>
          <input
            type="button"
            class="primary-btn"
            value="Send invitation"
            @click="sendInvite"
          />
        </div>
        <p v-if="inviteMessage" :class="inviteError ? 'invite-msg error' : 'invite-msg'">
          {{ inviteMessage }}
        </p>
        <div v-if="lastInviteLink" class="invite-link-box">
          <label>Shareable link</label>
          <div class="invite-link-row">
            <input type="text" readonly class="invite-link-input" :value="lastInviteLink" />
            <button type="button" class="primary-btn" @click="copyInviteLink">Copy</button>
          </div>
          <p v-if="lastInviteExpires" class="invite-expires">
            Expires: {{ lastInviteExpires }}
          </p>
        </div>
      </div>

      <label class="show-admin"
        ><input type="checkbox" v-model="showAdminOnly" />Show Admin
        Only?</label
      >
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
            <td class="pointer" :title="user.intendedUse">
              {{ user.apiKey ? "yes" : "no" }}
            </td>
            <td>
              <!-- Promote to VolunteerPlus -->
              <span
                v-if="user.role === 'Volunteer'"
                class="material-symbols-outlined"
                @click="promoteToVolunteerPlus(user.id)"
                title="Promote to VolunteerPlus"
              >
                admin_panel_settings
              </span>
              <!-- Promote to Admin -->
              <span
                v-if="user.role !== 'Admin'"
                class="material-symbols-outlined"
                @click="promote(user.id)"
                title="Promote to Admin"
              >
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
            paging: 20,
            inviteEmail: '',
            inviteRole: 'User',
            lastInviteLink: '',
            lastInviteExpires: '',
            inviteMessage: '',
            inviteError: false
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
                    var lineArray = ["Id, Email, Role, Verified, Intended Use, CreatedAt, ModifiedAt"];
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
        // Promote User to Admin
        async promote(id){
            if (confirm("Are you sure you want to promote this user to Admin?")){
                await utils.postData(`/user/promote?id=${id}`)
            }
            this.fetchData();
        },
        // Promote Volunteer to VolunteerPlus
        async promoteToVolunteerPlus(id){
            if (confirm("Are you sure you want to promote this user to VolunteerPlus?")){
                await utils.postData(`/user/promoteVolunteerPlus?id=${id}`)
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
        },
        async sendInvite(){
            this.inviteMessage = '';
            this.inviteError = false;
            this.lastInviteLink = '';
            this.lastInviteExpires = '';
            var email = (this.inviteEmail || '').trim();
            if (!email){
                this.inviteError = true;
                this.inviteMessage = 'Enter an email address';
                return;
            }
            var result = await utils.postData('/registrationInvite/create', {
                email: email,
                role: this.inviteRole
            });
            if (result.success){
                this.inviteMessage = result.message || 'Invitation sent.';
                this.lastInviteLink = result.inviteLink || '';
                if (result.expiresAt){
                    try {
                        this.lastInviteExpires = new Date(result.expiresAt).toLocaleString();
                    } catch (e) {
                        this.lastInviteExpires = result.expiresAt;
                    }
                }
                this.inviteEmail = '';
            } else {
                this.inviteError = true;
                this.inviteMessage = result.message || 'Could not create invitation';
            }
        },
        copyInviteLink(){
            if (!this.lastInviteLink) return;
            var ta = document.createElement('textarea');
            ta.value = this.lastInviteLink;
            document.body.appendChild(ta);
            ta.select();
            try {
                document.execCommand('copy');
                this.inviteMessage = 'Link copied to clipboard';
                this.inviteError = false;
            } catch (e) {
                this.inviteMessage = 'Copy failed — select the link manually';
                this.inviteError = true;
            }
            document.body.removeChild(ta);
        }
    },
});
</script>
<style scoped>
.resend,
.pointer {
  cursor: pointer;
}
.content {
  text-align: left;
  width: 1000px;
}
.export-csv {
  float: right;
}
.export-csv:hover {
  text-decoration: underline;
  cursor: pointer;
}
.grid {
  width: 100%;
}
.show-admin {
  cursor: pointer;
}
.invite-panel {
  margin-bottom: 24px;
  padding: 16px 0;
  border-bottom: 1px solid #c1c1c1;
  max-width: 900px;
}
.invite-panel h2 {
  font-size: 1.25rem;
  margin: 0 0 8px 0;
  text-align: left;
}
.invite-help {
  text-align: left;
  margin: 0 0 12px 0;
  color: #555;
  font-size: 0.95rem;
}
.invite-row {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 12px;
}
.invite-email,
.invite-role {
  box-sizing: border-box;
  margin: 0 !important;
  padding: 10px 15px;
  font-size: 1.2em;
  line-height: 1.35;
  min-height: 48px;
  border-radius: 5px;
  border: 1px solid #c1c1c1;
  color: #707070;
  font-family: sans-serif;
  vertical-align: middle;
}
.invite-email {
  width: 320px !important;
  max-width: 100%;
}
.invite-role {
  width: 200px !important;
  max-width: 100%;
}
.invite-msg {
  text-align: left;
  margin-top: 10px;
}
.invite-msg.error {
  color: #c00;
  font-weight: bold;
}
.invite-link-box {
  margin-top: 16px;
  text-align: left;
}
.invite-link-box label {
  display: block;
  margin-bottom: 6px;
  font-weight: 600;
}
.invite-link-row {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  align-items: center;
}
.invite-link-input {
  flex: 1 1 280px;
  min-width: 200px;
  margin: 0 !important;
}
.invite-expires {
  margin: 8px 0 0 0;
  font-size: 0.9rem;
  color: #555;
}
</style>
