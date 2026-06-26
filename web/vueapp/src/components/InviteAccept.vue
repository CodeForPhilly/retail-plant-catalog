<template>
  <div class="post">
    <h1>Accept invitation</h1>

    <div v-if="loading" class="fields">Checking your invitation…</div>

    <div v-else-if="!token" class="error-holder">
      This page needs a valid invitation link. Open the link from your email or ask an
      administrator to resend your invite.
    </div>

    <div v-else-if="!validation.valid" class="error-holder">
      {{ validation.message || "This invitation cannot be used." }}
    </div>

    <div v-else id="api-holder">
      <p class="invite-meta">
        Creating an account for <strong>{{ validation.email }}</strong> with role
        <strong>{{ validation.role || "User" }}</strong>. This invitation expires
        <span v-if="validation.expiresAt">{{ formatExpiry(validation.expiresAt) }}</span
        ><span v-else>in 3 days from when it was sent</span>.
      </p>
      <div class="fields">
        <input
          type="password"
          placeholder="Password"
          v-model="password"
          v-on:keyup="validateIfErrors"
        /><br />
        <input
          type="password"
          placeholder="Confirm password"
          v-model="passwordConfirm"
          v-on:keyup="validateIfErrors"
        /><br />
      </div>

      <div id="error-holder" class="error-holder" v-if="errors && errors.length > 0">
        <span class="material-symbols-outlined cancel" @click="closeError">cancel</span>
        <div v-for="error in errors" v-bind:key="error">{{ error }}</div>
      </div>
    </div>

    <input
      v-if="validation.valid"
      type="button"
      class="primary-btn submit"
      @click="submit"
      value="Create account"
    />
  </div>
</template>

<script lang="js">
import Vue from "vue";
import utils from "../utils";

function readTokenFromHash() {
  var hash = window.location.hash || "";
  var base = hash.split("?")[0] || "";
  if (base.indexOf("accept-invite") < 0) {
    return "";
  }
  if (hash.indexOf("?") < 0) {
    return "";
  }
  var query = hash.split("?")[1] || "";
  var params = new URLSearchParams(query);
  return (params.get("token") || "").trim();
}

export default Vue.extend({
  data() {
    return {
      loading: true,
      token: "",
      password: "",
      passwordConfirm: "",
      validation: {
        valid: false,
        email: "",
        role: "",
        message: "",
        expiresAt: null
      },
      errors: []
    };
  },
  async created() {
    this.token = readTokenFromHash();
    if (!this.token) {
      this.loading = false;
      return;
    }
    await this.runValidate();
  },
  methods: {
    formatExpiry(iso) {
      try {
        var d = new Date(iso);
        if (isNaN(d.getTime())) {
          return iso;
        }
        return d.toLocaleString();
      } catch (e) {
        return iso;
      }
    },
    closeError() {
      this.errors = [];
    },
    validateIfErrors() {
      if (this.errors.length) {
        this.validate();
      }
    },
    validate() {
      this.errors = [];
      if (!this.password) {
        this.errors.push("Password is a required field");
        return false;
      }
      if (this.password !== this.passwordConfirm) {
        this.errors.push("Passwords do not match");
        return false;
      }
      if (this.password.length < 10) {
        this.errors.push("Password must be at least 10 characters");
      }
      if (!/[a-z]/.test(this.password)) {
        this.errors.push("Password must have at least one lowercase letter");
      }
      if (!/[A-Z]/.test(this.password)) {
        this.errors.push("Password must have at least one UPPERCASE letter");
      }
      if (!/[0-9]/.test(this.password)) {
        this.errors.push("Password must have at least one digit");
      }
      if (!/[!@$#%^&*()]/.test(this.password)) {
        this.errors.push("Password must contain at least one symbol (!@$#%^&*())");
      }
      var valid = this.errors.length === 0;
      if (!valid) {
        var el = document.getElementById("error-holder");
        if (el) {
          el.style.animationName = "none";
          setTimeout(function () {
            el.style.animationName = "";
          }, 0);
        }
      }
      return valid;
    },
    async runValidate() {
      this.loading = true;
      var url =
        "/registrationInvite/validate?token=" + encodeURIComponent(this.token);
      var result = await utils.getData(url);
      this.loading = false;
      this.validation = {
        valid: !!result.valid,
        email: result.email || "",
        role: result.role || "",
        message: result.message || "",
        expiresAt: result.expiresAt || null
      };
    },
    async submit() {
      if (!this.validate()) {
        return;
      }
      var result = await utils.postData("/registrationInvite/accept", {
        token: this.token,
        password: this.password
      });
      if (result.success) {
        localStorage.setItem("flash", result.message || "Account created. Please sign in.");
        window.location = "/#/login";
      } else {
        this.errors.push(result.message || "Could not create account");
      }
    }
  }
});
</script>

<style scoped>
#api-holder {
  background: #ede6d6 0% 0% no-repeat padding-box;
  border-radius: 10px;
  width: 686px;
  text-align: left;
  padding-bottom: 10px;
}
.post {
  width: 686px;
  text-align: right;
}
.cancel {
  float: right;
}
.submit {
  margin: 10px;
}
.fields {
  padding: 10px 30px;
}
.invite-meta {
  padding: 16px 30px 0 30px;
  margin: 0;
  text-align: left;
  color: #2c3e50;
}
#error-holder {
  margin: 10px 40px !important;
  animation: shake 0.82s cubic-bezier(0.36, 0.07, 0.19, 0.97) both;
}
.error-holder {
  background: #f80000 0% 0% no-repeat padding-box;
  border-radius: 10px;
  color: #fff;
  padding: 15px;
  text-align: left;
  margin: 15px 0;
  width: 656px;
}

@keyframes shake {
  10%,
  90% {
    transform: translate3d(-1px, 0, 0);
  }

  20%,
  80% {
    transform: translate3d(2px, 0, 0);
  }

  30%,
  50%,
  70% {
    transform: translate3d(-4px, 0, 0);
  }

  40%,
  60% {
    transform: translate3d(4px, 0, 0);
  }
}
</style>
