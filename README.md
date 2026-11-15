<div align="center">

<img src="https://raw.githubusercontent.com/Shaikh-Imran/DeeplyDepBuilder/main/assets/DeeplyDepBuilder-128X128.png" alt="DeeplyDepBuilder Logo" width="128" height="128" />

# DeeplyDepBuilder

CLI tool to visualize .NET dependency graphs and build independent project levels in parallel.

[![NuGet Version](https://img.shields.io/nuget/v/CodersTea.DeeplyDepBuilder?style=flat-square)](https://www.nuget.org/packages/CodersTea.DeeplyDepBuilder)
[![NuGet Downloads](https://img.shields.io/nuget/dt/CodersTea.DeeplyDepBuilder?style=flat-square)](https://www.nuget.org/packages/CodersTea.DeeplyDepBuilder)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)

</div>

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Installation](#installation)
  - [Via NuGet](#via-nuget)
  - [From Source](#from-source)
- [Usage](#usage)
  - [CLI Options](#cli-options)
  - [Common Commands](#common-commands)
- [Dependency Graph Examples](#dependency-graph-examples)
  - [Simple Example](#simple-example)
  - [Complex Example](#complex-example)
- [Roadmap](#roadmap)
- [License](#license)

---

## Overview

DeeplyDepBuilder analyzes the complete project reference dependency graph of a .NET solution or project.

While `dotnet build` automatically resolves and builds project references, understanding deep dependency chains across large solutions or monorepos remains challenging. DeeplyDepBuilder addresses this by:

- **Visualizing Dependency Graphs**: Generates Mermaid diagrams illustrating project relationships, topological order, and level groupings.
- **Level-Parallel Builds**: Uses topological sorting to group mutually independent projects into discrete levels and builds them concurrently.
- **Cycle Detection**: Detects and reports circular dependencies before execution.

> **Note:** Direct performance comparisons and benchmarks between standard `dotnet build` and DeeplyDepBuilder's level-parallel build are currently in progress.

---

## Features

- **Cycle Detection**: Identifies circular references in project dependencies.
- **Mermaid Graph Generation**: Exports dependency graphs rooted at the target solution or project to Markdown.
- **Topological Sorting with Level Grouping**: Maps dependencies into execution levels of independent projects.
- **Level-Parallel Build**: Concurrently builds independent projects within the same topological level.
- **Clean Support**: Performs `dotnet clean` across the entire resolved dependency graph.
- **Customizable Graph Output**: Optional flag to display short project names instead of full file paths.

---

## Installation

### Via NuGet

Install DeeplyDepBuilder as a global .NET tool:

```bash
dotnet tool install --global CodersTea.DeeplyDepBuilder
```

> Latest package details: [NuGet - CodersTea.DeeplyDepBuilder](https://www.nuget.org/packages/CodersTea.DeeplyDepBuilder)

### From Source

Clone the repository and install the tool locally:

```bash
# Clone the repository
git clone https://github.com/Shaikh-Imran/DeeplyDepBuilder.git
cd DeeplyDepBuilder

# Build solution and create package
dotnet build DeeplyDepBuilder.sln

# Install globally from local package source
dotnet tool install --global --add-source ./nupkg CodersTea.DeeplyDepBuilder

# Verify installation
deeplydep --help
```

---

## Usage

Run `deeplydep` by passing the target `.sln` or `.csproj` path:

```bash
deeplydep -p /path/to/solution.sln
```

If running directly from source:

```bash
dotnet run --project DeeplyDepBuilder.csproj -- -p /path/to/solution.sln
```

### CLI Options

| Option | Short | Default | Description |
|---|---|---|---|
| `--project` | `-p` | *Required* | Full path to .NET project (`.csproj`) or solution (`.sln`). |
| `--clean` | `-c` | `false` | Run `dotnet clean` instead of `dotnet build`. |
| `--generate-graph-path` | `-g` | - | Output path for generated Markdown file with Mermaid graph visualization. |
| `--hide-path-in-graph` | - | `false` | Show project file names instead of full file paths in graphs. Use only when project names are unique. |
| `--no-parallel` | - | `false` | Build projects sequentially rather than in parallel by level. |
| `--show-build-output` | - | `false` | Display console build output from `dotnet`. |
| `--verbose` | `-v` | `false` | Enable trace logging. |
| `--help` | - | - | Display help screen. |
| `--version` | - | - | Display version information. |

### Common Commands

**Build a project or solution:**
```bash
deeplydep -p /path/to/project.csproj
```

**Clean all dependencies across the graph:**
```bash
deeplydep -p /path/to/solution.sln -c
```

**Generate a dependency graph markdown file and build:**
```bash
deeplydep -p /path/to/solution.sln -g /path/to/output/graph.md
```

**Build sequentially and display dotnet build output:**
```bash
deeplydep -p /path/to/solution.sln --no-parallel --show-build-output
```

---

## Dependency Graph Examples

Sample repository for testing: [dotnet-mono-repo-example](https://github.com/Shaikh-Imran/dotnet-mono-repo-example) (for cycle detection, see the [`cyclic-dep`](https://github.com/Shaikh-Imran/dotnet-mono-repo-example/tree/cyclic-dep) branch).

### Simple Example

#### Project Structure
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

#### Topological Sort
```mermaid
flowchart TD

P4["Shared P4"] --> P3["Shared P3"]
P3 --> P2["P2"]
P2 --> P1["P1"]
P1 --> MS["my Solution"]
```

#### Topological Sort with Levels
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

---

### Complex Example

#### Project Structure
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

#### Topological Sort
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

#### Topological Sort with Level Groups
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

---

## Roadmap

- [x] Graph visualization
- [x] Graph cycle detection
- [x] Graph level grouping
- [x] Graph level-parallel build
- [x] Clean build
- [ ] Skip build if target is already up-to-date
- [ ] Path shortening in graph for cleaner visualization
- [ ] Terminal-based graph visualization

---

## License

This project is licensed under the [MIT License](LICENSE).
