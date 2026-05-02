## DeeplyDepBuilder

Deeply Dependency Builder is a command lime tool to build the complete prject reference depenndency graph of the solution or project.
In big projects or monorepos, the dependecy chain can be very deep. The usual dotnet build only builds your current project and not referenced package.
Visual Studio had option to do that I guess, but not in VsCode or Rider.

The Features of this tool
- Identifies Dependency Cycle
- Create Mermaid Graph as your solution as Root.
- Uses Topological Sort with levels. Each level represents independent projects.
- Each level can be build parallely for speeding up build process.
- Levels and topo sort is also shown in graph.
- Can clean the build of the whole graph.


## Installing as a dotnet tool

```bash
dotnet pack
dotnet tool install --global --add-source ./nupkg deeplydepbuilder

# Once installed run the command
deeplydep --help
```
> If you are using it alot, consider generate a alias as `dpd`

## Usage

You can run the tool by passing the required project or solution path. 

```bash
deeplydep -p /path/to/your/project.csproj
```


if running from the source:
```bash
dotnet run -- -p <Path-to-Solution-Or-Project>
```

### CLI Options

**Required**:

```bash
-p, --project                Required. .Net Project or Solution Full Path
```

**Optional**:

```
  -v, --verbose                Add Trace Logging

  -c, --clean                  (Default: false) Do Dotnet Clean instead of Dotnet Build

  -g, --generate-graph-path    Generate markdown file for graph visualization in given path

  --hide-path-in-graph         (Default: false) In visual graph, if false shows path otherwise File name. Use only when names are unique

  --no-parallel                (Default: false) If false builds projects in the same level in parallelly otherwise sequentially

  --show-build-output          (Default: false) If True shows the build output in the console.

  --help                       Display this help screen.

  --version                    Display version information.
  
  ```

## Example with Graphs

 This Repo: https://github.com/Shaikh-Imran/dotnet-mono-repo-example is generated based on this graph.

### Simple Example

Project Structure
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


Topological Sort
```mermaid
flowchart TD

MS["my Solution"] --> P1["P1"]
P1 --> P2["P2"]
P2 --> P3["Shared P3"]
P3 --> P4["Shared P4"]
```


Topological Sort with Levels
```mermaid
flowchart TD

subgraph Level0
MS["my Solution"]
end

subgraph Level1
P1["P1"]
P2["P2"]
end

subgraph Level2
P3["Shared P3"]
end

subgraph Level3
P4["Shared P4"]
end

MS --> P1
MS --> P2
MS --> P4

P2 --> P3
P3 --> P4
```
### Complex Example
Project Structure

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

Topological Sort

```mermaid
flowchart TD

MS["my Solution"] --> P8["P8"]
P8 --> P1["P1"]
P1 --> P3["P3"]
P3 --> P2["P2"]
P2 --> P4["Common P4"]
P4 --> P5["Common P5"]
P5 --> P6["Shared P6"]
P6 --> P7["Shared P7"]
```

Topological Sort with Level Groups

```mermaid
flowchart TD

subgraph Level0["Level 0 - Root"]
MS["my Solution"]
end

subgraph Level1["Level 1"]
P1["P1"]
P3["P3"]
P8["P8"]
P4["Common P4"]
end

subgraph Level2["Level 2"]
P2["P2"]
end

subgraph Level3["Level 3"]
P5["Common P5"]
P6["Shared P6"]
end

subgraph Level4["Level 4 - Leaf"]
P7["Shared P7"]
end

MS --> P1
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

### Examples

Use this https://github.com/Shaikh-Imran/dotnet-mono-repo-example solution for testing.

**Build a project:**
```bash
deeplydep -p /path/to/your/project.csproj
```

**Clean a solution instead of building:**
```bash
deeplydep -p /path/to/your/solution.sln -c
```

**Generate a dependency graph markdown file and build:**
```bash
deeplydep -p /path/to/your/solution.sln -g /path/to/output/graph.md
```

**Build sequentially and show dotnet build output:**
```bash
deeplydep -p /path/to/your/solution.sln --no-parallel --show-build-output
```

