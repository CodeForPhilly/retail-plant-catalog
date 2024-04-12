(function(e){function t(t){for(var r,i,n=t[0],l=t[1],d=t[2],u=0,h=[];u<n.length;u++)i=n[u],Object.prototype.hasOwnProperty.call(a,i)&&a[i]&&h.push(a[i][0]),a[i]=0;for(r in l)Object.prototype.hasOwnProperty.call(l,r)&&(e[r]=l[r]);c&&c(t);while(h.length)h.shift()();return o.push.apply(o,d||[]),s()}function s(){for(var e,t=0;t<o.length;t++){for(var s=o[t],r=!0,n=1;n<s.length;n++){var l=s[n];0!==a[l]&&(r=!1)}r&&(o.splice(t--,1),e=i(i.s=s[0]))}return e}var r={},a={app:0},o=[];function i(t){if(r[t])return r[t].exports;var s=r[t]={i:t,l:!1,exports:{}};return e[t].call(s.exports,s,s.exports,i),s.l=!0,s.exports}i.m=e,i.c=r,i.d=function(e,t,s){i.o(e,t)||Object.defineProperty(e,t,{enumerable:!0,get:s})},i.r=function(e){"undefined"!==typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(e,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(e,"__esModule",{value:!0})},i.t=function(e,t){if(1&t&&(e=i(e)),8&t)return e;if(4&t&&"object"===typeof e&&e&&e.__esModule)return e;var s=Object.create(null);if(i.r(s),Object.defineProperty(s,"default",{enumerable:!0,value:e}),2&t&&"string"!=typeof e)for(var r in e)i.d(s,r,function(t){return e[t]}.bind(null,r));return s},i.n=function(e){var t=e&&e.__esModule?function(){return e["default"]}:function(){return e};return i.d(t,"a",t),t},i.o=function(e,t){return Object.prototype.hasOwnProperty.call(e,t)},i.p="/";var n=window["webpackJsonp"]=window["webpackJsonp"]||[],l=n.push.bind(n);n.push=t,n=n.slice();for(var d=0;d<n.length;d++)t(n[d]);var c=l;o.push([0,"chunk-vendors"]),s()})({0:function(e,t,s){e.exports=s("56d7")},"0d25":function(e,t,s){},1298:function(e,t,s){},"16c9":function(e,t,s){"use strict";s("b606")},1932:function(e,t,s){"use strict";s("962d")},"1d2e":function(e,t,s){},"205b":function(e,t,s){"use strict";s("1298")},"396b":function(e,t,s){"use strict";s("1d2e")},"3a66":function(e,t,s){},"56d7":function(e,t,s){"use strict";s.r(t);var r=s("2b0e"),a=function(){var e=this,t=e._self._c;e._self._setupProxy;return t("div",{attrs:{id:"app"}},[t("div",{attrs:{id:"nav"}},[e._m(0),t("ul",e._l(e.links,(function(s){return t("li",{key:s.uri},[t("a",{attrs:{href:s.uri}},[e._v(e._s(s.text))])])})),0)]),t("div",{attrs:{id:"top"}},[e.loggedIn&&"Vendor"==e.role?t("a",{attrs:{href:"#/vendor-registration"}},[t("span",{staticClass:"material-symbols-outlined"},[e._v(" account_circle ")]),e._v("Account")]):e._e(),e.loggedIn?t("a",{attrs:{href:"#"},on:{click:e.logout}},[t("span",{staticClass:"material-symbols-outlined"},[e._v(" logout ")]),e._v("Logout")]):e._e(),e.loggedIn?e._e():t("a",{attrs:{href:"#/login"}},[t("span",{staticClass:"material-symbols-outlined"},[e._v(" login ")]),e._v("Login")])]),t("div",{attrs:{id:"body"}},[t("div",{attrs:{id:"flash-message"}}),t(e.currentView,{tag:"component"})],1)])},o=[function(){var e=this,t=e._self._c;e._self._setupProxy;return t("a",{attrs:{href:"#/"}},[t("img",{attrs:{src:"/pac.svg",alt:"Plant Agents Collective",width:"100%"}})])}],i=function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[e.loading?t("div",{staticClass:"loading"},[e._v(" Loading... ")]):e._e(),t("h1",[e._v("Users")]),e.post?t("div",{staticClass:"content"},[t("label",{staticClass:"show-admin"},[t("input",{directives:[{name:"model",rawName:"v-model",value:e.showAdminOnly,expression:"showAdminOnly"}],attrs:{type:"checkbox"},domProps:{checked:Array.isArray(e.showAdminOnly)?e._i(e.showAdminOnly,null)>-1:e.showAdminOnly},on:{change:function(t){var s=e.showAdminOnly,r=t.target,a=!!r.checked;if(Array.isArray(s)){var o=null,i=e._i(s,o);r.checked?i<0&&(e.showAdminOnly=s.concat([o])):i>-1&&(e.showAdminOnly=s.slice(0,i).concat(s.slice(i+1)))}else e.showAdminOnly=a}}}),e._v("Show Admin Only?")]),t("a",{staticClass:"export-csv",on:{click:e.exportCSV}},[e._v("Export as CSV")]),t("table",{staticClass:"grid"},[e._m(0),t("tbody",e._l(e.post,(function(s){return t("tr",{key:s.id},[t("td",[e._v(e._s(s.email))]),t("td",[e._v(e._s(s.role))]),t("td",[e._v(e._s(s.verified))]),t("td",{staticClass:"pointer",attrs:{title:s.intendedUse}},[e._v(" "+e._s(s.apiKey?"yes":"no"))]),t("td",[t("span",{staticClass:"material-symbols-outlined",attrs:{title:"Promote to Admin"},on:{click:function(t){return e.promote(s.id)}}},[e._v(" admin_panel_settings ")])]),t("td",[t("span",{staticClass:"material-symbols-outlined",on:{click:function(t){return e.del(s.id)}}},[e._v(" delete ")])]),t("td",[t("a",{staticClass:"resend",on:{click:function(t){return e.resend(s.id)}}},[e._v("Resend")])])])})),0)]),e.pagenumber>0?t("a",{on:{click:function(t){return e.prev()}}},[e._v("Prev")]):e._e(),e.count==e.paging?t("a",{on:{click:function(t){return e.next()}}},[e._v("Next")]):e._e()]):e._e()])},n=[function(){var e=this,t=e._self._c;return t("thead",[t("tr",[t("th",[e._v("Email")]),t("th",[e._v("Role")]),t("th",[e._v("Verified")]),t("th",[e._v("API Key")]),t("th",{attrs:{colspan:"3"}},[e._v("Actions")])])])}];s("14d9");async function l(e,t={},s="GET"){return await fetch(e,{method:s,mode:"cors",cache:"no-cache",credentials:"same-origin",headers:{"Content-Type":"application/json"},redirect:"manual",referrerPolicy:"no-referrer",body:"GET"==s?void 0:JSON.stringify(t)}).then(e=>0==e.status&&e.url?(window.location="/#/login",{}):e.json())}var d={async getData(e){return l(e)},async putData(e,t={}){return l(e,t,"PUT")},async postData(e,t={}){return l(e,t,"POST")},async delData(e){return l(e,"DELETE")}};function c(e,t){var s=document.createElement("a");s.setAttribute("href","data:text/plain;charset=utf-8,"+encodeURIComponent(t)),s.setAttribute("download",e),s.style.display="none",document.body.appendChild(s),s.click(),document.body.removeChild(s)}var u=r["default"].extend({data(){return{loading:!1,showAdminOnly:!1,post:null,pagenumber:0,count:0,paging:20}},created(){this.fetchData()},watch:{$route:"fetchData",showAdminOnly:"fetchData"},methods:{async fetchData(){this.post=null,this.loading=!0;var e=this.pagenumber*this.paging;await d.getData(`user/search?showAdminOnly=${this.showAdminOnly}&skip=${e}&take=${this.paging}`,{redirect:"manual"}).then(e=>{this.post=e,this.count=this.post.length,this.loading=!1})},async exportCSV(){this.loading=!0;const e=new Date;let t=e.getTimezoneOffset();await d.getData(`user/export?showAdminOnly=${this.showAdminOnly}&offset=${t}`).then(e=>{this.post=e;var t=["Id,Email,Role, Verified, Intended Use,CreatedAt, ModifiedAt"];this.post.forEach((function(e){var s=e.join(",");t.push(s)}));var s=t.join("\n");c("users.csv",s),this.count=this.post.length,this.loading=!1,this.fetchData()})},async promote(e){confirm("Are you sure you want to promote this user to Admin?")&&await d.postData("/user/promote?id="+e),this.fetchData()},async del(e){confirm("Are you sure?")&&(await d.postData("/user/delete?id="+e),await this.fetchData())},async resend(e){await d.postData("/user/resend?id="+e),alert("Verification email has been resent")},next(){this.pagenumber++,this.fetchData()},prev(){this.pagenumber--,this.fetchData()}}}),h=u,p=(s("8b59"),s("2877")),m=Object(p["a"])(h,i,n,!1,null,"0288a4f5",null),v=m.exports,g=function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[t("h1",[e._v("Create User "),e.type?t("span",[e._v("("+e._s(e.type)+")")]):e._e()]),t("div",{attrs:{id:"api-holder"}},[t("div",{staticClass:"fields"},[t("input",{directives:[{name:"model",rawName:"v-model",value:e.user.email,expression:"user.email"}],attrs:{type:"text",placeholder:"Email"},domProps:{value:e.user.email},on:{keyup:e.validateIfErrors,input:function(t){t.target.composing||e.$set(e.user,"email",t.target.value)}}}),t("br"),t("input",{directives:[{name:"model",rawName:"v-model",value:e.user.password,expression:"user.password"}],attrs:{type:"password",placeholder:"Password"},domProps:{value:e.user.password},on:{keyup:e.validateIfErrors,input:function(t){t.target.composing||e.$set(e.user,"password",t.target.value)}}}),t("br")]),e.errors&&e.errors.length>0?t("div",{staticClass:"error-holder",attrs:{id:"error-holder"}},[t("span",{staticClass:"material-symbols-outlined cancel",on:{click:e.closeError}},[e._v(" cancel ")]),e._l(e.errors,(function(s){return t("div",{key:s},[e._v(" "+e._s(s)+" ")])}))],2):e._e()]),t("input",{staticClass:"primary-btn submit",attrs:{type:"button",value:"Submit"},on:{click:e.createUser}})])},f=[],y=r["default"].extend({data(){return{user:{email:"",password:""},type:"",shake:!0,message:"",errors:[]}},created(){try{var e=window.location.hash;if(e.indexOf("?")){const t=e.split("?")[1],s=t.split("=");console.log("Params",s),this.type=s[1]}else this.type="Vendor"}catch(t){this.type="Vendor"}},methods:{closeError(){this.errors=[]},validateIfErrors(){this.errors.length&&this.validate()},validate(){if(this.errors=[],this.user.email||this.errors.push("Email is a required field"),!this.user.password)return this.errors.push("Password is a required field"),!1;/(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])/.test(this.user.email)||this.errors.push("Email must be valid"),this.user.password.length<10&&this.errors.push("Password must be at least 10 characters"),/[a-z]/.test(this.user.password)||this.errors.push("Password must have a least one lowercase letter"),/[A-Z]/.test(this.user.password)||this.errors.push("Password must have at least one UPPERCASE letter"),/[0-9]/.test(this.user.password)||this.errors.push("Password must have at least one digit"),/[!@$#%^&*()]/.test(this.user.password)||this.errors.push("Password must contain at least one symbol");var e=0==this.errors.length;if(!e){var t=document.getElementById("error-holder");t.style.animationName="none",setTimeout(()=>{t.style.animationName=""},0)}return e},async createUser(){if(this.validate()){var e;e="api"==this.type.toLowerCase()?"#/login?redirectUrl=%2F%23%2Fapi-registration":"#/login?redirectUrl=%2F%23%2Fvendor-registration";var t=await d.postData("/user/create",{user:this.user,redirectUrl:e});console.log("Result",t),localStorage.setItem("email",this.user.email),localStorage.setItem("flash","Check your email for confirmation link"),t.success?window.location="/#/thank-you":this.errors.push(t.message)}}}}),_=y,b=(s("7311"),Object(p["a"])(_,g,f,!1,null,"0adcabee",null)),w=b.exports,x=function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[e.loading?t("div",{staticClass:"loading"},[e._v(" Loading... ")]):e._e(),t("h1",[e._v("Vendors")]),e.post?t("div",{staticClass:"content"},[t("table",{staticClass:"grid"},[e._m(0),t("tbody",e._l(e.post,(function(s){return t("tr",{key:s.id},[t("td",[e._v(e._s(s.storeName))]),t("td",[e._v(e._s(s.createdAt))]),t("td",[e._v(e._s(s.approved))]),t("td",[t("span",{staticClass:"material-symbols-outlined",on:{click:function(t){return e.edit(s.id)}}},[e._v(" edit ")]),t("span",{staticClass:"material-symbols-outlined",on:{click:function(t){return e.del(s.id)}}},[e._v(" delete ")]),s.approved?e._e():t("span",{staticClass:"material-symbols-outlined",on:{click:function(t){return e.approve(s.id)}}},[e._v(" thumb_up ")]),t("span",{staticClass:"material-symbols-outlined",on:{click:function(t){return e.reject(s.id)}}},[e._v(" thumb_down ")])])])})),0)]),e.pagenumber>0?t("a",{staticClass:"skipnext",on:{click:function(t){return e.prev()}}},[t("span",{staticClass:"material-symbols-outlined"},[e._v(" skip_previous ")])]):e._e(),e.count==e.paging?t("a",{staticClass:"skipnext",on:{click:function(t){return e.next()}}},[t("span",{staticClass:"material-symbols-outlined"},[e._v(" skip_next ")])]):e._e()]):e._e()])},k=[function(){var e=this,t=e._self._c;return t("thead",[t("tr",[t("th",[e._v("Name")]),t("th",[e._v("Created At")]),t("th",[e._v("Approved?")]),t("th",[e._v("Actions")])])])}],P=s("1315"),C=r["default"].extend({data(){return{loading:!1,post:null,pagenumber:0,count:0,paging:20}},created(){this.fetchData()},watch:{$route:"fetchData"},methods:{async fetchData(){this.post=null,this.loading=!0;var e=this.pagenumber*this.paging;await d.getData(`vendor/search?skip=${e}&take=${this.paging}`).then(e=>{this.post=e,this.count=this.post.length,this.post=this.post.map(e=>(e.createdAt=P["DateTime"].fromISO(e.createdAt+"Z").toLocaleString(P["DateTime"].DATETIME_SHORT),e)),this.loading=!1})},edit(e){window.location="/#/vendor-registration?id="+e},async del(e){confirm("Are you sure?")&&(await d.postData("/vendor/delete?id="+e),await this.fetchData())},async approve(e){await d.postData("/vendor/approve",{id:e,approved:!0}),await this.fetchData()},async reject(e){var t=prompt("What reason would you like to give them for your rejection?");await d.postData("/vendor/approve",{id:e,approved:!1,denialReason:t}),await this.fetchData()},async next(){this.pagenumber++,this.fetchData()},async prev(){this.pagenumber--,this.fetchData()}}}),A=C,I=Object(p["a"])(A,x,k,!1,null,null,null),T=I.exports,U=function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[""==e.vendor.storeName?t("h1",[e._v("Register as a Vendor")]):e._e(),""!==e.vendor.storeName?t("h1",[e._v("Edit Vendor Information")]):e._e(),t("div",{staticClass:"form-holder"},[e._m(0),t("label",[t("input",{directives:[{name:"model",rawName:"v-model",value:e.vendor.storeName,expression:"vendor.storeName"}],attrs:{type:"text",placeholder:"Store Name"},domProps:{value:e.vendor.storeName},on:{input:function(t){t.target.composing||e.$set(e.vendor,"storeName",t.target.value)}}})]),t("br"),t("label",[t("GooglePlacesInput",{attrs:{address:e.vendor.address},on:{placeChange:function(t){e.vendor.lat=t.lat,e.vendor.lng=t.lng,e.vendor.address=t.address}}}),t("span",{staticClass:"location"},[e._v("("+e._s(e.vendor.lat)+", "+e._s(e.vendor.lng)+")")])],1),e._v(" "),t("br"),t("label",[t("input",{directives:[{name:"model",rawName:"v-model",value:e.vendor.storeUrl,expression:"vendor.storeUrl"}],attrs:{type:"text",placeholder:"Store Url"},domProps:{value:e.vendor.storeUrl},on:{input:function(t){t.target.composing||e.$set(e.vendor,"storeUrl",t.target.value)}}})]),e._v(" "),t("br"),t("label",[t("input",{directives:[{name:"model",rawName:"v-model",value:e.vendor.publicEmail,expression:"vendor.publicEmail"}],attrs:{type:"text",placeholder:"Public Email"},domProps:{value:e.vendor.publicEmail},on:{input:function(t){t.target.composing||e.$set(e.vendor,"publicEmail",t.target.value)}}})]),e._v(" "),t("br"),t("label",[t("VuePhoneNumberInput",{attrs:{placeholder:"Public Phone","only-countries":e.countries},model:{value:e.vendor.publicPhone,callback:function(t){e.$set(e.vendor,"publicPhone",t)},expression:"vendor.publicPhone"}})],1),t("label",[t("input",{directives:[{name:"model",rawName:"v-model",value:e.plantListingUrl,expression:"plantListingUrl"}],attrs:{type:"text",placeholder:"Plant Listing Url"},domProps:{value:e.plantListingUrl},on:{input:function(t){t.target.composing||(e.plantListingUrl=t.target.value)}}})]),t("a",{on:{click:e.addUrl}},[t("span",{staticClass:"material-symbols-outlined"},[e._v(" add_box ")])]),t("br"),e.error?t("div",{staticClass:"error-holder"},[e._v(" "+e._s(e.error)+" "),t("span",{staticClass:"material-symbols-outlined cancel",on:{click:e.closeError}},[e._v(" cancel ")])]):e._e(),e._l(e.vendor.plantListingUrls,(function(s,r){return t("div",{key:r,staticClass:"urls"},[e._v(" "+e._s(s)+" "),t("span",{staticClass:"material-symbols-outlined",on:{click:function(t){return e.removeUrl(s)}}},[e._v(" disabled_by_default ")])])})),e.errors.length?t("div",{staticClass:"error-box"},[t("ul",{staticClass:"errors"},e._l(e.errors,(function(s){return t("li",{key:s},[e._v(" "+e._s(s))])})),0)]):e._e(),e.vendor.id?e._e():t("label",{staticClass:"tos"},[t("input",{directives:[{name:"model",rawName:"v-model",value:e.agreeToTerms,expression:"agreeToTerms"}],attrs:{type:"checkbox"},domProps:{checked:Array.isArray(e.agreeToTerms)?e._i(e.agreeToTerms,null)>-1:e.agreeToTerms},on:{change:function(t){var s=e.agreeToTerms,r=t.target,a=!!r.checked;if(Array.isArray(s)){var o=null,i=e._i(s,o);r.checked?i<0&&(e.agreeToTerms=s.concat([o])):i>-1&&(e.agreeToTerms=s.slice(0,i).concat(s.slice(i+1)))}else e.agreeToTerms=a}}}),e._v("I agree to the "),t("a",{attrs:{href:"#"}},[e._v("Terms of Service")])]),t("div",{attrs:{id:"plants"}},[e._v(" Plants Detected ["+e._s(e.plants.length)+"] "),t("ul",e._l(e.plants,(function(s){return t("li",{key:s.id},[e._v(" "+e._s(s.commonName)+" ")])})),0)]),e.vendor.id?e._e():t("input",{staticClass:"primary-btn",attrs:{type:"button",value:"Submit for Approval"},on:{click:function(t){return e.submit()}}}),"Admin"==e.role?t("input",{staticClass:"primary-btn",attrs:{type:"button",disabled:e.crawlInProgress,value:"Crawl Site(s)"},on:{click:function(t){return e.crawl()}}}):e._e(),e.vendor.id?t("input",{staticClass:"primary-btn save",attrs:{type:"button",value:"Save"},on:{click:function(t){return e.submit()}}}):e._e()],2)])},E=[function(){var e=this,t=e._self._c;return t("p",[t("em",{staticClass:"info"},[e._v("May take up to 72 hours before your listing is approved or denied")])])}],S=s("7bec"),N=s.n(S),O=(s("4413"),function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[t("input",{directives:[{name:"model",rawName:"v-model",value:e.address,expression:"address"}],ref:"autocomplete",attrs:{placeholder:"Address",required:"",autocomplete:"off",type:"text"},domProps:{value:e.address},on:{input:function(t){t.target.composing||(e.address=t.target.value)}}})])}),D=[],L=r["default"].extend({props:["address"],data(){return{value:"",lng:0,lat:0}},mounted(){this.autocomplete=new window.google.maps.places.Autocomplete(this.$refs.autocomplete,{types:["geocode"]}),this.autocomplete.addListener("place_changed",()=>{let e=this.autocomplete.getPlace(),t=e.address_components;const s=`${t[0].short_name} ${t[1].short_name}, ${t[2].short_name}, ${t[4].short_name} ${t[6].short_name}`;this.lat=e.geometry.location.lat(),this.lng=e.geometry.location.lng(),console.log(`The user picked ${s} the coordinates ${this.lat}, ${this.lng}`),this.$emit("placeChange",{lat:this.lat,lng:this.lng,place:e,address:s})})},methods:{}}),$=L,z=(s("855b"),Object(p["a"])($,O,D,!1,null,"4c397de7",null)),j=z.exports,R=r["default"].extend({components:{VuePhoneNumberInput:N.a,GooglePlacesInput:j},data(){return{vendor:{approved:!1,storeName:"",address:"",storeUrl:"",publicEmail:"",publicPhone:"",plantListingUrls:[]},crawlInProgress:!1,lat:0,role:"",plants:[],plantListingUrl:"",countries:["US"],agreeToTerms:!1,error:"",errors:[]}},async created(){var e=window.location.hash.split("?"),t=e.length>1?e[1].split("=")[1]:null;this.role=localStorage.getItem("role"),"Admin"==this.role&&null!=t?await d.getData("/vendor/get?id="+t).then(e=>{console.log(e),this.vendor=e}):await d.getData("/vendor/current").then(e=>{console.log(e),this.vendor=e}),t&&await d.getData("/plant/FindByVendorInternal?vendorId="+t).then(e=>{console.log(e),this.plants=e})},methods:{async submit(){var e=await this.validate();if(e)if(console.log(this.vendor),void 0==this.vendor.id){var t=await d.postData("/vendor/create",this.vendor);t.success&&(window.location=t.redirectUrl)}else{var s=await d.postData("/vendor/update",this.vendor);console.log(s),s.success&&(window.location=s.redirectUrl)}},async crawl(){this.crawlInProgress=!0,await d.postData("/vendor/crawl?id="+this.vendor.id),this.crawlInProgress=!1,window.location="/#/vendors"},closeError(){this.error=""},async validate(){var e,t,s,r,a;if(console.log("validate",this.vendor),this.errors=[],0===(null===(e=this.vendor.storeName)||void 0===e?void 0:e.trim().length)?this.errors.push("Store Name is a required field"):/^[a-zA-Z 0-9]+$/.test(this.vendor.storeName)||this.errors.push("Store Name may only contain alpha numeric characters"),0===(null===(t=this.vendor.address)||void 0===t?void 0:t.trim().length)?this.errors.push("Address is a required field"):this.vendor.address.length>500&&this.errors.push("Address must be less than 500 characters"),0===(null===(s=this.vendor.storeUrl)||void 0===s?void 0:s.trim().length)?this.errors.push("Store Url is a required field"):this.vendor.storeUrl.length>500?this.errors.push("Store URL may not be longer than 500 characters"):/https?:\/\/(www\.)?[-a-zA-Z0-9@:%._+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_+.~#?&//=]*)/.test(this.vendor.storeUrl)||this.errors.push("Store URL is not a valid URL"),0===(null===(r=this.vendor.publicEmail)||void 0===r?void 0:r.trim().length)?this.errors.push("Public Email is a required field"):/(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])/.test(this.vendor.publicEmail)||this.errors.push("Public Email must be a valid email"),0===(null===(a=this.vendor.publicPhone)||void 0===a?void 0:a.trim().length)?this.errors.push("Public Phone is a required field"):/^\(\d{3}\) \d{3}-\d{4}$/.test(this.vendor.publicPhone)||this.errors.push("Public Phone must be a valid US number"),null==this.vendor.plantListingUrls||0===this.vendor.plantListingUrls.length)this.errors.push("There must be at least one Plant Listing URL.  Be sure to hit the add button");else for(var o of(console.log("listing urls breakpoint"),this.vendor.plantListingUrls)){console.log("Evaluating url: "+o);var i=await d.getData("/vendor/IsAllowed?Url="+encodeURIComponent(o));i.success?console.log("Allowed:"+o):this.errors.push(`Unfortunately due to robots.txt policy we cannot spider this url [${o}].  Please remove it`)}return this.agreeToTerms||this.vendor.id||this.errors.push("You must agree to the terms of service"),0===this.errors.length},addUrl(){if(this.error="",this.plantListingUrl){var e=/((([A-Za-z]{3,9}:(?:\/\/)?)(?:[-;:&=+$,\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=+$,\w]+@)[A-Za-z0-9.-]+)((?:\/[+~%/.\w-_]*)?\??(?:[-+=&;%@.\w_]*)#?(?:[\w]*))?)/.test(this.plantListingUrl);if(e){var t=this.vendor.plantListingUrls.filter(e=>e==this.plantListingUrl);t.length>0?this.error="Cannot enter a duplicate url.":(this.vendor.plantListingUrls.push(this.plantListingUrl),this.plantListingUrl="")}else this.error="Invalid url.  Please try again."}else this.error="Must enter url before adding!"},removeUrl(e){confirm("Are you sure you want to remove this url? "+e)&&(this.vendor.plantListingUrls=this.vendor.plantListingUrls.filter(t=>t!=e))}}}),V=R,K=(s("9f73"),Object(p["a"])(V,U,E,!1,null,"44cdb15d",null)),q=K.exports,M=function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[t("h1",[e._v("API Key")]),t("div",{attrs:{id:"api-holder"}},[t("input",{directives:[{name:"model",rawName:"v-model",value:e.apiKey,expression:"apiKey"}],attrs:{type:"text",placeholder:"API Key"},domProps:{value:e.apiKey},on:{input:function(t){t.target.composing||(e.apiKey=t.target.value)}}}),e.apiKey?t("span",{staticClass:"material-symbols-outlined",on:{click:e.clipboard}},[e._v(" content_copy ")]):e._e(),t("span",{staticClass:"material-symbols-outlined",on:{click:e.genKey}},[e._v(" recycling ")]),t("br"),e._v(" "+e._s(e.message)+" "),e.error?t("div",{staticClass:"error-holder"},[t("span",{domProps:{innerHTML:e._s(e.error)}}),t("span",{staticClass:"material-symbols-outlined cancel",on:{click:e.closeError}},[e._v(" cancel ")])]):e._e(),t("a",{attrs:{id:"api-link",target:"_blank",href:"/swagger/index.html"}},[e._v(" View API Documentation")])])])},Z=[],G=r["default"].extend({data(){return{apiKey:"",message:"",error:""}},async created(){await this.getKey()},methods:{async getKey(){await d.getData("/user/GetApiKey").then(e=>{console.log(e),this.apiKey=e.id})},async genKey(){if(this.message="",confirm("Are you sure you want to regenerate an api key?  This will invalidate prior keys.")){var e=await d.postData("/user/GenApi");e.success?this.apiKey=e.value:this.error=e.message}},clipboard(){this.message="",this.apiKey?(navigator.clipboard.writeText(this.apiKey),this.message="copied!"):this.error="Must generate a key first!"},closeError(){this.error=""}}}),F=G,B=(s("396b"),Object(p["a"])(F,M,Z,!1,null,"4d1b3c7f",null)),H=B.exports,J=function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[t("h1",[e._v("Register as an API User")]),t("div",{staticClass:"form-holder"},[t("label",[t("input",{directives:[{name:"model",rawName:"v-model",value:e.user.name,expression:"user.name"}],attrs:{type:"text",placeholder:"Name"},domProps:{value:e.user.name},on:{input:function(t){t.target.composing||e.$set(e.user,"name",t.target.value)}}})]),t("br"),t("label",[t("input",{directives:[{name:"model",rawName:"v-model",value:e.user.organizationName,expression:"user.organizationName"}],attrs:{type:"text",placeholder:"Organization Name"},domProps:{value:e.user.organizationName},on:{input:function(t){t.target.composing||e.$set(e.user,"organizationName",t.target.value)}}})]),e._v(" "),t("br"),t("label",[t("VuePhoneNumberInput",{attrs:{placeholder:"Phone","only-countries":e.countries},model:{value:e.user.phone,callback:function(t){e.$set(e.user,"phone",t)},expression:"user.phone"}})],1),t("label",[t("GooglePlacesInput",{attrs:{address:e.user.address},on:{placeChange:function(t){e.user.lat=t.lat,e.user.lng=t.lng,e.user.address=t.address}}}),t("span",{staticClass:"location"},[e._v("("+e._s(e.user.lat)+", "+e._s(e.user.lng)+")")])],1),t("textarea",{directives:[{name:"model",rawName:"v-model",value:e.user.intendedUse,expression:"user.intendedUse"}],attrs:{placeholder:"Summary of Intended Use"},domProps:{value:e.user.intendedUse},on:{input:function(t){t.target.composing||e.$set(e.user,"intendedUse",t.target.value)}}}),e.error?t("div",{staticClass:"error-holder"},[e._v(" "+e._s(e.error)+" "),t("span",{staticClass:"material-symbols-outlined cancel",on:{click:e.closeError}},[e._v(" cancel ")])]):e._e(),e.errors.length?t("div",{staticClass:"error-box"},[t("ul",{staticClass:"errors"},e._l(e.errors,(function(s){return t("li",{key:s},[e._v(" "+e._s(s))])})),0)]):e._e(),t("label",{staticClass:"tos"},[t("input",{directives:[{name:"model",rawName:"v-model",value:e.agreeToTerms,expression:"agreeToTerms"}],attrs:{type:"checkbox"},domProps:{checked:Array.isArray(e.agreeToTerms)?e._i(e.agreeToTerms,null)>-1:e.agreeToTerms},on:{change:function(t){var s=e.agreeToTerms,r=t.target,a=!!r.checked;if(Array.isArray(s)){var o=null,i=e._i(s,o);r.checked?i<0&&(e.agreeToTerms=s.concat([o])):i>-1&&(e.agreeToTerms=s.slice(0,i).concat(s.slice(i+1)))}else e.agreeToTerms=a}}}),e._v("I agree to the "),t("a",{attrs:{href:"#"}},[e._v("Terms of Service")])]),t("input",{staticClass:"primary-btn",attrs:{type:"button",value:"Register"},on:{click:function(t){return e.submit()}}})])])},W=[],Y=r["default"].extend({components:{VuePhoneNumberInput:N.a,GooglePlacesInput:j},data(){return{user:{name:"",organizationName:"",phone:"",address:"",intendedUse:""},countries:["US"],agreeToTerms:!1,error:"",errors:[]}},created(){},methods:{async submit(){if(this.validate()){var e=await d.postData("/apiInfo/create",this.user);e.success&&(localStorage.setItem("flash","API application successful, Click recycle below, to get your key. Enjoy!"),window.location="#/api-key")}},closeError(){this.error=""},validate(){var e,t,s,r;return this.errors=[],0===(null===(e=this.user.name)||void 0===e?void 0:e.trim().length)&&this.errors.push("Name is a required field"),0===(null===(t=this.user.organizationName)||void 0===t?void 0:t.trim().length)&&this.errors.push("Organization Name is a required field"),0===(null===(s=this.user.phone)||void 0===s?void 0:s.trim().length)?this.errors.push("Phone is a required field"):/^\(\d{3}\) \d{3}-\d{4}$/.test(this.user.phone)||this.errors.push("Phone must be a valid US number"),0===(null===(r=this.user.address)||void 0===r?void 0:r.trim().length)&&this.errors.push("Address is a required field"),console.log("AgreeToTerms",this.agreeToTerms),this.agreeToTerms||this.errors.push("You must agree to the terms of service"),0===this.errors.length}}}),Q=Y,X=(s("b698"),Object(p["a"])(Q,J,W,!1,null,"6a3cfb8b",null)),ee=X.exports,te=function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[t("h1",[e._v("Reset Password")]),t("div",{attrs:{id:"api-holder"}},[t("div",{staticClass:"fields"},[t("input",{directives:[{name:"model",rawName:"v-model",value:e.user.password,expression:"user.password"}],attrs:{type:"password",placeholder:"Password"},domProps:{value:e.user.password},on:{keyup:e.validateIfErrors,input:function(t){t.target.composing||e.$set(e.user,"password",t.target.value)}}}),t("br")]),e.errors&&e.errors.length>0?t("div",{staticClass:"error-holder",attrs:{id:"error-holder"}},[t("span",{staticClass:"material-symbols-outlined cancel",on:{click:e.closeError}},[e._v(" cancel ")]),e._l(e.errors,(function(s){return t("div",{key:s},[e._v(" "+e._s(s)+" ")])}))],2):e._e()]),t("input",{staticClass:"primary-btn submit",attrs:{type:"button",value:"Submit"},on:{click:e.createUser}})])},se=[],re=r["default"].extend({data(){return{user:{id:"",password:""},shake:!0,message:"",errors:[]}},created(){var e=window.location.hash;console.log("Hash",e),e.indexOf("=")>0&&(this.user.id=e.split("=")[1])},methods:{closeError(){this.errors=[]},validateIfErrors(){this.errors.length&&this.validate()},validate(){if(this.errors=[],!this.user.password)return this.errors.push("Password is a required field"),!1;this.user.password.length<10&&this.errors.push("Password must be at least 10 characters"),/[a-z]/.test(this.user.password)||this.errors.push("Password must have a least one lowercase letter"),/[A-Z]/.test(this.user.password)||this.errors.push("Password must have at least one UPPERCASE letter"),/[0-9]/.test(this.user.password)||this.errors.push("Password must have at least one digit"),/[!@$#%^&*()]/.test(this.user.password)||this.errors.push("Password must contain at least one symbol");var e=0==this.errors.length;if(!e){var t=document.getElementById("error-holder");t.style.animationName="none",setTimeout(()=>{t.style.animationName=""},0)}return e},async createUser(){if(this.validate()){var e=await d.postData("/user/SetPassword",{inviteId:this.user.id,password:this.user.password});console.log("Result",e),e.success?window.location="/#/login":this.errors.push(e.message)}}}}),ae=re,oe=(s("9b70"),Object(p["a"])(ae,te,se,!1,null,"3c4c0987",null)),ie=oe.exports,ne=function(){var e=this;e._self._c;return e._m(0)},le=[function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[t("h1",[e._v("Thank you for Registering")]),t("p",[e._v("Please check your email for your confirmation link.")])])}],de=r["default"].extend({data(){return{}}}),ce=de,ue=Object(p["a"])(ce,ne,le,!1,null,"9d4522c4",null),he=ue.exports,pe=function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[t("h1",[e._v("Login")]),t("div",{attrs:{id:"api-holder"}},[t("input",{directives:[{name:"model",rawName:"v-model",value:e.user.email,expression:"user.email"}],attrs:{type:"text",placeholder:"Email"},domProps:{value:e.user.email},on:{keyup:function(t){return!t.type.indexOf("key")&&e._k(t.keyCode,"enter",13,t.key,"Enter")?null:e.login.apply(null,arguments)},input:function(t){t.target.composing||e.$set(e.user,"email",t.target.value)}}}),t("input",{directives:[{name:"model",rawName:"v-model",value:e.user.password,expression:"user.password"}],attrs:{type:"password",placeholder:"Password"},domProps:{value:e.user.password},on:{keyup:function(t){return!t.type.indexOf("key")&&e._k(t.keyCode,"enter",13,t.key,"Enter")?null:e.login.apply(null,arguments)},input:function(t){t.target.composing||e.$set(e.user,"password",t.target.value)}}}),t("br"),e.error?t("div",{staticClass:"error-holder"},[e._v(" "+e._s(e.error)+" "),t("span",{staticClass:"material-symbols-outlined cancel",on:{click:e.closeError}},[e._v(" cancel ")])]):e._e(),t("a",{staticClass:"forgot-password",on:{click:e.passwordReset}},[e._v("Forgot Password?")]),t("br"),t("input",{staticClass:"primary-btn",attrs:{type:"submit",value:"Login"},on:{click:e.login}})])])},me=[];const ve=new r["default"];var ge=r["default"].extend({data(){return{user:{email:"",password:""},error:""}},created(){this.user.email=localStorage.getItem("email")},methods:{async passwordReset(){this.user.email?(await d.postData("/user/ForgotPassword?email="+encodeURIComponent(this.user.email)),alert("If your email is found then you will receive a password reset request")):alert("Enter your email before clicking this link")},async login(){var e=window.location.hash,t=e.split("?")[1];if(t)var s=t.split("=")[1];this.error="";var r=await d.postData("/user/login",this.user);if(r.success)if(console.log(r),r.verified){if(localStorage.setItem("role",r.role),localStorage.setItem("email",r.email),ve.$emit("userLoggedIn"),s)return void(window.location=decodeURIComponent(s));"Admin"==r.role?window.location="/#/vendors":window.location="/#/"}else this.error="Please check your email for the verification email before your first login";else this.error="Authentication attempt failed"},closeError(){this.error=""}}}),fe=ge,ye=(s("205b"),Object(p["a"])(fe,pe,me,!1,null,"03475557",null)),_e=ye.exports,be=function(){var e=this;e._self._c;return e._m(0)},we=[function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[t("h1",[e._v("Not Found")])])}],xe=r["default"].extend({data(){return{}}}),ke=xe,Pe=Object(p["a"])(ke,be,we,!1,null,"439dd121",null),Ce=Pe.exports,Ae=function(){var e=this,t=e._self._c;return t("div",{staticClass:"holder"},[t("h1",[e._v("Welcome")]),t("a",{staticClass:"register-vendor",attrs:{href:"/#/vendor-registration"}},["User"==e.role?t("span",[e._v("Register")]):e._e(),"Vendor"==e.role||"Admin"==e.role?t("span",[e._v("Edit")]):e._e(),e._v(" Vendor Listing "),t("span",{staticClass:"material-symbols-outlined"},[e._v(" arrow_circle_right ")])]),e._m(0)])},Ie=[function(){var e=this,t=e._self._c;return t("a",{staticClass:"get-api",attrs:{href:"/#/api-key"}},[e._v("Get API"),t("span",{staticClass:"material-symbols-outlined"},[e._v(" arrow_circle_right ")])])}],Te=r["default"].extend({data(){return{role:""}},created(){this.role=localStorage.getItem("role")}}),Ue=Te,Ee=(s("16c9"),Object(p["a"])(Ue,Ae,Ie,!1,null,"302457d5",null)),Se=Ee.exports;const Ne={"/":Se,"/users":v,"/user-create":w,"/vendors":T,"/vendor-registration":q,"/api-key":H,"/api-registration":ee,"/login":_e,"/forgot-password":ie,"/thank-you":he,"/places":j},Oe=Object(r["ref"])(window.location.hash);function De(){var e=localStorage.getItem("flash"),t=document.getElementById("flash-message");"undefined"!=e&&void 0!==e?(t.innerHTML=e,localStorage.setItem("flash",void 0),t.style.display="block"):(t.innerHTML="",t.style.display="none")}window.addEventListener("hashchange",()=>{Oe.value=window.location.hash,setTimeout(De,200)});const Le=Object(r["computed"])(()=>{var e=Oe.value.split("?")[0].slice(1);return Ne[e||"/"]||Ce}),$e=[{uri:"#/api-registration",text:"Register API",role:"Vendor"},{uri:"#/api-registration",text:"Register API",role:"User"},{uri:"#/vendor-registration",text:"Register as Vendor",role:"User"},{uri:"#/vendor-registration",text:"Edit Vendor Listing",role:"Vendor"},{uri:"#/users",text:"Users",role:"Admin"},{uri:"#/vendors",text:"Vendors",role:"Admin"},{uri:"#/login",text:"Log in",role:"all",hideIfAuth:!0}];var ze=r["default"].extend({name:"App",data(){return{loggedIn:!1,currentView:Le,links:$e}},created(){this.evaluateLinks(),ve.$on("userLoggedIn",()=>{localStorage.setItem("loggedIn",!0),localStorage.setItem("flash","Login successful"),this.evaluateLinks()})},methods:{async logout(){await d.getData("user/logout"),console.log("Logging out"),this.loggedIn=!1,localStorage.setItem("email",""),localStorage.setItem("role",""),localStorage.setItem("loggedIn",!1),window.location="/#/login",this.evaluateLinks()},evaluateLinks(){this.links=$e,this.email=localStorage.getItem("email"),this.role=localStorage.getItem("role"),this.loggedIn=localStorage.getItem("loggedIn");var e=this.links.filter(e=>"all"==e.role),t=this.links.filter(e=>e.role==this.role&&this.loggedIn);this.links=[...e,...t],this.email&&(this.links=this.links.filter(e=>!e.hideIfAuth))}},components:{Users:v,Vendors:T,VendorRegistration:q}}),je=ze,Re=(s("1932"),Object(p["a"])(je,a,o,!1,null,null,null)),Ve=Re.exports;r["default"].config.productionTip=!1,new r["default"]({render:e=>e(Ve)}).$mount("#app")},"5afa":function(e,t,s){},7102:function(e,t,s){},7311:function(e,t,s){"use strict";s("ff4d")},"855b":function(e,t,s){"use strict";s("d46c")},"8b59":function(e,t,s){"use strict";s("0d25")},"962d":function(e,t,s){},"9b70":function(e,t,s){"use strict";s("7102")},"9f73":function(e,t,s){"use strict";s("3a66")},b606:function(e,t,s){},b698:function(e,t,s){"use strict";s("5afa")},d46c:function(e,t,s){},ff4d:function(e,t,s){}});
//# sourceMappingURL=app.68f8cab5.js.map