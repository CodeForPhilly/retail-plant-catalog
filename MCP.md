# PlantAgents MCP Server

The PlantAgents MCP (Model Context Protocol) server exposes plants, vendors, crawl, and approval tools so AI assistants (e.g. Cursor, Claude Code) can query and manage vendor and plant data via the existing web API.

## Adding the server to Cursor (mcp.json)

1. **Locate or create MCP config**
   - **macOS:** `~/.cursor/mcp.json`
   - **Windows:** `%USERPROFILE%\.cursor\mcp.json`
   - If the file does not exist, create it with `{}`.

2. **Add the PlantAgents MCP server**

   Add a new entry under the `mcpServers` key. Use **npx** to run the published package (no repo clone needed), with the hosted API at `https://app.plantagents.org`:

   ```json
   {
     "mcpServers": {
       "plantagents": {
         "command": "npx",
         "args": ["-y", "plant-agents-mcp"],
         "env": {
           "PLANTAGENTS_API_URL": "https://app.plantagents.org",
           "PLANTAGENTS_BEARER_TOKEN": "YOUR_API_KEY_HERE"
         }
       }
     }
   }
   ```

   Replace `YOUR_API_KEY_HERE` with a valid API key (Bearer token) from your user account. See [Bearer token generation](#bearer-token-generation) below.

3. **Restart Cursor** (or reload the window) so it picks up the new MCP config.

**Claude Code:** To use this server in Claude Code instead of Cursor, see [Setting up the PlantAgents MCP server with Claude Code](docs/MCP-Claude-Code-setup.md).

## Bearer token generation

The MCP server calls the web API with **Bearer token** authentication. You must use an API key that the API recognizes.

- **Where it comes from:** In this project, the API key is the `ApiKey` field on the `user` table. It is validated by the web API (e.g. `ApiAuthorize` / `FindByKey`).
- **How to get one:**
  - **Option A:** Your app may expose “Generate API key” (or similar) in the user/admin UI; use that key.
  - **Option B:** If you have DB access, an admin can set or generate a key for a user, e.g. via the repository method that updates `user.ApiKey` (and optionally the user’s “API key” screen if present).
- **Use in mcp.json:** Put that value in `PLANTAGENTS_BEARER_TOKEN` in the server’s `env` block, as in the example above. Do **not** commit this file with real keys; keep it local or in a secure secret store.

**Claude Code users:** For step-by-step setup of your `mcp.json` (or Claude config) with this API key, see [Setting up the PlantAgents MCP server with Claude Code](docs/MCP-Claude-Code-setup.md).

For more detail on how the web API uses this token, see the main [README](README.md) and the API/auth docs referenced there (including any section on “Bearer token” or “API key”).

## Environment variables

| Variable | Description |
|----------|-------------|
| `PLANTAGENTS_API_URL` | Base URL of the web API. Use `https://app.plantagents.org` for the hosted app. |
| `PLANTAGENTS_BEARER_TOKEN` | API key sent as `Authorization: Bearer <token>`. **Required** for authenticated endpoints. |

These are typically set in the `env` block for the server in `mcp.json`, as shown above.

## Tools

### Query

- **mcp_plants_lookup** – Get a single plant by ID.
- **mcp_plants_list** – List plants with optional filters: `name`, `scientificName`, `limit`.
- **mcp_vendors_lookup** – Get a single vendor by ID (with populated URLs and crawl status).
- **mcp_vendors_list** – List vendors with optional filters: `state`, `approved`, `limit`.
- **mcp_plants_by_vendor** – For a vendor: `lookup_plant` (one plant), `lookup_nursery` (vendor details), or `list` (all plants).

### Crawl status

Vendor lookup (**mcp_vendors_lookup** / **FindById**) returns:

- Overall crawl status: `LastCrawlStatus`, `LastCrawled`, `CrawlErrors`, `CrawlInProgress`.
- Per-URL status in `PlantListingUris`: `LastStatus`, `PlantCount`, `CrawlInProgress`.

### Populate / no-duplicates

- **create_vendor** – Create a vendor (validates duplicate URLs); returns created vendor with populated URLs.
- **update_vendor** – Update by ID; validates duplicate URLs in the list; returns updated vendor.
- **add_vendor_plant_list_urls** – Add URLs for a vendor; checks duplicates and tests each URL; returns added URLs with status.
- **test_vendor_url** – Test a single URL for a vendor (status and plant count preview).
- **trigger_crawl** – Run crawl for a vendor; optional `urlId` to crawl a single URL; errors if a crawl is already in progress.
- **approval_vendor** – Approve or deny a vendor (sends email); optional `denialReason`; caller must be Admin.

## Implementation notes

- The MCP server is a small Node.js app in `mcp-server/` that talks to the existing web API over HTTP.
- All authenticated requests use the Bearer token from `PLANTAGENTS_BEARER_TOKEN`.
- Duplicate URL checks, TestUrl, and approval email behavior are implemented in the web API; the MCP layer only invokes the corresponding endpoints.
