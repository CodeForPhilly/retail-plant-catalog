<template>
    <div class="post">
         <input v-model="address" placeholder="Address"  ref="autocomplete" 
          required
          autocomplete="off"
          type="text"
          
        />
    </div>
</template>

<script lang="js">
    import Vue from 'vue';
    
    export default Vue.extend({
        props:["address"],
        data() {
            return {
               value: "",
               lng: 0,
               lat: 0
            };
        },
        mounted(){
            this.autocomplete = new window.google.maps.places.Autocomplete(
                (this.$refs.autocomplete),
                {types: ['geocode']}
            );

            this.autocomplete.addListener('place_changed', () => {
                let place = this.autocomplete.getPlace();
                let ac = place.address_components;
                this.lat = place.geometry.location.lat();
                this.lng = place.geometry.location.lng();
               
                console.log(`The user picked`, place);
                var state = ac.filter(a => a.types.indexOf("administrative_area_level_1") > -1)[0].short_name
                const addy = `${ac[0].short_name} ${ac[1].short_name}, ${ac[2].short_name}, ${state} `
                var retVal = {lat:this.lat, lng:this.lng, place, address:addy, state:state}
                this.$emit("placeChange", retVal)
            });
        },
        methods:{
           
        }

    });
</script>
<style scoped>
*, *::after, *::before {
  margin: 0;
  padding: 0;
  box-sizing: inherit;
}

body {
  background-color: #dcdde1;
  color: #2f3640;
  padding: 3rem;
}

.search-location {
  display: block;
  width: 60vw;
  margin: 0 auto;
  margin-top: 5vw;
  font-size: 20px;
  font-weight: 400;
  outline: none;
  height: 30px;
  line-height: 30px;
  text-align: center;
  border-radius: 10px;
}
</style>