# Setting up the PlantAgents MCP server with Claude Code

This page explains how to add the PlantAgents MCP server to **Claude Code** so Claude can use plants, vendors, crawl, and approval tools.

## Prerequisites

- [Claude Code](https://code.claude.com/) installed
- Node.js 18+ (for `npx`)
- A PlantAgents API key (Bearer token). Get one from your app’s **API Key** page (e.g. **Generate API key**), then use it below.

## Where Claude Code stores MCP config

Claude Code can use:

- **User (global):** `~/.claude.json` or config under `~/.claude/`
- **Project:** `.mcp.json` in the project root (shared with the team)

Use the scope that fits how you work (just you vs. whole project).

## Add the PlantAgents MCP server

Run the server with **npx** (no repo clone needed). Open your Claude Code MCP config file (e.g. `~/.claude.json` or the file used for your scope), ensure there is an `mcpServers` object, and add a `plantagents` entry like this:

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

Replace `YOUR_API_KEY_HERE` with your API key from the app’s **API Key** page at https://app.plantagents.org. Save the file and restart Claude Code (or reload the window). The server should show up in Claude’s MCP list.

## Environment variables

| Variable | Description |
|----------|-------------|
| `PLANTAGENTS_API_URL` | Base URL of the PlantAgents web API. Use `https://app.plantagents.org`. |
| `PLANTAGENTS_BEARER_TOKEN` | Your API key (Bearer token). **Required** for authenticated endpoints. |

Get the API key from your app’s **API Key** page at https://app.plantagents.org; do not commit it or share it.

## Verify

- In Claude Code, check that the PlantAgents MCP server is listed (e.g. in MCP or settings).
- Ask Claude to list plants or vendors; if the server is configured correctly, it will use the MCP tools.

## Cursor users

If you use **Cursor** instead of Claude Code, see the main [MCP.md](../MCP.md) in this repo for Cursor’s `mcp.json` paths and the same env variables.
