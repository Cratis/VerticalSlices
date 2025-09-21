# Vertical Slices MCP

[![Discord](https://img.shields.io/discord/1182595891576717413?label=Discord&logo=discord&color=7289da)](https://discord.gg/kt4AMpV8WV)
[![Docker](https://img.shields.io/docker/v/cratis/vertical-slices-mcp?label=VerticalSlices&logo=docker&sort=semver)](https://hub.docker.com/r/cratis/vertical-slices-mcp)
[![Build](https://github.com/Cratis/VerticalSlices/actions/workflows/build.yml/badge.svg)](https://github.com/Cratis/VerticalSlices/actions/workflows/build.yml)
[![Publish](https://github.com/cratis/VerticalSlices/actions/workflows/publish.yml/badge.svg)](https://github.com/Cratis/VerticalSlices/actions/workflows/publish.yml)

## Using

The Vertical Slices MCP server leverages Stdio and is packaged as a container.
In your tool, configure it using that.

### Example: VSCode

In VSCode you would do this by adding a tool to your agent.
This can done either by adding it to the global user settings or through an `mcp.json` file in
the `.vscode` folder of your project.

For the global user settings, you simply do the following:

```json
"mcp": {
    "servers": {
        "VerticalSlices": {
            "type": "stdio",
            "command": "docker",
            "args": [
                "run",
                "-i",
                "--rm",
                "cratis/vertical-slices-mcp"
            ]
        }
    }
}
```

For a local `mcp.json` file, its almost the same:

```json
{
    "servers": {
        "VerticalSlices": {
            "type": "stdio",
            "command": "docker",
            "args": [
                "run",
                "-i",
                "--rm",
                "cratis/vertical-slices-mcp"
            ]
        }
    }
}
```

You can see this in action in the [mcp.json](./.vscode/mcp.json) in this repository.

> Note: The `cratis/vertical-slices-mcp` is a multi CPU architecture image supporting both x64 and arm64 automatically.

## Prompts / Tools

## Local development

Using VSCode, the [mcp.json](./.vscode/mcp.json) in the `.vscode` folder of this repository is automatically supported.
Open it and click the **Start** button:

![](./images/start.png)

During development, compile and click the **Restart** button when having the `mcp.json` open:

![](./images/restart.png)
