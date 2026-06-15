# IotMonitor - Project Context

- Regla: Siempre que te pregunte algo sobre este proyecto, responde primero con "CONTEXTO CARGADO:".

## Project Overview
**IotMonitor** is a stateful infrastructure orchestrator designed to centralize the monitoring and control of industrial and security devices. It uses a terminal-based interface powered by **Spectre.Console** and integrates with a simulated ecosystem via **Node-RED**.

### Core Architecture
The project follows a Clean Architecture approach with a focus on domain-driven communication patterns based on device lifecycles:

*   **Always-On (Modbus TCP):** Permanent socket connection with active polling (Sensors).
*   **On-Demand (TCP / HTTP):** Request-response pattern where the connection opens, executes an action, and closes (Barriers, Traffic Lights, Cameras).
*   **Fire & Forget (UDP):** One-way datagram transmission without confirmation (LED Signs).

### Key Components
*   **IotMonitor.Domain:** Core models, enums (DeviceCategory, DeviceStatus), and capability interfaces (`IPhotographic`, `IAccessController`, `ILightController`, `IUdpMessenger`).
*   **IotMonitor.Application:** Orchestration services (`DeviceOrchestrator`) and business logic.
*   **IotMonitor.Infrastructure:** Concrete implementations of protocol handlers (Modbus, HTTP, UDP) and external service integrations.
*   **IotMonitor.Data:** Entity Framework Core integration using a Table-Per-Type (TPT) inheritance strategy for device configurations.
*   **IotMonitor.Presentation:** Console UI management using Spectre.Console.
*   **IotMonitor.ConsoleApp:** Entry point and Dependency Injection configuration.

## Building and Running

### Prerequisites
*   Docker & Docker Compose
*   .NET 8 SDK (for local development)

### Commands
*   **Full Environment (Containers):** `docker compose up --build`
*   **Build Solution:** `dotnet build`
*   **Run Tests:** `dotnet test`
*   **Run Console App (Local):** `dotnet run --project src/IotMonitor.ConsoleApp/IotMonitor.ConsoleApp.csproj`

## Development Conventions

### Coding Style
*   **Dependency Injection:** Favor constructor injection and registration in `Program.cs`.
*   **Async/Await:** Use asynchronous operations for all I/O bound tasks.
*   **Error Handling:** Provide clean, user-friendly feedback in the console, avoiding technical log clutter in the UI.

### Tactical Console UX
*   **Navigation Flow:** Category -> Device -> Action.
*   **Dynamic Menus:** Menus are built at runtime based on the `Capabilities` list in `DeviceSnapshot`.
*   **Live Updates:** The main dashboard uses a state-driven approach where the table is refreshed upon receiving `StatusChanged` events.

### Testing
*   Add unit tests in `tests/IotMonitor.Tests` for new protocol handlers or orchestrator logic.
*   Use Moq for mocking dependencies like `IDeviceRepository` or `IServiceScopeFactory`.

## Infrastructure Simulation
*   **Node-RED:** Accessible at `http://localhost:1880`. It simulates the hardware responses for Modbus, HTTP, and UDP.
*   **SQL Server:** Used for persistent storage of device configurations and metadata.
