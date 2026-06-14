-- =============================================================================
-- Seed: Dispositivos de prueba para IotMonitor
-- Enums:
--   DeviceCategory  -> Lectura=1, Acceso=2, Senalizacion=3, Seguridad=4, Visualizacion=5
--   ConnectionLifecycle -> AlwaysOn=1, OnDemand=2, FireAndForget=3
-- =============================================================================

USE IotMonitor;
GO

-- -----------------------------------------------------------------------------
-- 1. Tipos de dispositivo
-- -----------------------------------------------------------------------------

-- -----------------------------------------------------------------------------
-- 2. Dispositivos HTTP  (tabla Devices + HttpConfigs)
-- -----------------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM Devices WHERE Id = 'A2000002-0000-0000-0000-000000000002')
BEGIN
    INSERT INTO Devices (Id, Alias, DeviceTypeCode, IpAddress, Port, CategoryId)
    VALUES ('A2000002-0000-0000-0000-000000000002', 'Camara Cocina', 'CAMARA_HTTP', 'iotmonitor-nodered', 1880, 5);

    INSERT INTO HttpConfigs (Id, EndpointPath)
    VALUES ('A2000002-0000-0000-0000-000000000002', '/snapshot');
END

GO

-- -----------------------------------------------------------------------------
-- 3. Dispositivos Modbus  (tabla Devices + ModbusConfigs)
-- -------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM Devices WHERE Id = 'B2000003-0000-0000-0000-000000000003')
BEGIN
    INSERT INTO Devices (Id, Alias, DeviceTypeCode, IpAddress, Port, CategoryId)
    VALUES ('B2000003-0000-0000-0000-000000000003', 'Sensor Humo Cocina', 'SENSOR_MODBUS', 'iotmonitor-nodered', 5001, 4);

    INSERT INTO ModbusConfigs (Id, UnitId, PrimaryCoil)
    VALUES ('B2000003-0000-0000-0000-000000000003', 3, 10);
END

GO

-- -----------------------------------------------------------------------------
-- 4. Dispositivos UDP  (tabla Devices + UdpDevices)
-- -----------------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM Devices WHERE Id = 'C2000004-0000-0000-0000-000000000004')
BEGIN
    INSERT INTO Devices (Id, Alias, DeviceTypeCode, IpAddress, Port, CategoryId)
    VALUES ('C2000004-0000-0000-0000-000000000004', 'Señalización cocina', 'CARTEL_UDP', 'iotmonitor-nodered', 5002, 3);

    INSERT INTO UdpDevices (Id)
    VALUES ('C2000004-0000-0000-0000-000000000004');
END

GO

