<div align="center">

<img src="https://raw.githubusercontent.com/Shaikh-Imran/DeeplyDepBuilder/main/assets/DeeplyDepBuilder-128X128.png" alt="DeeplyDepBuilder logo" width="128" height="128" />

# DeeplyDepBuilder

Visualize .NET project dependencies and build independent projects in parallel.

[![NuGet version](https://img.shields.io/nuget/v/CodersTea.DeeplyDepBuilder?style=flat-square)](https://www.nuget.org/packages/CodersTea.DeeplyDepBuilder)
[![NuGet downloads](https://img.shields.io/nuget/dt/CodersTea.DeeplyDepBuilder?style=flat-square)](https://www.nuget.org/packages/CodersTea.DeeplyDepBuilder)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)

</div>

## Table of contents

- [Overview](#overview)
- [Features](#features)
- [Installation](#installation)
  - [Via NuGet](#via-nuget)
  - [From source](#from-source)
- [Usage](#usage)
  - [CLI options](#cli-options)
  - [Examples](#examples)
- [Dependency graph examples](#dependency-graph-examples)
  - [Simple example](#simple-example)
  - [Complex example](#complex-example)
- [Roadmap](#roadmap)
- [License](#license)

## Overview

`dotnet build` builds project references by default. DeeplyDepBuilder adds dependency visualization and level-based parallel execution.

The tool parses your solution or project references, flags circular dependencies, and groups projects into discrete levels with a topological sort. Projects within the same level have no dependencies on each other and build in parallel. You can also export the dependency tree as a Mermaid diagram.

Build time comparisons against standard `dotnet build` are not yet benchmarked.

## Features

- Cycle detection to catch circular references before building.
- Mermaid graph export to Markdown for project trees and topological levels.
- Level-based topological sort so independent projects build concurrently.
- Clean command that runs `dotnet clean` across the entire resolved tree.
- Option to display file names instead of full paths in graphs.

## Installation

### Via NuGet

Install the tool globally:

```bash
dotnet tool install --global CodersTea.DeeplyDepBuilder
```

Package details are on [NuGet](https://www.nuget.org/packages/CodersTea.DeeplyDepBuilder).

### From source

```bash
git clone https://github.com/Shaikh-Imran/DeeplyDepBuilder.git
cd DeeplyDepBuilder
dotnet build DeeplyDepBuilder.sln
dotnet tool install --global --add-source ./nupkg CodersTea.DeeplyDepBuilder
```

## Usage

Pass a solution or project path:

```bash
deeplydep -p /path/to/solution.sln
```

To run directly from source:

```bash
dotnet run --project DeeplyDepBuilder.csproj -- -p /path/to/solution.sln
```

### CLI options

| Option | Short | Default | Description |
|---|---|---|---|
| `--project` | `-p` | None (required) | Path to `.csproj` or `.sln` |
| `--clean` | `-c` | `false` | Run `dotnet clean` instead of `dotnet build` |
| `--generate-graph-path` | `-g` | None | Write a Markdown file with Mermaid diagrams to this path |
| `--hide-path-in-graph` | | `false` | Show file names instead of full paths in graphs |
| `--no-parallel` | | `false` | Build projects sequentially instead of in parallel |
| `--show-build-output` | | `false` | Print `dotnet` build output to the console |
| `--verbose` | `-v` | `false` | Print trace logs |
| `--help` | | | Show help text |
| `--version` | | | Show version |

### Examples

Build a project:
```bash
deeplydep -p /path/to/project.csproj
```

Clean all projects in the dependency graph:
```bash
deeplydep -p /path/to/solution.sln -c
```

Generate a Mermaid graph file and build:
```bash
deeplydep -p /path/to/solution.sln -g /path/to/output/graph.md
```

Build sequentially and print build output:
```bash
deeplydep -p /path/to/solution.sln --no-parallel --show-build-output
```

## Dependency graph examples

Test solution: [dotnet-mono-repo-example](https://github.com/Shaikh-Imran/dotnet-mono-repo-example) (for cycle detection, see the [`cyclic-dep`](https://github.com/Shaikh-Imran/dotnet-mono-repo-example/tree/cyclic-dep) branch).

### Simple example

#### Project structure
```mermaid
graph TD

MS["my Solution"]

P1["P1"]
P2["P2"]
P3["Shared P3"]
P4["Shared P4"]

MS --> P1
MS --> P2
MS --> P4

P2 --> P3
P3 --> P4
```

#### Topological sort
```mermaid
flowchart TD

P4["Shared P4"] --> P3["Shared P3"]
P3 --> P2["P2"]
P2 --> P1["P1"]
P1 --> MS["my Solution"]
```

#### Topological sort with levels
```mermaid
flowchart TD
    subgraph Level0
        direction TB
        P4["Shared P4"]
    end

    subgraph Level1
        direction TB
        P3["Shared P3"]
    end

    subgraph Level2
        direction TB
        P1["P1"]
        P2["P2"]
    end

    subgraph Level3
        MS["my Solution"]
    end

Level0 --> Level1
Level1 --> Level2
Level2 --> Level3
```

### Complex example

#### Project structure
```mermaid
graph TD

MS["my Solution"]
P1["P1"]
P2["P2"]
P3["P3"]
P8["P8"]
P4["Common P4"]
P5["Common P5"]
P6["Shared P6"]
P7["Shared P7"]

MS --> P1
MS --> P2
MS --> P3
MS --> P8
MS --> P4
MS --> P7
P3 --> P2
P2 --> P5
P3 --> P5
P4 --> P6
P2 --> P6
P6 --> P7
P5 --> P7
```

#### Topological sort
```mermaid
flowchart TD

P7["Shared P7"] --> P6["Shared P6"]
P6 --> P5["Common P5"]
P5 --> P4["Common P4"]
P4 --> P2["P2"]
P2 --> P3["P3"]
P3 --> P1["P1"]
P1 --> P8["P8"]
P8 --> MS["my Solution"]
```

#### Topological sort with level groups
```mermaid
flowchart TD

    subgraph Level0["Level 0"]
        direction TB
        P7["Shared P7"]
    end

    subgraph Level1["Level 1"]
        direction TB
        P5["Common P5"]
        P6["Shared P6"]
    end

    subgraph Level2["Level 2"]
        direction TB
        P2["P2"]
    end

    subgraph Level3["Level 3"]
        direction TB
        P1["P1"]
        P3["P3"]
        P8["P8"]
        P4["Common P4"]
    end

    subgraph Level4["Level 4 - Root"]
        direction TB
        MS["my Solution"]
    end

Level0 --> Level1
Level1 --> Level2
Level2 --> Level3
Level3 --> Level4
```

## Roadmap

- [x] Graph visualization
- [x] Graph cycle detection
- [x] Graph level grouping
- [x] Parallel build by level
- [x] Clean build
- [ ] Skip build if project is up to date
- [ ] Shorten paths in graph output
- [ ] Terminal-based graph visualization

## License

[MIT](LICENSE)
