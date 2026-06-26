# Publishing the PlantAgents MCP Server to npm

This guide explains how to publish the MCP server as the npm package **plant-agents-mcp** so users can run it with `npx plant-agents-mcp` without cloning the repo.

## Prerequisites

- Node.js 18+
- An [npm](https://www.npmjs.com/) account
- npm CLI logged in: `npm login`

## Package identity

- **Package name:** `plant-agents-mcp`
- **npx command:** `npx plant-agents-mcp`
- **Bin entry:** `plant-agents-mcp` → `dist/index.js` (see `mcp-server/package.json`)

## Build for publish

From the repo root:

```bash
cd /path/to/SavvyCrawler/mcp-server
npm ci
npm run build
```

Ensure `dist/index.js` exists and includes the shebang `#!/usr/bin/env node` so npx can execute it.

## Version and changelog

1. Bump the version in `mcp-server/package.json`:
   - **Patch:** bug fixes → `1.0.1`
   - **Minor:** new tools or options → `1.1.0`
   - **Major:** breaking changes → `2.0.0`

2. Optionally document changes in a `CHANGELOG.md` in `mcp-server/` or in the repo root.

## Publishing to npm

1. From `mcp-server/`:

   ```bash
   cd /path/to/SavvyCrawler/mcp-server
   npm publish
   ```

2. For a scoped package (e.g. `@your-org/plant-agents-mcp`), use:

   ```bash
   npm publish --access public
   ```

3. To do a dry run without publishing:

   ```bash
   npm publish --dry-run
   ```

   This lists the files that would be included (only `dist/` if `files` is set in `package.json`).

## What gets published

- The `files` field in `mcp-server/package.json` is set to `["dist"]`, so only the `dist/` directory is included. Source (`src/`), config, and lockfile are not published.
- Consumers get the compiled `dist/index.js` and run it via the `plant-agents-mcp` bin.

## After publishing

- Users can install and run with:
  ```bash
  npx plant-agents-mcp
  ```
- In **mcp.json** (Cursor or Claude Code), the server is configured with:
  - **command:** `npx`
  - **args:** `["plant-agents-mcp"]`
  - Plus `env` for `PLANTAGENTS_API_URL` and `PLANTAGENTS_BEARER_TOKEN` as in [MCP.md](../MCP.md).

## Troubleshooting

| Issue | Check |
|-------|--------|
| Name already taken | Choose a scoped name, e.g. `@yourorg/plant-agents-mcp`, and use `--access public` when publishing. |
| npx runs old version | Users may need to run `npx plant-agents-mcp@latest` or clear npx cache. |
| Bin not found | Ensure `package.json` has `"bin": { "plant-agents-mcp": "dist/index.js" }` and that `dist/index.js` is in the tarball (`npm pack` and inspect). |

## Unpublishing (emergency only)

- Unpublish within 72 hours: `npm unpublish plant-agents-mcp@<version>`
- After 72 hours, unpublish is no longer allowed; publish a new version that deprecates or fixes the problem instead.
