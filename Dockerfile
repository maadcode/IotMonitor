FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
ARG APP_PROJECT=src/IotMonitor.ConsoleApp/IotMonitor.ConsoleApp.csproj

WORKDIR /src

COPY ["src/IotMonitor.ConsoleApp/IotMonitor.ConsoleApp.csproj", "src/IotMonitor.ConsoleApp/"]
COPY ["src/IotMonitor.Presentation/IotMonitor.Presentation.csproj", "src/IotMonitor.Presentation/"]
COPY ["src/IotMonitor.Application/IotMonitor.Application.csproj", "src/IotMonitor.Application/"]
COPY ["src/IotMonitor.Domain/IotMonitor.Domain.csproj", "src/IotMonitor.Domain/"]
COPY ["src/IotMonitor.Infrastructure/IotMonitor.Infrastructure.csproj", "src/IotMonitor.Infrastructure/"]
COPY ["src/IotMonitor.Data/IotMonitor.Data.csproj", "src/IotMonitor.Data/"]

RUN dotnet restore "$APP_PROJECT"

COPY . .
RUN dotnet publish "$APP_PROJECT" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0-bookworm-slim AS final
WORKDIR /app

ENV DOTNET_EnableDiagnostics=0
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV IOTMONITOR__LOGGING__DESTINATION=Console

ARG APP_UID=64198
USER $APP_UID

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet"]
CMD ["IotMonitor.ConsoleApp.dll"]
