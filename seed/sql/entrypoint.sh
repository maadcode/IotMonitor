#!/bin/bash

# Función para inicializar la base de datos
init_db() {
    echo "Esperando a que SQL Server inicie..."
    # Esperar hasta que sqlcmd responda
    for i in {1..30}; do
        /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "SELECT 1" > /dev/null 2>&1
        if [ $? -eq 0 ]; then
            echo "SQL Server listo. Creando base de datos..."
            /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'IotMonitor') CREATE DATABASE IotMonitor;"
            return 0
        fi
        sleep 2
    done
    echo "Error: No se pudo conectar a SQL Server."
}

# Ejecutar inicialización en segundo plano
init_db &

# Iniciar el proceso principal de SQL Server
exec /opt/mssql/bin/sqlservr