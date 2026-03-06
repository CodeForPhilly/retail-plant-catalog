/**
 * API client for PlantAgents webapi. Uses base URL and Bearer token from env.
 */
const getConfig = () => {
    const baseUrl = process.env.PLANTAGENTS_API_URL || "http://localhost:5000";
    const token = process.env.PLANTAGENTS_BEARER_TOKEN || "";
    return { baseUrl: baseUrl.replace(/\/$/, ""), token };
};
export async function apiGet(path, params) {
    const { baseUrl, token } = getConfig();
    const url = new URL(path, baseUrl);
    if (params) {
        Object.entries(params).forEach(([k, v]) => {
            if (v !== undefined && v !== "")
                url.searchParams.set(k, String(v));
        });
    }
    const res = await fetch(url.toString(), {
        headers: token ? { Authorization: `Bearer ${token}` } : {},
    });
    if (!res.ok) {
        const text = await res.text();
        throw new Error(`API ${res.status}: ${text || res.statusText}`);
    }
    if (res.status === 204 || res.headers.get("content-length") === "0")
        return undefined;
    return res.json();
}
export async function apiPost(path, body) {
    const { baseUrl, token } = getConfig();
    const url = `${baseUrl}${path.startsWith("/") ? path : "/" + path}`;
    const res = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
        },
        body: body ? JSON.stringify(body) : undefined,
    });
    if (!res.ok) {
        const text = await res.text();
        throw new Error(`API ${res.status}: ${text || res.statusText}`);
    }
    if (res.status === 204 || res.headers.get("content-length") === "0")
        return undefined;
    return res.json();
}
export async function apiPut(path, body) {
    const { baseUrl, token } = getConfig();
    const url = `${baseUrl}${path.startsWith("/") ? path : "/" + path}`;
    const res = await fetch(url, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
        },
        body: JSON.stringify(body),
    });
    if (!res.ok) {
        const text = await res.text();
        throw new Error(`API ${res.status}: ${text || res.statusText}`);
    }
    if (res.status === 204 || res.headers.get("content-length") === "0")
        return undefined;
    return res.json();
}
