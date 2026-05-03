# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2026-05-03

### Added
- Initial release of DeeplyDepBuilder.
- Command-line tool to build complete project reference dependency graphs.
- Cycle detection in dependencies.
- Topological sort with levels for identifying independent build stages.
- Generation of Mermaid graphs for visual dependency representation.
- Parallel building of projects on the same topological level to speed up the process.
- `dotnet clean` support for the entire graph.
- Support to install and run as a global `dotnet tool` via the `deeplydep` command.
