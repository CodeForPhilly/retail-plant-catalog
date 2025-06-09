<!-- Google Places API (new) -->
<template>
  <div class="autocomplete-wrapper">
    <div ref="autocompleteContainer"></div>
  </div>
</template>

<script>
import Vue from "vue";

function mapsReady(){
    return window.google && window.google.maps &&
      window.google.maps.places &&
      window.google.maps.places.PlaceAutocompleteElement
}

export default Vue.extend({
  props: ["address"],
  mounted() {
    var isMapsReady = false;
    while (!isMapsReady){
       isMapsReady = mapsReady();
    }
     console.log("Google Places input has mounted successfully");
    const placeAutocomplete =
      new window.google.maps.places.PlaceAutocompleteElement();
    placeAutocomplete.types = ["geocode"];
    placeAutocomplete.componentRestrictions = { country: ["us"] };

    this.$refs.autocompleteContainer.appendChild(placeAutocomplete);

    placeAutocomplete.addEventListener(
      "gmp-select",
      async ({ placePrediction }) => {
        const place = placePrediction.toPlace();
        console.log("User selected " + place.location.l);
        await place.fetchFields({
          fields: ["formattedAddress", "location", "addressComponents"],
        });

        const lat = place.location.lat();
        const lng = place.location.lng();

        const components = place.addressComponents;

        const state =
          components.find((c) =>
            c.types.includes("administrative_area_level_1")
          )?.shortText || "";

        const address = `${components[0]?.shortText || ""} ${
          components[1]?.shortText || ""
        }, ${components[2]?.shortText || ""}, ${state}`;

        this.$emit("placeChange", {
          lat,
          lng,
          address,
          state,
          place,
        });
      }
    );
  },
});
</script>

<style scoped>
/* Note: The PlaceAutocompleteElement uses shadow DOM, which limits styling flexibility.
Only outer wrapper styles (e.g., size, margin) can be applied reliably. */
.autocomplete-wrapper {
  width: 100%;
  max-width: 455.27px;
  margin-left: 12px;
  margin-bottom: 5px;
  padding: 0;
  border-radius: 6px;
  background: white;
  box-sizing: border-box;
}
</style>

<!-- Google Places API (Legacy) -->
<!-- <template>
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

    }); -->
<!-- <style scoped>
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
</style> -->
