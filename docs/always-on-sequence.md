# Always-On (Modbus TCP) Sequence Diagrams

The Always-On pattern is used for devices that maintain a persistent TCP connection, such as industrial sensors and controllers.

## Status Monitoring (Sensor)

This flow shows how the system maintains a persistent connection and receives real-time updates from a sensor.

```mermaid
sequenceDiagram
    participant Worker as AlwaysOnDeviceRunner
    participant DNS as DNS Resolver
    participant Device as Modbus Device (Node-RED)
    participant Orchestrator as DeviceOrchestrator
    participant Console as Console UI (Dashboard)

    Note over Worker, Device: Persistent Connection Loop
    Worker->>DNS: GetHostAddressesAsync(IP)
    DNS-->>Worker: Resolved IP
    Worker->>Device: ConnectAsync(IP, Port)
    Device-->>Worker: TCP Connected
    Worker->>Orchestrator: UpdateStatus(Ready)
    Orchestrator-->>Console: StatusChanged Event (Ready)
    
    Worker->>Device: Send Registration Byte (0x00)
    Note right of Device: Node-RED registers client session

    loop Every status change
        Device->>Worker: Broadcast Byte (0x01 = ON / 0x00 = OFF)
        Worker->>Orchestrator: UpdateStatus(Connected/Disconnected)
        Orchestrator-->>Console: StatusChanged Event (Refresh UI)
    end

    Note over Worker, Device: On Connection Drop
    Device-XWorker: Socket Closed
    Worker->>Orchestrator: UpdateStatus(Unknown)
    Orchestrator-->>Console: StatusChanged Event
    Worker->>Worker: Wait ReconnectDelay (5s)
```

## Command Execution (Barrier/Traffic Light)

Commands are executed on-demand, opening a temporary connection for the protocol transaction.

```mermaid
sequenceDiagram
    participant User as User (Console)
    participant Service as AccessControlService
    participant Repo as IDeviceRepository
    participant Device as ModbusDevice (Infrastructure)
    participant HW as Physical Device (Node-RED)

    User->>Service: SetAccessStateAsync(id, active)
    Service->>Repo: GetDeviceByIdAsync(id)
    Repo-->>Service: ModbusDeviceEntity
    Service->>Device: Instantiate(entity)
    Service->>Device: SetAccessStateAsync(active)
    
    Device->>HW: TCP Connect
    HW-->>Device: Connected
    Device->>HW: WriteSingleCoil(unitId, coil, value)
    HW-->>Device: ACK
    Device->>HW: Close Connection
    Device-->>Service: Success
    Service-->>User: Feedback in Console
```
