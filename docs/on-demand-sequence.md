# On-Demand (HTTP) Sequence Diagrams

The On-Demand pattern is used for devices that follow a request-response cycle over HTTP, such as IP Cameras.

## Status Monitoring (Ping)

A background runner periodically checks if the device is reachable.

```mermaid
sequenceDiagram
    participant Runner as HttpPingRunner
    participant Client as HttpClient
    participant Device as HTTP Device (Node-RED)
    participant Orchestrator as DeviceOrchestrator
    participant Console as Console UI (Dashboard)

    loop Every 15 seconds
        Runner->>Client: SendAsync(HEAD request)
        Client->>Device: HTTP HEAD /
        Device-->>Client: 200 OK
        Client-->>Runner: HttpResponse (Success)
        Runner->>Orchestrator: UpdateStatus(Ready)
        Orchestrator-->>Console: StatusChanged Event
    end

    Note over Runner, Device: On Failure
    Runner->>Client: SendAsync(HEAD request)
    Client-XDevice: Timeout or Refused
    Runner->>Orchestrator: UpdateStatus(Disconnected)
    Orchestrator-->>Console: StatusChanged Event
```

## Command Execution (Capture Photo)

The action is executed by opening a connection, retrieving data, and closing it.

```mermaid
sequenceDiagram
    participant User as User (Console)
    participant Service as PhotoCaptureService
    participant Device as HttpDevice (Infrastructure)
    participant HW as Physical Camera (Node-RED)
    participant FS as File System

    User->>Service: CapturePhotoAsync(id)
    Service->>Device: CapturePhotoAsync(targetPath)
    Device->>HW: HTTP GET /photo
    HW-->>Device: 200 OK (Binary Data)
    Device->>FS: Save to disk
    FS-->>Device: File saved
    Device-->>Service: targetFilePath
    Service-->>User: Display image path
```
