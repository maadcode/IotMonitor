# Fire & Forget (UDP) Sequence Diagrams

The Fire & Forget pattern is used for devices where low latency is preferred over delivery guarantees, such as LED Message Signs.

## Status Monitoring (DNS Ping)

Since UDP is connectionless, reachability is estimated via DNS resolution.

```mermaid
sequenceDiagram
    participant Runner as UdpPingRunner
    participant DNS as DNS Resolver
    participant Orchestrator as DeviceOrchestrator
    participant Console as Console UI (Dashboard)

    loop Every 10 seconds
        Runner->>DNS: GetHostAddressesAsync(IP)
        DNS-->>Runner: IP Addresses Found
        Runner->>Orchestrator: UpdateStatus(Connected)
        Orchestrator-->>Console: StatusChanged Event
    end

    Note over Runner, DNS: On Failure
    Runner->>DNS: GetHostAddressesAsync(IP)
    DNS-->>Runner: No Addresses/Error
    Runner->>Orchestrator: UpdateStatus(Disconnected)
    Orchestrator-->>Console: StatusChanged Event
```

## Command Execution (Send Message)

Datagrams are sent without awaiting a response from the device.

```mermaid
sequenceDiagram
    participant User as User (Console)
    participant Service as UdpDeviceSender
    participant Device as UdpDevice (Infrastructure)
    participant Client as UdpClient
    participant HW as LED Sign (Node-RED)

    User->>Service: SendMessageAsync(id, message)
    Service->>Device: SendMessageAsync(message)
    Device->>Client: Connect(IP, Port)
    Device->>Client: SendAsync(UTF8 bytes)
    Client->>HW: UDP Datagram
    Note right of HW: Device receives but doesn't ACK
    Device->>Client: Close/Dispose
    Device-->>Service: Success
    Service-->>User: Feedback in Console
```
