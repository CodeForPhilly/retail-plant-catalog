* See README.md in sub projects.

## MCP (Model Context Protocol)

An MCP server is provided so Cursor (or other MCP clients) can query plants and vendors, trigger crawls, and manage approvals. See **[MCP.md](MCP.md)** for:

- How to add the server to your Cursor **mcp.json** (including **Bearer token** for API authorization)
- Environment variables (`PLANTAGENTS_API_URL`, `PLANTAGENTS_BEARER_TOKEN`)
- List of tools (plants, vendors, crawl, approval)

**Bearer token:** The MCP server calls the web API with a Bearer token. How to obtain and use it (including in `mcp.json`) is described in [MCP.md#bearer-token-generation](MCP.md#bearer-token-generation).

**Publishing to npm:** The MCP server can be published as the npm package **plant-agents-mcp** for use with `npx plant-agents-mcp`. See [docs/PUBLISHING-MCP-NPM.md](docs/PUBLISHING-MCP-NPM.md) for the publish guide.
