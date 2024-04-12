(function(e){function t(t){for(var s,o,n=t[0],l=t[1],d=t[2],c=0,p=[];c<n.length;c++)o=n[c],Object.prototype.hasOwnProperty.call(a,o)&&a[o]&&p.push(a[o][0]),a[o]=0;for(s in l)Object.prototype.hasOwnProperty.call(l,s)&&(e[s]=l[s]);u&&u(t);while(p.length)p.shift()();return i.push.apply(i,d||[]),r()}function r(){for(var e,t=0;t<i.length;t++){for(var r=i[t],s=!0,n=1;n<r.length;n++){var l=r[n];0!==a[l]&&(s=!1)}s&&(i.splice(t--,1),e=o(o.s=r[0]))}return e}var s={},a={app:0},i=[];function o(t){if(s[t])return s[t].exports;var r=s[t]={i:t,l:!1,exports:{}};return e[t].call(r.exports,r,r.exports,o),r.l=!0,r.exports}o.m=e,o.c=s,o.d=function(e,t,r){o.o(e,t)||Object.defineProperty(e,t,{enumerable:!0,get:r})},o.r=function(e){"undefined"!==typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(e,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(e,"__esModule",{value:!0})},o.t=function(e,t){if(1&t&&(e=o(e)),8&t)return e;if(4&t&&"object"===typeof e&&e&&e.__esModule)return e;var r=Object.create(null);if(o.r(r),Object.defineProperty(r,"default",{enumerable:!0,value:e}),2&t&&"string"!=typeof e)for(var s in e)o.d(r,s,function(t){return e[t]}.bind(null,s));return r},o.n=function(e){var t=e&&e.__esModule?function(){return e["default"]}:function(){return e};return o.d(t,"a",t),t},o.o=function(e,t){return Object.prototype.hasOwnProperty.call(e,t)},o.p="/";var n=window["webpackJsonp"]=window["webpackJsonp"]||[],l=n.push.bind(n);n.push=t,n=n.slice();for(var d=0;d<n.length;d++)t(n[d]);var u=l;i.push([0,"chunk-vendors"]),r()})({0:function(e,t,r){e.exports=r("56d7")},"0d9b":function(e,t,r){},"16c9":function(e,t,r){"use strict";r("b606")},"21db":function(e,t,r){"use strict";r("fd0d")},"2e10":function(e,t,r){},"38ac":function(e,t,r){},3948:function(e,t,r){},4881:function(e,t,r){"use strict";r("0d9b")},"56d7":function(e,t,r){"use strict";r.r(t);var s=r("2b0e"),a=function(){var e=this,t=e._self._c;e._self._setupProxy;return t("div",{attrs:{id:"app"}},[t("div",{attrs:{id:"nav"}},[e._m(0),t("ul",e._l(e.links,(function(r){return t("li",{key:r.uri},[t("a",{attrs:{href:r.uri}},[e._v(e._s(r.text))])])})),0)]),t("div",{attrs:{id:"top"}},[e.loggedIn&&"Vendor"==e.role?t("a",{attrs:{href:"#/vendor-registration"}},[t("span",{staticClass:"material-symbols-outlined"},[e._v(" account_circle ")]),e._v("Account")]):e._e(),e.loggedIn?t("a",{attrs:{href:"#"},on:{click:e.logout}},[t("span",{staticClass:"material-symbols-outlined"},[e._v(" logout ")]),e._v("Logout")]):e._e(),e.loggedIn?e._e():t("a",{attrs:{href:"#/login"}},[t("span",{staticClass:"material-symbols-outlined"},[e._v(" login ")]),e._v("Login")])]),t("div",{attrs:{id:"body"}},[t(e.currentView,{tag:"component"})],1)])},i=[function(){var e=this,t=e._self._c;e._self._setupProxy;return t("a",{attrs:{href:"#/"}},[t("img",{attrs:{src:"/pac.svg",alt:"Plant Agents Collective",width:"100%"}})])}],o=function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[e.loading?t("div",{staticClass:"loading"},[e._v(" Loading... ")]):e._e(),t("h1",[e._v("Users")]),e.post?t("div",{staticClass:"content"},[t("label",{staticClass:"show-admin"},[t("input",{directives:[{name:"model",rawName:"v-model",value:e.showAdminOnly,expression:"showAdminOnly"}],attrs:{type:"checkbox"},domProps:{checked:Array.isArray(e.showAdminOnly)?e._i(e.showAdminOnly,null)>-1:e.showAdminOnly},on:{change:function(t){var r=e.showAdminOnly,s=t.target,a=!!s.checked;if(Array.isArray(r)){var i=null,o=e._i(r,i);s.checked?o<0&&(e.showAdminOnly=r.concat([i])):o>-1&&(e.showAdminOnly=r.slice(0,o).concat(r.slice(o+1)))}else e.showAdminOnly=a}}}),e._v("Show Admin Only?")]),t("a",{staticClass:"export-csv",on:{click:e.exportCSV}},[e._v("Export as CSV")]),t("table",{staticClass:"grid"},[e._m(0),t("tbody",e._l(e.post,(function(r){return t("tr",{key:r.id},[t("td",[e._v(e._s(r.email))]),t("td",[e._v(e._s(r.role))]),t("td",[e._v(e._s(r.verified))]),t("td",[t("span",{staticClass:"material-symbols-outlined",attrs:{title:"Promote to Admin"},on:{click:function(t){return e.promote(r.id)}}},[e._v(" admin_panel_settings ")])]),t("td",[t("span",{staticClass:"material-symbols-outlined",on:{click:function(t){return e.del(r.id)}}},[e._v(" delete ")])]),t("td",[t("a",{staticClass:"resend",on:{click:function(t){return e.resend(r.id)}}},[e._v("Resend")])])])})),0)]),e.pagenumber>0?t("a",{on:{click:function(t){return e.prev()}}},[e._v("Prev")]):e._e(),e.count==e.paging?t("a",{on:{click:function(t){return e.next()}}},[e._v("Next")]):e._e()]):e._e()])},n=[function(){var e=this,t=e._self._c;return t("thead",[t("tr",[t("th",[e._v("Email")]),t("th",[e._v("Role")]),t("th",[e._v("Verified")]),t("th",{attrs:{colspan:"3"}},[e._v("Actions")])])])}];r("14d9");async function l(e,t={},r="GET"){return await fetch(e,{method:r,mode:"cors",cache:"no-cache",credentials:"same-origin",headers:{"Content-Type":"application/json"},redirect:"manual",referrerPolicy:"no-referrer",body:"GET"==r?void 0:JSON.stringify(t)}).then(e=>0==e.status&&e.url?(window.location="/#/login",{}):e.json())}var d={async getData(e){return l(e)},async putData(e,t={}){return l(e,t,"PUT")},async postData(e,t={}){return l(e,t,"POST")},async delData(e){return l(e,"DELETE")}};function u(e,t){var r=document.createElement("a");r.setAttribute("href","data:text/plain;charset=utf-8,"+encodeURIComponent(t)),r.setAttribute("download",e),r.style.display="none",document.body.appendChild(r),r.click(),document.body.removeChild(r)}var c=s["default"].extend({data(){return{loading:!1,showAdminOnly:!1,post:null,pagenumber:0,count:0,paging:20}},created(){this.fetchData()},watch:{$route:"fetchData",showAdminOnly:"fetchData"},methods:{async fetchData(){this.post=null,this.loading=!0;var e=this.pagenumber*this.paging;await d.getData(`user/search?showAdminOnly=${this.showAdminOnly}&skip=${e}&take=${this.paging}`,{redirect:"manual"}).then(e=>{this.post=e,this.count=this.post.length,this.loading=!1})},async exportCSV(){this.loading=!0,await d.getData("user/export?showAdminOnly="+this.showAdminOnly).then(e=>{this.post=e;var t=["Id,Email,Role, Verified, CreatedAt, ModifiedAt"];this.post.forEach((function(e){var r=e.join(",");t.push(r)}));var r=t.join("\n");u("users.csv",r),this.count=this.post.length,this.loading=!1})},async promote(e){confirm("Are you sure you want to promote this user to Admin?")&&await d.postData("/user/promote?id="+e),this.fetchData()},async del(e){confirm("Are you sure?")&&(await d.postData("/user/delete?id="+e),await this.fetchData())},async resend(e){await d.postData("/user/resend?id="+e),alert("Verification email has been resent")},next(){this.pagenumber++,this.fetchData()},prev(){this.pagenumber--,this.fetchData()}}}),p=c,h=(r("21db"),r("2877")),m=Object(h["a"])(p,o,n,!1,null,"5be34e56",null),v=m.exports,g=function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[t("h1",[e._v("Create User")]),t("div",{attrs:{id:"api-holder"}},[t("div",{staticClass:"fields"},[t("input",{directives:[{name:"model",rawName:"v-model",value:e.user.email,expression:"user.email"}],attrs:{type:"text",placeholder:"Email"},domProps:{value:e.user.email},on:{keyup:e.validateIfErrors,input:function(t){t.target.composing||e.$set(e.user,"email",t.target.value)}}}),t("br"),t("input",{directives:[{name:"model",rawName:"v-model",value:e.user.password,expression:"user.password"}],attrs:{type:"password",placeholder:"Password"},domProps:{value:e.user.password},on:{keyup:e.validateIfErrors,input:function(t){t.target.composing||e.$set(e.user,"password",t.target.value)}}}),t("br")]),e.errors&&e.errors.length>0?t("div",{staticClass:"error-holder",attrs:{id:"error-holder"}},[t("span",{staticClass:"material-symbols-outlined cancel",on:{click:e.closeError}},[e._v(" cancel ")]),e._l(e.errors,(function(r){return t("div",{key:r},[e._v(" "+e._s(r)+" ")])}))],2):e._e()]),t("input",{staticClass:"primary-btn submit",attrs:{type:"button",value:"Submit"},on:{click:e.createUser}})])},f=[],_=s["default"].extend({data(){return{user:{email:"",password:""},shake:!0,message:"",errors:[]}},methods:{closeError(){this.errors=[]},validateIfErrors(){this.errors.length&&this.validate()},validate(){if(this.errors=[],this.user.email||this.errors.push("Email is a required field"),!this.user.password)return this.errors.push("Password is a required field"),!1;/(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])/.test(this.user.email)||this.errors.push("Email must be valid"),this.user.password.length<10&&this.errors.push("Password must be at least 10 characters"),/[a-z]/.test(this.user.password)||this.errors.push("Password must have a least one lowercase letter"),/[A-Z]/.test(this.user.password)||this.errors.push("Password must have at least one UPPERCASE letter"),/[0-9]/.test(this.user.password)||this.errors.push("Password must have at least one digit"),/[!@$#%^&*()]/.test(this.user.password)||this.errors.push("Password must contain at least one symbol");var e=0==this.errors.length;if(!e){var t=document.getElementById("error-holder");t.style.animationName="none",setTimeout(()=>{t.style.animationName=""},0)}return e},async createUser(){if(this.validate()){var e=await d.postData("/user/create",this.user);console.log("Result",e),e.success?window.location="/#/login":this.errors.push(e.message)}}}}),y=_,b=(r("a63a"),Object(h["a"])(y,g,f,!1,null,"d3ae33b4",null)),x=b.exports,w=function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[e.loading?t("div",{staticClass:"loading"},[e._v(" Loading... ")]):e._e(),t("h1",[e._v("Vendors")]),e.post?t("div",{staticClass:"content"},[t("table",{staticClass:"grid"},[e._m(0),t("tbody",e._l(e.post,(function(r){return t("tr",{key:r.id},[t("td",[e._v(e._s(r.storeName))]),t("td",[e._v(e._s(r.createdAt))]),t("td",[e._v(e._s(r.approved))]),t("td",[t("span",{staticClass:"material-symbols-outlined",on:{click:function(t){return e.edit(r.id)}}},[e._v(" edit ")]),t("span",{staticClass:"material-symbols-outlined",on:{click:function(t){return e.del(r.id)}}},[e._v(" delete ")]),r.approved?e._e():t("span",{staticClass:"material-symbols-outlined",on:{click:function(t){return e.approve(r.id)}}},[e._v(" thumb_up ")]),t("span",{staticClass:"material-symbols-outlined",on:{click:function(t){return e.reject(r.id)}}},[e._v(" thumb_down ")])])])})),0)]),e.pagenumber>0?t("a",{staticClass:"skipnext",on:{click:function(t){return e.prev()}}},[t("span",{staticClass:"material-symbols-outlined"},[e._v(" skip_previous ")])]):e._e(),e.count==e.paging?t("a",{staticClass:"skipnext",on:{click:function(t){return e.next()}}},[t("span",{staticClass:"material-symbols-outlined"},[e._v(" skip_next ")])]):e._e()]):e._e()])},k=[function(){var e=this,t=e._self._c;return t("thead",[t("tr",[t("th",[e._v("Name")]),t("th",[e._v("Created At")]),t("th",[e._v("Approved?")]),t("th",[e._v("Actions")])])])}],A=r("1315"),C=s["default"].extend({data(){return{loading:!1,post:null,pagenumber:0,count:0,paging:20}},created(){this.fetchData()},watch:{$route:"fetchData"},methods:{async fetchData(){this.post=null,this.loading=!0;var e=this.pagenumber*this.paging;await d.getData(`vendor/search?skip=${e}&take=${this.paging}`).then(e=>{this.post=e,this.count=this.post.length,this.post=this.post.map(e=>(e.createdAt=A["DateTime"].fromISO(e.createdAt).toLocaleString(A["DateTime"].DATETIME_SHORT),e)),this.loading=!1})},edit(e){window.location="/#/vendor-registration?id="+e},async del(e){confirm("Are you sure?")&&(await this.postData("/vendor/delete?id="+e),await this.fetchData())},async approve(e){await d.postData("/vendor/approve",{id:e,approved:!0}),await this.fetchData()},async reject(e){var t=prompt("What reason would you like to give them for your rejection?");await this.postData("/vendor/approve",{id:e,approved:!1,denialReason:t}),await this.fetchData()},next(){this.pagenumber++,this.fetchData()},prev(){this.pagenumber--,this.fetchData()}}}),P=C,T=Object(h["a"])(P,w,k,!1,null,null,null),U=T.exports,N=function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[""==e.vendor.storeName?t("h1",[e._v("Register as a Vendor")]):e._e(),""!==e.vendor.storeName?t("h1",[e._v("Edit Vendor Information")]):e._e(),t("div",{staticClass:"form-holder"},[e._m(0),t("label",[t("input",{directives:[{name:"model",rawName:"v-model",value:e.vendor.storeName,expression:"vendor.storeName"}],attrs:{type:"text",placeholder:"Store Name"},domProps:{value:e.vendor.storeName},on:{input:function(t){t.target.composing||e.$set(e.vendor,"storeName",t.target.value)}}})]),t("br"),t("label",[t("input",{directives:[{name:"model",rawName:"v-model",value:e.vendor.address,expression:"vendor.address"}],attrs:{type:"text",placeholder:"Address"},domProps:{value:e.vendor.address},on:{input:function(t){t.target.composing||e.$set(e.vendor,"address",t.target.value)}}})]),e._v(" "),t("br"),t("label",[t("input",{directives:[{name:"model",rawName:"v-model",value:e.vendor.storeUrl,expression:"vendor.storeUrl"}],attrs:{type:"text",placeholder:"Store Url"},domProps:{value:e.vendor.storeUrl},on:{input:function(t){t.target.composing||e.$set(e.vendor,"storeUrl",t.target.value)}}})]),e._v(" "),t("br"),t("label",[t("input",{directives:[{name:"model",rawName:"v-model",value:e.vendor.publicEmail,expression:"vendor.publicEmail"}],attrs:{type:"text",placeholder:"Public Email"},domProps:{value:e.vendor.publicEmail},on:{input:function(t){t.target.composing||e.$set(e.vendor,"publicEmail",t.target.value)}}})]),e._v(" "),t("br"),t("label",[t("VuePhoneNumberInput",{attrs:{placeholder:"Public Phone","only-countries":e.countries},model:{value:e.vendor.publicPhone,callback:function(t){e.$set(e.vendor,"publicPhone",t)},expression:"vendor.publicPhone"}})],1),t("label",[t("input",{directives:[{name:"model",rawName:"v-model",value:e.plantListingUrl,expression:"plantListingUrl"}],attrs:{type:"text",placeholder:"Plant Listing Url"},domProps:{value:e.plantListingUrl},on:{input:function(t){t.target.composing||(e.plantListingUrl=t.target.value)}}})]),t("a",{on:{click:e.addUrl}},[t("span",{staticClass:"material-symbols-outlined"},[e._v(" add_box ")])]),t("br"),e.error?t("div",{staticClass:"error-holder"},[e._v(" "+e._s(e.error)+" "),t("span",{staticClass:"material-symbols-outlined cancel",on:{click:e.closeError}},[e._v(" cancel ")])]):e._e(),e._l(e.vendor.plantListingUrls,(function(r,s){return t("div",{key:s,staticClass:"urls"},[e._v(" "+e._s(r)+" "),t("span",{staticClass:"material-symbols-outlined",on:{click:function(t){return e.removeUrl(r)}}},[e._v(" disabled_by_default ")])])})),e.errors.length?t("div",{staticClass:"error-box"},[t("ul",{staticClass:"errors"},e._l(e.errors,(function(r){return t("li",{key:r},[e._v(" "+e._s(r))])})),0)]):e._e(),e.vendor.id?e._e():t("label",{staticClass:"tos"},[t("input",{directives:[{name:"model",rawName:"v-model",value:e.agreeToTerms,expression:"agreeToTerms"}],attrs:{type:"checkbox"},domProps:{checked:Array.isArray(e.agreeToTerms)?e._i(e.agreeToTerms,null)>-1:e.agreeToTerms},on:{change:function(t){var r=e.agreeToTerms,s=t.target,a=!!s.checked;if(Array.isArray(r)){var i=null,o=e._i(r,i);s.checked?o<0&&(e.agreeToTerms=r.concat([i])):o>-1&&(e.agreeToTerms=r.slice(0,o).concat(r.slice(o+1)))}else e.agreeToTerms=a}}}),e._v("I agree to the "),t("a",{attrs:{href:"#"}},[e._v("Terms of Service")])]),t("div",{attrs:{id:"plants"}},[e._v(" Plants Detected ["+e._s(e.plants.length)+"] "),t("ul",e._l(e.plants,(function(r){return t("li",{key:r.id},[e._v(" "+e._s(r.commonName)+" ")])})),0)]),e.vendor.id?e._e():t("input",{staticClass:"primary-btn",attrs:{type:"button",value:"Submit for Approval"},on:{click:function(t){return e.submit()}}}),e.vendor.id?t("input",{staticClass:"primary-btn save",attrs:{type:"button",value:"Save"},on:{click:function(t){return e.submit()}}}):e._e()],2)])},I=[function(){var e=this,t=e._self._c;return t("p",[t("em",{staticClass:"info"},[e._v("May take up to 72 hours before your listing is approved or denied")])])}],E=r("7bec"),O=r.n(E),D=(r("4413"),s["default"].extend({components:{VuePhoneNumberInput:O.a},data(){return{vendor:{approved:!1,storeName:"",address:"",storeUrl:"",publicEmail:"",publicPhone:"",plantListingUrls:[]},plants:[],plantListingUrl:"",countries:["US"],agreeToTerms:!1,error:"",errors:[]}},async created(){var e=window.location.hash.split("?"),t=e.length>1?e[1].split("=")[1]:null,r=localStorage.getItem("role");"Admin"==r&&null!=t?await d.getData("/vendor/get?id="+t).then(e=>{console.log(e),this.vendor=e}):await d.getData("/vendor/current").then(e=>{console.log(e),this.vendor=e}),t&&await d.getData("/plant/FindByVendorInternal?vendorId="+t).then(e=>{console.log(e),this.plants=e})},methods:{async submit(){if(this.validate())if(console.log(this.vendor),void 0==this.vendor.id){var e=await d.postData("/vendor/create",this.vendor);e.success&&(window.location="/#/")}else{var t=await d.postData("/vendor/update",this.vendor);console.log(t),t.success&&(window.location="/#/")}},closeError(){this.error=""},validate(){var e,t,r,s,a;return this.errors=[],0===(null===(e=this.vendor.storeName)||void 0===e?void 0:e.trim().length)?this.errors.push("Store Name is a required field"):/^[a-zA-Z 0-9]+$/.test(this.vendor.storeName)||this.errors.push("Store Name may only contain alpha numeric characters"),0===(null===(t=this.vendor.address)||void 0===t?void 0:t.trim().length)?this.errors.push("Address is a required field"):this.vendor.address.length>500&&this.errors.push("Address must be less than 500 characters"),0===(null===(r=this.vendor.storeUrl)||void 0===r?void 0:r.trim().length)?this.errors.push("Store Url is a required field"):this.vendor.storeUrl.length>500?this.errors.push("Store URL may not be longer than 500 characters"):/https?:\/\/(www\.)?[-a-zA-Z0-9@:%._+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_+.~#?&//=]*)/.test(this.vendor.storeUrl)||this.errors.push("Store URL is not a valid URL"),0===(null===(s=this.vendor.publicEmail)||void 0===s?void 0:s.trim().length)?this.errors.push("Public Email is a required field"):/(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])/.test(this.vendor.publicEmail)||this.errors.push("Public Email must be a valid email"),0===(null===(a=this.vendor.publicPhone)||void 0===a?void 0:a.trim().length)&&this.errors.push("Public Phone is a required field"),0===this.vendor.plantListingUrls.length&&this.errors.push("There must be at least one Plant Listing URL.  Be sure to hit the add button"),this.agreeToTerms||this.vendor.id||this.errors.push("You must agree to the terms of service"),0===this.errors.length},addUrl(){if(this.error="",this.plantListingUrl){var e=/((([A-Za-z]{3,9}:(?:\/\/)?)(?:[-;:&=+$,\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=+$,\w]+@)[A-Za-z0-9.-]+)((?:\/[+~%/.\w-_]*)?\??(?:[-+=&;%@.\w_]*)#?(?:[\w]*))?)/.test(this.plantListingUrl);if(e){var t=this.vendor.plantListingUrls.filter(e=>e==this.plantListingUrl);t.length>0?this.error="Cannot enter a duplicate url.":(this.vendor.plantListingUrls.push(this.plantListingUrl),this.plantListingUrl="")}else this.error="Invalid url.  Please try again."}else this.error="Must enter url before adding!"},removeUrl(e){confirm("Are you sure you want to remove this url? "+e)&&(this.vendor.plantListingUrls=this.vendor.plantListingUrls.filter(t=>t!=e))}}})),S=D,L=(r("4881"),Object(h["a"])(S,N,I,!1,null,"e5a0e5ae",null)),z=L.exports,$=function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[t("h1",[e._v("API Key")]),t("div",{attrs:{id:"api-holder"}},[t("input",{directives:[{name:"model",rawName:"v-model",value:e.apiKey,expression:"apiKey"}],attrs:{type:"text",placeholder:"API Key"},domProps:{value:e.apiKey},on:{input:function(t){t.target.composing||(e.apiKey=t.target.value)}}}),t("span",{staticClass:"material-symbols-outlined",on:{click:e.clipboard}},[e._v(" content_copy ")]),t("span",{staticClass:"material-symbols-outlined",on:{click:e.genKey}},[e._v(" recycling ")]),t("br"),e._v(" "+e._s(e.message)+" "),e.error?t("div",{staticClass:"error-holder"},[e._v(" "+e._s(e.error)+" "),t("span",{staticClass:"material-symbols-outlined cancel",on:{click:e.closeError}},[e._v(" cancel ")])]):e._e(),t("a",{attrs:{id:"api-link",href:"/swagger/index.html"}},[e._v(" View API Documentation")])])])},j=[],V=s["default"].extend({data(){return{apiKey:"",message:"",error:""}},async created(){await this.getKey()},methods:{async getKey(){await d.getData("/user/GetApiKey").then(e=>{console.log(e),this.apiKey=e.id})},async genKey(){if(this.message="",confirm("Are you sure you want to regenerate an api key?  This will invalidate prior keys.")){var e=await d.postData("/user/GenApi");console.log(e),this.apiKey=e.value}},clipboard(){this.message="",this.apiKey?(navigator.clipboard.writeText(this.apiKey),this.message="copied!"):this.error="Must generate a key first!"},closeError(){this.error=""}}}),R=V,K=(r("e463"),Object(h["a"])(R,$,j,!1,null,"3ead7dd2",null)),q=K.exports,M=function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[t("h1",[e._v("Register as an Api User")]),t("div",{staticClass:"form-holder"},[e._m(0),t("label",[t("input",{directives:[{name:"model",rawName:"v-model",value:e.user.name,expression:"user.name"}],attrs:{type:"text",placeholder:"Name"},domProps:{value:e.user.name},on:{input:function(t){t.target.composing||e.$set(e.user,"name",t.target.value)}}})]),t("br"),t("label",[t("input",{directives:[{name:"model",rawName:"v-model",value:e.user.organizationName,expression:"user.organizationName"}],attrs:{type:"text",placeholder:"Organization Name"},domProps:{value:e.user.organizationName},on:{input:function(t){t.target.composing||e.$set(e.user,"organizationName",t.target.value)}}})]),e._v(" "),t("br"),t("label",[t("VuePhoneNumberInput",{attrs:{placeholder:"Phone","only-countries":e.countries},model:{value:e.user.phone,callback:function(t){e.$set(e.user,"phone",t)},expression:"user.phone"}})],1),t("label",[t("input",{directives:[{name:"model",rawName:"v-model",value:e.user.address,expression:"user.address"}],attrs:{type:"text",placeholder:"Address"},domProps:{value:e.user.address},on:{input:function(t){t.target.composing||e.$set(e.user,"address",t.target.value)}}})]),e._v(" "),t("br"),t("textarea",{directives:[{name:"model",rawName:"v-model",value:e.user.intendedUse,expression:"user.intendedUse"}],attrs:{placeholder:"Summary of Intended Use"},domProps:{value:e.user.intendedUse},on:{input:function(t){t.target.composing||e.$set(e.user,"intendedUse",t.target.value)}}}),e.error?t("div",{staticClass:"error-holder"},[e._v(" "+e._s(e.error)+" "),t("span",{staticClass:"material-symbols-outlined cancel",on:{click:e.closeError}},[e._v(" cancel ")])]):e._e(),e.errors.length?t("div",{staticClass:"error-box"},[t("ul",{staticClass:"errors"},e._l(e.errors,(function(r){return t("li",{key:r},[e._v(" "+e._s(r))])})),0)]):e._e(),t("span",{staticClass:"tos"},[t("input",{directives:[{name:"model",rawName:"v-model",value:e.agreeToTerms,expression:"agreeToTerms"}],attrs:{type:"checkbox"},domProps:{checked:Array.isArray(e.agreeToTerms)?e._i(e.agreeToTerms,null)>-1:e.agreeToTerms},on:{change:function(t){var r=e.agreeToTerms,s=t.target,a=!!s.checked;if(Array.isArray(r)){var i=null,o=e._i(r,i);s.checked?o<0&&(e.agreeToTerms=r.concat([i])):o>-1&&(e.agreeToTerms=r.slice(0,o).concat(r.slice(o+1)))}else e.agreeToTerms=a}}}),e._v("I agree to the "),t("a",{attrs:{href:"#"}},[e._v("Terms of Service")])]),t("input",{staticClass:"primary-btn",attrs:{type:"button",value:"Submit for Approval"},on:{click:function(t){return e.submit()}}})])])},Z=[function(){var e=this,t=e._self._c;return t("p",[t("em",{staticClass:"info"},[e._v("May take up to 72 hours before your listing is approved or denied")])])}],G=s["default"].extend({components:{VuePhoneNumberInput:O.a},data(){return{user:{name:"",organizationName:"",phone:"",address:"",intendedUse:""},countries:["US"],agreeToTerms:!1,error:"",errors:[]}},created(){},methods:{async submit(){this.validate()&&await d.postData("/vendor/create",this.vendor)},closeError(){this.error=""},validate(){var e,t,r,s;return this.errors=[],0===(null===(e=this.user.name)||void 0===e?void 0:e.trim().length)&&this.errors.push("Name is a required field"),0===(null===(t=this.user.organizationName)||void 0===t?void 0:t.trim().length)&&this.errors.push("Organization Name is a required field"),0===(null===(r=this.user.phone)||void 0===r?void 0:r.trim().length)&&this.errors.push("Phone is a required field"),0===(null===(s=this.user.address)||void 0===s?void 0:s.trim().length)&&this.errors.push("Address is a required field"),console.log("AgreeToTerms",this.agreeToTerms),this.agreeToTerms||this.errors.push("You must agree to the terms of service"),0===this.errors.length}}}),B=G,J=(r("d40e"),Object(h["a"])(B,M,Z,!1,null,"58c5b09a",null)),F=J.exports,W=function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[t("h1",[e._v("Login")]),t("div",{attrs:{id:"api-holder"}},[t("input",{directives:[{name:"model",rawName:"v-model",value:e.user.email,expression:"user.email"}],attrs:{type:"text",placeholder:"Email"},domProps:{value:e.user.email},on:{keyup:function(t){return!t.type.indexOf("key")&&e._k(t.keyCode,"enter",13,t.key,"Enter")?null:e.login.apply(null,arguments)},input:function(t){t.target.composing||e.$set(e.user,"email",t.target.value)}}}),t("input",{directives:[{name:"model",rawName:"v-model",value:e.user.password,expression:"user.password"}],attrs:{type:"password",placeholder:"Password"},domProps:{value:e.user.password},on:{keyup:function(t){return!t.type.indexOf("key")&&e._k(t.keyCode,"enter",13,t.key,"Enter")?null:e.login.apply(null,arguments)},input:function(t){t.target.composing||e.$set(e.user,"password",t.target.value)}}}),t("br"),e.error?t("div",{staticClass:"error-holder"},[e._v(" "+e._s(e.error)+" "),t("span",{staticClass:"material-symbols-outlined cancel",on:{click:e.closeError}},[e._v(" cancel ")])]):e._e(),t("input",{staticClass:"primary-btn",attrs:{type:"submit",value:"Login"},on:{click:e.login}}),t("a",{attrs:{href:"#/user-create"}},[e._v("Create New User")])])])},Y=[];const H=new s["default"];var Q=s["default"].extend({data(){return{user:{email:"",password:""},error:""}},created(){this.user.email=localStorage.getItem("email")},methods:{async login(){this.error="";var e=await d.postData("/user/login",this.user);e.success?(console.log(e),e.verified?(localStorage.setItem("role",e.role),localStorage.setItem("email",e.email),H.$emit("userLoggedIn"),"Admin"==e.role?window.location="/#/vendors":window.location="/#/"):this.error="Please check your email for the verification email before your first login"):this.error="Authentication attempt failed"},closeError(){this.error=""}}}),X=Q,ee=(r("e094"),Object(h["a"])(X,W,Y,!1,null,"5ebbe42e",null)),te=ee.exports,re=function(){var e=this;e._self._c;return e._m(0)},se=[function(){var e=this,t=e._self._c;return t("div",{staticClass:"post"},[t("h1",[e._v("Not Found")])])}],ae=s["default"].extend({data(){return{}}}),ie=ae,oe=Object(h["a"])(ie,re,se,!1,null,"439dd121",null),ne=oe.exports,le=function(){var e=this,t=e._self._c;return t("div",{staticClass:"holder"},[t("h1",[e._v("Welcome")]),t("a",{staticClass:"register-vendor",attrs:{href:"/#/vendor-registration"}},["User"==e.role?t("span",[e._v("Register")]):e._e(),"Vendor"==e.role||"Admin"==e.role?t("span",[e._v("Edit")]):e._e(),e._v(" Vendor Listing "),t("span",{staticClass:"material-symbols-outlined"},[e._v(" arrow_circle_right ")])]),e._m(0)])},de=[function(){var e=this,t=e._self._c;return t("a",{staticClass:"get-api",attrs:{href:"/#/api-key"}},[e._v("Get API"),t("span",{staticClass:"material-symbols-outlined"},[e._v(" arrow_circle_right ")])])}],ue=s["default"].extend({data(){return{role:""}},created(){this.role=localStorage.getItem("role")}}),ce=ue,pe=(r("16c9"),Object(h["a"])(ce,le,de,!1,null,"302457d5",null)),he=pe.exports;const me={"/":he,"/users":v,"/user-create":x,"/vendors":U,"/vendor-registration":z,"/api-key":q,"/api-registration":F,"/login":te},ve=Object(s["ref"])(window.location.hash);window.addEventListener("hashchange",()=>{ve.value=window.location.hash});const ge=Object(s["computed"])(()=>{var e=ve.value.split("?")[0].slice(1);return me[e||"/"]||ne}),fe=[{uri:"#/api-registration",text:"Register API",role:"Vendor"},{uri:"#/api-registration",text:"Register API",role:"User"},{uri:"#/vendor-registration",text:"Register as Vendor",role:"User"},{uri:"#/vendor-registration",text:"Edit Vendor Listing",role:"Vendor"},{uri:"#/users",text:"Users",role:"Admin"},{uri:"#/vendors",text:"Vendors",role:"Admin"},{uri:"#/login",text:"Log in",role:"all",hideIfAuth:!0}];var _e=s["default"].extend({name:"App",data(){return{loggedIn:!1,currentView:ge,links:fe}},created(){this.evaluateLinks(),H.$on("userLoggedIn",()=>{localStorage.setItem("loggedIn",!0),this.evaluateLinks()})},methods:{async logout(){await d.getData("user/logout"),console.log("Logging out"),this.loggedIn=!1,localStorage.setItem("email",""),localStorage.setItem("role",""),localStorage.setItem("loggedIn",!1),window.location="/#/login",this.evaluateLinks()},evaluateLinks(){this.links=fe,this.email=localStorage.getItem("email"),this.role=localStorage.getItem("role"),this.loggedIn=localStorage.getItem("loggedIn");var e=this.links.filter(e=>"all"==e.role),t=this.links.filter(e=>e.role==this.role);this.links=[...e,...t],this.email&&(this.links=this.links.filter(e=>!e.hideIfAuth))}},components:{Users:v,Vendors:U,VendorRegistration:z}}),ye=_e,be=(r("a7d8"),Object(h["a"])(ye,a,i,!1,null,null,null)),xe=be.exports;s["default"].config.productionTip=!1,new s["default"]({render:e=>e(xe)}).$mount("#app")},7198:function(e,t,r){},a63a:function(e,t,r){"use strict";r("2e10")},a7d8:function(e,t,r){"use strict";r("38ac")},b606:function(e,t,r){},d40e:function(e,t,r){"use strict";r("eddd")},e094:function(e,t,r){"use strict";r("3948")},e463:function(e,t,r){"use strict";r("7198")},eddd:function(e,t,r){},fd0d:function(e,t,r){}});
//# sourceMappingURL=app.dbfde682.js.map