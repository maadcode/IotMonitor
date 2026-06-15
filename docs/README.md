# IotMonitor Sequence Diagrams

This directory contains the sequence diagrams for the different device communication patterns used in the project.

## Device Types

- [Always-On (Modbus TCP)](always-on-sequence.md): Persistent connections for sensors and active control.
- [On-Demand (HTTP)](on-demand-sequence.md): Request-response patterns for cameras and web-based devices.
- [Fire & Forget (UDP)](fire-and-forget-sequence.md): One-way datagram transmission for message signs.

## Architecture Overview

The system uses a **Stateful Orchestrator** (`DeviceOrchestrator`) that maintains the runtime status of all devices. 

- **Background Runners** update the status in real-time or via periodic pings.
- **Infrastructure Services** execute commands by interacting with the protocol handlers.
- **Console UI** reacts to status change events to provide a live dashboard.
