# Console Monitor Devices

## 1. Resumen

**Console Monitor Devices** es un orquestador de infraestructura *stateful* que centraliza el monitoreo y control de dispositivos industriales y de seguridad. La aplicación utiliza **Spectre.Console** para ofrecer una interfaz en la terminal, comunicándose con un ecosistema simulado en **Node-RED**.

---

## 2. Arquitectura

El sistema gestiona tres tipos de ciclos de vida de comunicación según la naturaleza del dispositivo:

| **Tipo de Conexión** | **Protocolo** | **Comportamiento** | **Tipo de dispositivo** |
| --- | --- | --- | --- |
| **Always-On** | Modbus TCP | Socket abierto permanentemente con polling activo. | Sensores |
| **On-Demand** | TCP / HTTP | Abre conexión, envía comando/petición y cierra. | Barreras, Semáforos, Cámara |
| **Fire & Forget** | UDP | Envía datagramas sin confirmación. | Carteles |
- **Patrón de Herencia de Datos (TPT):** La base de datos (SQL Server) debe separar la definición base de los dispositivos de sus configuraciones específicas por protocolo.
- **Polimorfismo Operativo:** El código C# debe tratar a todos los dispositivos como una clase base abstracta (`DeviceBase`) para operaciones comunes, delegando la implementación técnica a cada clase hija.
- **Segregación de Interfaces:** Las acciones específicas (ej. tomar foto vs. cambiar luz) se expondrán en la UI casteando el dispositivo a interfaces dedicadas (ej. `IPhotographic`, `ILightController`).

## 3. Requerimientos Funcionales (RF)

### RF1: Gestión de Dispositivos

- **RF1.1 Catálogo de Tipos (`DeviceTypes`):** Debe existir una tabla inmutable usando códigos alfanuméricos como llave primaria o identificador (ej. `SENSOR_MODBUS`, `CAMARA_HTTP`, `CARTEL_UDP`). Define el protocolo y el ciclo de vida (Persistente vs. Efímero).
- **RF1.2 Tabla Base (`Devices`):** Almacena los datos comunes: `Id`, `Alias`, `DeviceTypeCode`, `IPAddress`, `Port`, `CategoryId`.
- **RF1.3 Tablas de Configuración Específica:**
    - `ModbusConfigs`: Almacena `UnitId`, `PrimaryCoil`, etc. Vinculada por FK a `Devices`.
    - `HttpConfigs`: Almacena `EndpointPath` (ej. `/camara/foto`). Vinculada por FK a `Devices`.

### RF2: Monitor de Estado (Dashboard)

- **RF2.1 Visualización Stateful:** La pantalla principal debe mostrar una tabla dinámica con el estado de salud de los dispositivos.
- **RF2.2 Definición de Estados:**
    - **🟢 Conectado:** Socket activo (Persistente) o última operación exitosa (Efímero).
    - **🔴 Desconectado:** Fallo de conexión o socket cerrado por error.
    - **🟡 Desconocido:** Timeout detectado o estado "zombie" (sin respuesta a handshakes).
- **RF2.3 Estampado de Tiempo:** Mostrar la "Última Conexión" exitosa para cada nodo.

## RF3: Control de Dispositivos por Categoría

Todos los dispositivos deben implementar un método base `TestConnectionAsync()`, pero su comportamiento variará drásticamente por protocolo.

### RF3.1 Cámara de Seguridad (HTTP - Efímero)

- **Test de Conexión:** Ejecución de un request `HTTP HEAD` o `GET` rápido validando un `StatusCode 200 OK`.
- **Acción Específica (Tomar Foto):** Petición `GET` al endpoint de Node-RED. El sistema debe descargar el *stream* de bytes, guardarlo localmente en disco como archivo de imagen (`.jpg`/`.png`) y notificar el éxito en la consola.

### RF3.2 Barrera de Acceso (Modbus TCP - Efímero)

- **Test de Conexión:** Intento de apertura de socket TCP (`TcpClient.ConnectAsync`).
- **Acción Específica (Activar):** Abrir socket, enviar trama Modbus *Write Single Coil* al registro correspondiente para cambiar el estado a ON/OFF, y cerrar socket inmediatamente.

### RF3.3 Semáforo (Modbus TCP - Efímero)

- **Test de Conexión:** Intento de apertura de socket TCP.
- **Acción Específica (Control de Luces):** Interfaz que permita elegir Verde, Amarillo o Rojo. Al seleccionar un color, el sistema debe enviar el *Coil* de encendido para ese color y los *Coils* de apagado para los otros dos, garantizando exclusión mutua. Cierra socket al terminar.

### RF3.4 Sensor (Modbus TCP - Persistente)

- **Test de Conexión:** Intento de apertura de socket TCP.
- **Acción Específica (Monitor en Vivo):** Al ingresar, la consola debe mostrar un bucle reactivo (`LiveDisplay`) que refleje los cambios de estado del sensor en Node-RED en tiempo real.
- **Acción Específica (Reconexión):** Debe existir un atajo por teclado para forzar el reinicio del socket del *worker* TCP asociado al sensor sin reiniciar la aplicación completa.

### RF3.5 Cartel LED (UDP - Unidireccional)

- **Test de Conexión:** Ejecución de un **Ping ICMP** a la IP objetivo. Al no tener estado, UDP no admite prueba de *handshake*.
- **Acción Específica (Enviar Mensaje):** Captura de texto mediante *prompt* en consola, conversión a *bytes* y envío de datagrama UDP.

### RF4: Gestión de Trazabilidad (Logging)

- **RF4.1 Switch de Destino:** Configuración dinámica para elegir el destino de los logs:
    - **Archivo:** Escritura local en formato `.log`.
    - **APM:** Envío de eventos a **New Relic** (Ingest API).
- **RF4.2 Limpieza de Consola:** La interfaz Spectre no debe mostrar logs de depuración para evitar interferir con el Dashboard visual.

---

## 4. Requerimientos de Interfaz (UX/UI)

- **RF5.1 Navegación Jerárquica:** El usuario debe seleccionar primero una **Categoría** y luego el **Dispositivo** específico para acceder a sus controles.
- **RF5.2 Feedback en Vivo:** Uso de componentes `Live` de Spectre para que la tabla de estados se actualice sin refrescar toda la pantalla.
- **RF5.3 Validación de Comandos:** El sistema debe solicitar confirmación o mostrar el resultado del comando (Éxito/Fallo) en el panel de control táctico.
- 
- **RF4.1 Filtro por Categoría:** Antes de listar dispositivos para control, el sistema debe presentar un menú de categorías (`Lectura`, `Acceso`, `Señalización`, `Seguridad`, `Visualización`).
- **RF4.2 Menú de Acciones Dinámico:** Al seleccionar un dispositivo, el menú táctico debe renderizarse evaluando las interfaces implementadas por el objeto en memoria (ej. si implementa `IPhotographic`, mostrar el botón "Tomar Foto").
- **RF4.3 Dashboard Stateful:** La vista principal (fuera de las opciones de control) debe ser una tabla de solo lectura que indique el estado real (`Conectado`, `Desconectado`, `Desconocido`) de los sockets persistentes. Los efímeros se mostrarán como `Standby` o `Ready`.

---

## 5. Requerimientos No Funcionales

- **RNF1 (Observabilidad):** El sistema contará con un selector de logs configurado por ambiente. Modo local (`.log` mediante Serilog/NLog) y Modo APM (envío de telemetría a New Relic).
- **RNF2 (Graceful Shutdown):** La interrupción de la consola (`Ctrl+C`) debe interceptarse para asegurar que todos los hilos *workers* de los Sensores (Persistentes) envíen el fin de la transmisión TCP de forma ordenada y liberen los recursos del sistema operativo antes de cerrar el proceso.