IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'IotMonitor')
BEGIN
    CREATE DATABASE IotMonitor;
END
GO