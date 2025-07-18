<template>
  <div class="holder">
    <h1>Welcome</h1>

    <a class="volunteer-listing" href="/#/vendors">
      <span v-if="role == 'User'">Register</span
      ><span
        v-if="role == 'Volunteer' || role == 'VolunteerPlus' || role == 'Admin'"
        >Edit</span
      >
      Volunteer Listing
      <span class="material-symbols-outlined"> arrow_circle_right </span></a
    >
    <a class="get-api" href="/#/api-key"
      >Get API<span class="material-symbols-outlined">
        arrow_circle_right
      </span></a
    >
  </div>
</template>

<script lang="js">
import Vue from 'vue';

export default Vue.extend({
    data() {
        return {
            role:""
        };
    },created(){
        this.role = localStorage.getItem('role')
    }
});
</script>
<style scoped>
a {
  display: block;
  text-decoration: none;
  font-family: Arial;
  font-size: 35px;
  text-align: left;
  padding: 40px;
  width: 500px;
  float: left;
  margin: 15px;
  opacity: 1;
}
a:hover {
  opacity: 0.8;
}
.material-symbols-outlined {
  font-size: 1em;
  float: right;
}
.holder {
  padding: 30px;
}
.volunteer-listing {
  color: #707070;
  /* UI Properties */
  background: #ede6d6 0% 0% no-repeat padding-box;
  border-radius: 10px;
}
.get-api {
  color: #fff;
  /* UI Properties */
  background: #333333 0% 0% no-repeat padding-box;
  border-radius: 10px;
}
</style>
