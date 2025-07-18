<template>
  <div id="app">
    <div id="nav">
      <a :href="homeLink">
        <img
          src="/Native-Plant-Retail-Catalog-Logo-1-600x248.png"
          alt="Native Plant Retail Catalog"
          width="100%"
        />
      </a>
      <ul>
        <li v-for="link in links" v-bind:key="link.uri">
          <a :href="link.uri">{{ link.text }}</a>
        </li>
      </ul>
    </div>
    <div id="top">
      <a href="#" @click="logout" v-if="loggedIn"
        ><span class="material-symbols-outlined"> logout </span>Logout</a
      >
      <a href="#/login" v-if="!loggedIn"
        ><span class="material-symbols-outlined"> login </span>Login</a
      >
    </div>
    <div id="body">
      <div id="flash-message"></div>
      <component :is="currentView" />
    </div>
  </div>
</template>

<script>
import Users from "./components/Users.vue";
import UserCreate from "./components/UserCreate.vue";
import Vendors from "./components/Vendors.vue";
import VendorRegistration from "./components/VendorRegistration.vue";
import ApiKey from "./components/ApiKey.vue";
import ApiRegistration from "./components/ApiRegistration.vue";
import ForgotPassword from "./components/ForgotPassword.vue";
import ThankYou from "./components/ThankYou.vue";

import Places from "./components/GooglePlacesInput.vue";

import Login from "./components/Login.vue";
import utils from "./utils";

import NotFound from "./components/NotFound.vue";
import Vue from "vue";
import { ref, computed } from "vue";
import Home from "./components/Home.vue";
import { GlobalEventEmitter } from "@/events";

const routes = {
  "/": Home,
  "/users": Users,
  "/user-create": UserCreate,
  "/vendors": Vendors,
  "/vendor-registration": VendorRegistration,
  "/api-key": ApiKey,
  "/api-registration": ApiRegistration,
  "/login": Login,
  "/forgot-password": ForgotPassword,
  "/thank-you": ThankYou,
  "/places": Places
};

const currentPath = ref(window.location.hash);

function evalFlash() {
  var flash = localStorage.getItem("flash");
  var el = document.getElementById("flash-message");
  if (!(flash == "undefined" || flash === undefined)) {
    el.innerHTML = flash;
    localStorage.setItem("flash", undefined);
    el.style.display = "block";
  } else {
    el.innerHTML = "";
    el.style.display = "none";
  }
}

window.addEventListener("hashchange", () => {
  currentPath.value = window.location.hash;
  setTimeout(evalFlash, 200);
});

const currentView = computed(() => {
  var path = currentPath.value.split("?")[0].slice(1);
  return routes[path || "/"] || NotFound;
});
const staticLinks = [
  { uri: "#/users", text: "Users", role: "Admin" },
  { uri: "#/vendors", text: "Volunteers", role: "Admin" },
  { uri: "#/vendors", text: "Volunteers", role: "Volunteer" },
  { uri: "#/vendors", text: "Volunteers", role: "VolunteerPlus" },
  { uri: "#/api-registration", text: "Register API", role: "all" },
  { uri: "#/vendor-registration", text: "Register as Volunteer", role: "all" },
  { uri: "#/login", text: "Log in", role: "all", hideIfAuth: true }
];
export default Vue.extend({
  name: "App",
  data() {
    return {
      loggedIn: false,
      currentView,
      homeLink: "#/",
      links: staticLinks
    };
  },
  created() {
    this.evaluateLinks();
    GlobalEventEmitter.$on("userLoggedIn", () => {
      localStorage.setItem("loggedIn", true);
      localStorage.setItem("flash", "Login successful");
      this.evaluateLinks();
    });
    //evalFlash();
  },
  methods: {
    async logout() {
      await utils.getData(`user/logout`);
      console.log("Logging out");
      this.loggedIn = false;
      localStorage.setItem("email", "");
      localStorage.setItem("role", "");
      localStorage.setItem("loggedIn", false);
      window.location = "/#/login";
      this.evaluateLinks();
    },

    evaluateLinks() {
      this.links = staticLinks;
      this.email = localStorage.getItem("email");
      this.role = localStorage.getItem("role");
      this.loggedIn = localStorage.getItem("loggedIn") === "true";

      var currentPath = window.location.hash.split("?")[0].slice(1);
      if (currentPath === "/vendor-registration") {
        this.links = [
          { uri: "#/api-registration", text: "Register API", role: "all" },
          {
            uri: "#/vendor-registration",
            text: "Register as Volunteer",
            role: "all"
          }
        ];
        return;
      }
      if (currentPath === "/user-create") {
        this.links = [
          { uri: "#/api-registration", text: "Register API", role: "all" },
          {
            uri: "#/vendor-registration",
            text: "Register as Volunteer",
            role: "all"
          }
        ];
        return;
      }

      // Show registration links on login page and home page (root)
      if (
        currentPath === "/login" ||
        currentPath === "/" ||
        currentPath === ""
      ) {
        if (!this.loggedIn) {
          // Unauthenticated: only show the login link
          this.links = [
            { uri: "#/login", text: "Log in", role: "all", hideIfAuth: true }
          ];
        }
      }

      var allLinks = this.links.filter((l) => l.role == "all");
      var roleLinks = this.links.filter(
        (l) => l.role == this.role && this.loggedIn
      );
      this.homeLink =
        this.role == "Admin" ||
        this.role == "VolunteerPlus" ||
        this.role == "Volunteer"
          ? "#/vendors"
          : "#/";

      this.links = [...roleLinks, ...allLinks];
      if (this.email) {
        this.links = this.links.filter((l) => !l.hideIfAuth);
      }
    }
  },
  components: {
    Users,
    Vendors,
    VendorRegistration
  }
});
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
#top {
  background: #01573e 0% 0% no-repeat padding-box;
  border-radius: 10px;
  opacity: 1;
  height: 56px;
  margin-left: 300px;
}
#body {
  margin-left: 290px;
  height: 100%;
  padding-left: 30px;
}
#nav {
  /* background: #EBECF0 0% 0% no-repeat padding-box; */
  width: 300px;
  margin: 0;
  padding: 0;
  position: fixed;
  height: 100%;
  overflow: auto;
}
#flash-message {
  background-color: #ff7f02;
  border: dashed 2px #000;
  padding: 15px;
  margin: 10px;
  display: none;
  color: #fff;
  font-weight: bold;
}
#nav ul li {
  text-align: left;
  list-style-type: none;
}
#nav ul li a {
  color: #01573e;
  opacity: 1;
  text-transform: uppercase;
  font: serif;
  font-size: 19px;
  letter-spacing: 0px;
  text-decoration: none;
}
#nav ul li a:hover {
  text-decoration: underline;
}
#body h1 {
  text-align: left;
  font: normal normal normal 40px/49px Proxima Nova, Arial;
  letter-spacing: 0px;
  color: #707070;
  padding-left: 10px;
}
input[type="text"],
textarea,
input[type="password"] {
  border-radius: 5px;
  border: 1px solid #c1c1c1;
  padding: 5px 15px;
  font-size: 1.2em;
  width: 422px;
  margin: 13px;
  color: #707070;
  font-family: sans-serif;
}
.primary-btn {
  background: #ff7f02 0% 0% no-repeat padding-box;
  border-radius: 5px;
  /* font: normal normal normal 21px/25px Lato; */
  font: serif;
  font-size: 1.4em;
  letter-spacing: 0px;
  color: #ffffff;
  opacity: 1;
  padding: 9px 25px;
  border: none;
}
ul.errors {
  list-style-type: none;
  color: #ff0000;
  padding-left: 20px;
}
.error-box {
  background: #ffffff 0% 0% no-repeat padding-box;
  border: 1px solid #ff0000;
  opacity: 1;
  /* margin-bottom:15px; */
  margin: 15px;
}
.grid {
  width: 80%;
  border: none;
}
.grid th {
  background-color: #fff;
  border: none;
  font-weight: normal;
}
.grid td {
  border: 1px solid #fff;
  padding: 10px;
}
.grid tr {
  background: #ebecf0 0% 0% no-repeat padding-box;
  border-radius: 10px;
  border: none;
}
.grid tr:hover {
  background: #f6f6f8 0% 0% no-repeat padding-box;
}
.grid tr:nth-child(even) {
  background-color: #d5d6db;
}
.skipnext span.material-symbols-outlined {
  cursor: pointer;
  font-size: 35px;
  float: left;
}
#top a {
  text-decoration: none;
  color: #fff;
  float: right;
  padding-right: 20px;
  padding-top: 16px;
  text-transform: uppercase;
}
#top a span.material-symbols-outlined {
  font-size: 17px;
  position: relative;
  top: 3px;
  padding-right: 5px;
}
#top a:hover {
  text-decoration: underline;
}
.error-holder {
  background: #f80000 0% 0% no-repeat padding-box;
  border-radius: 10px;
  color: #fff;
  padding: 15px;
  text-align: left;
  margin-bottom: 10px;
}
span.material-symbols-outlined {
  cursor: pointer;
}
</style>
