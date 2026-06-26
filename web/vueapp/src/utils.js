async function getRequest(url, data = {}, method="GET"){
    return await fetch(url, {
        method: method, // *GET, POST, PUT, DELETE, etc.
        mode: "cors", // no-cors, *cors, same-origin
        cache: "no-cache", // *default, no-cache, reload, force-cache, only-if-cached
        credentials: "same-origin", // include, *same-origin, omit
        headers: {
        "Content-Type": "application/json",
        // 'Content-Type': 'application/x-www-form-urlencoded',
        },
        redirect: "manual", // manual, *follow, error
        referrerPolicy: "no-referrer", // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
        body: method == "GET" ? undefined : JSON.stringify(data), // body data type must match "Content-Type" header
    })
    .then(r =>{
        if (r.status ==0 && r.url){
            window.location = "/#/login";
            return {};
        } 
        return r.json();
    } )
}


export default {
    async getData(url) {
        return getRequest(url);
    },
    async putData(url, data = {}){
        return getRequest(url, data, "PUT")
    },
    async postData(url, data = {}){
        return getRequest(url, data, "POST")
    },
    async delData(url){
        return getRequest(url, "DELETE")
    },
    async downloadExport(url, filename) {
        const response = await fetch(url, {
            method: "GET",
            mode: "cors",
            cache: "no-cache",
            credentials: "same-origin",
            redirect: "manual"
        });
        if (!response.ok) throw new Error(response.statusText || "Export failed");
        const blob = await response.blob();
        const a = document.createElement("a");
        a.href = URL.createObjectURL(blob);
        a.download = filename || "export.csv";
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(a.href);
    }
}