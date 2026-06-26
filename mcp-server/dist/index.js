#!/usr/bin/env node
/**
 * PlantAgents MCP Server – exposes plants, vendors, crawl, and approval tools.
 * Uses PLANTAGENTS_API_URL and PLANTAGENTS_BEARER_TOKEN (or env from mcp.json).
 */
import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { z } from "zod";
import * as api from "./api.js";
function textResult(data, isError = false) {
    return {
        content: [{ type: "text", text: JSON.stringify(data, null, 2) }],
        isError,
    };
}
function main() {
    const server = new McpServer({ name: "plantagents-mcp", version: "1.0.0" }, { capabilities: {} });
    // Use registerTool with inputSchema as raw shape (ZodRawShapeCompat)
    const register = (name, description, inputSchema, handler) => {
        server.registerTool(name, { description, inputSchema }, handler);
    };
    // —— Plants ——
    register("mcp_plants_lookup", "Get a single plant by ID", { plantId: z.string() }, async ({ plantId }) => {
        const plant = await api.apiGet("/Plant/Get", { id: plantId });
        return textResult(plant);
    });
    register("mcp_plants_list", "List plants with optional name/scientificName filter and limit", { name: z.string().optional(), scientificName: z.string().optional(), limit: z.number().optional() }, async (filters) => {
        const list = await api.apiGet("/Plant/List", filters);
        return textResult(list ?? []);
    });
    // —— Vendors ——
    register("mcp_vendors_lookup", "Get a single vendor by ID with populated plant listing URLs and crawl status", { vendorId: z.string() }, async ({ vendorId }) => {
        const vendor = await api.apiGet("/Vendor/FindById", { id: vendorId });
        return textResult(vendor);
    });
    register("mcp_vendors_list", "List vendors with optional state/approved filter and limit", { state: z.string().optional(), approved: z.boolean().optional(), limit: z.number().optional() }, async (filters) => {
        const list = await api.apiGet("/Vendor/ListClient", filters);
        return textResult(Array.isArray(list) ? list : []);
    });
    // —— Plants by vendor ——
    register("mcp_plants_by_vendor", "Lookup plant list for a vendor, or vendor details, or list all plants for a vendor", { vendorId: z.string(), action: z.enum(["lookup_plant", "lookup_nursery", "list"]) }, async ({ vendorId, action }) => {
        if (action === "lookup_nursery") {
            const vendor = await api.apiGet("/Vendor/FindById", { id: vendorId });
            return textResult(vendor);
        }
        const plants = await api.apiGet("/Plant/FindByVendor", { vendorId: vendorId });
        if (action === "lookup_plant") {
            return textResult(Array.isArray(plants) ? plants[0] ?? null : null);
        }
        return textResult(Array.isArray(plants) ? plants : []);
    });
    // —— create_vendor ——
    register("create_vendor", "Create a vendor. Validates duplicate URLs; returns created vendor with populated URLs.", {
        StoreName: z.string(),
        Address: z.string(),
        State: z.string().length(2),
        Lat: z.number(),
        Lng: z.number(),
        StoreUrl: z.string().url(),
        PublicEmail: z.string().email(),
        PublicPhone: z.string(),
        AllNative: z.boolean().optional(),
        LivePlant: z.boolean().optional(),
        Seed: z.boolean().optional(),
        Notes: z.string().optional(),
        PlantListingUrls: z.array(z.string().url()).optional(),
    }, async (body) => {
        const res = await api.apiPost("/Vendor/CreateClientApi", body);
        const vendor = await api.apiGet("/Vendor/FindById", { id: res.id });
        return textResult(vendor);
    });
    // —— update_vendor ——
    register("update_vendor", "Update a vendor by ID. Validates duplicate URLs in list. Returns updated vendor.", {
        Id: z.string(),
        StoreName: z.string(),
        Address: z.string(),
        State: z.string().length(2),
        Lat: z.number(),
        Lng: z.number(),
        StoreUrl: z.string().url(),
        PublicEmail: z.string().email(),
        PublicPhone: z.string(),
        AllNative: z.boolean().optional(),
        LivePlant: z.boolean().optional(),
        Seed: z.boolean().optional(),
        Notes: z.string().optional(),
        PlantListingUrls: z.array(z.string()),
    }, async (body) => {
        const b = body;
        await api.apiPut("/Vendor/UpdateClientApi", b);
        const vendor = await api.apiGet("/Vendor/FindById", { id: b.Id });
        return textResult(vendor);
    });
    // —— mcp_partners_list ——
    register("mcp_partners_list", "List partner directories (e.g. Xerces) and their ids, for use with set_vendor_partner.", {}, async () => {
        const list = await api.apiGet("/Vendor/PartnersApi", {});
        return textResult(Array.isArray(list) ? list : []);
    });
    // —— set_vendor_partner ——
    register("set_vendor_partner", "Set or clear a vendor's partner-directory link (e.g. Xerces). Pass partner='' (or omit) to clear the link. externalKey is the partner-side id for this vendor (optional). Use mcp_partners_list for valid partner ids.", { vendorId: z.string(), partner: z.string().optional(), externalKey: z.string().optional() }, async ({ vendorId, partner, externalKey }) => {
        const res = await api.apiPost("/Vendor/SetPartnerApi", {
            VendorId: vendorId,
            Partner: partner ?? "",
            ExternalKey: externalKey ?? "",
        });
        return textResult(res);
    });
    // —— add_vendor_plant_list_urls ——
    register("add_vendor_plant_list_urls", "Add plant listing URLs to a vendor. Checks duplicates, tests each URL, adds only non-duplicate. Returns list of added URLs with status.", { vendorId: z.string(), urls: z.array(z.string().url()) }, async ({ vendorId, urls }) => {
        const results = [];
        for (const url of urls) {
            try {
                const res = await api.apiPost("/Vendor/TestUrl", { VendorId: vendorId, Url: url });
                results.push({ url, added: res.success, message: res.message });
            }
            catch (e) {
                results.push({ url, added: false, message: e instanceof Error ? e.message : String(e) });
            }
        }
        return textResult(results);
    });
    // —— test_vendor_url ——
    register("test_vendor_url", "Test a plant listing URL for a vendor. Returns status and plant count preview.", { vendorId: z.string(), url: z.string().url() }, async ({ vendorId, url }) => {
        const res = await api.apiPost("/Vendor/TestUrl", { VendorId: vendorId, Url: url });
        return textResult(res);
    });
    // —— trigger_crawl ——
    register("trigger_crawl", "Trigger crawl for a vendor. If urlId provided, crawl single URL; else crawl all. Returns crawl status. Fails if crawl already in progress.", { vendorId: z.string(), urlId: z.string().optional() }, async ({ vendorId, urlId }) => {
        const params = { id: vendorId };
        if (urlId)
            params.urlId = urlId;
        const q = new URLSearchParams(params).toString();
        const res = await api.apiPost(`/Vendor/CrawlByUrl?${q}`);
        return textResult(res);
    });
    // —— approval_vendor ——
    register("approval_vendor", "Approve or deny a vendor. Sends email notification. Caller must be Admin.", { vendorId: z.string(), approved: z.boolean(), denialReason: z.string().optional() }, async ({ vendorId, approved, denialReason }) => {
        const res = await api.apiPost("/Vendor/ApproveClient", {
            Id: vendorId,
            Approved: approved,
            DenialReason: denialReason ?? undefined,
        });
        return textResult(res);
    });
    const transport = new StdioServerTransport();
    server.connect(transport).catch((err) => {
        console.error("MCP server connection error", err);
        process.exit(1);
    });
}
main();
