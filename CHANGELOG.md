# Changelog

All notable changes to the Misaki Graph Processor package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2024-01-XX

### Added
- **Core Graph Processing System**
  - `ExecutableGraphProcessor` class with topological sorting algorithm
  - `IGraphProcessor` interface for extensible graph processing
  - `IBuildOption` interface for configurable build parameters
  - Support for both forward and backward graph flow processing

- **Node System**
  - `DataNode` abstract base class implementing `IPortValueContainer`
  - `ExecutableNode` abstract base class extending `DataNode` with `IExecutable`
  - `IPortValueContainer` interface for port value management
  - `IExecutable` interface for node execution logic
  - Type-safe port value retrieval with `GetInputPortValue<T>()`
  - Generic port value management with `GetPortValue()` and `SetPortValue()`

- **Graph Flow Control**
  - `GraphFlow` enum supporting Forward and Backward processing modes
  - Automatic dependency resolution using depth-first search
  - Circular dependency detection and prevention
  - Master node configuration for execution starting points

- **Port Management**
  - Seamless data flow between connected nodes
  - Type safety for port connections
  - Default value support for input ports
  - Automatic port value propagation during execution

- **Sample Implementation**
  - `MathGraph` sample demonstrating complete graph workflow
  - `AddNode` sample showing basic arithmetic operations
  - `OutputNode` sample for result display and logging
  - Asset creation menu integration with Unity's project browser
  - Automatic graph execution on change detection

- **Performance Optimizations**
  - Iterative depth-first search to prevent stack overflow on large graphs
  - Efficient port pooling for connection traversal
  - Minimal memory allocations during graph execution
  - Capacity pre-allocation for processed node collections

- **Error Handling & Validation**
  - Comprehensive circular dependency detection
  - Master node validation against graph contents
  - Type safety checks for port value operations
  - Informative exception messages with context