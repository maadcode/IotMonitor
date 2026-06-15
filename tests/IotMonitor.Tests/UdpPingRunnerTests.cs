using IotMonitor.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace IotMonitor.Tests;

[TestClass]
public class UdpPingRunnerTests
{
    private Mock<IServiceScopeFactory> _scopeFactoryMock = null!;
    private Mock<ILogger> _loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _loggerMock = new Mock<ILogger>();
    }

    [TestMethod]
    public async Task RunAsync_ShouldExecuteLoop()
    {
        
        var deviceId = Guid.NewGuid();
        var orchestrator = new DeviceOrchestrator(_scopeFactoryMock.Object);
        var runner = new UdpPingRunner(deviceId, "127.0.0.1", orchestrator, _loggerMock.Object);

        using var cts = new CancellationTokenSource();
        
        
        var runTask = runner.RunAsync(cts.Token);
        
        
        await Task.Delay(100);
        cts.Cancel();
    }
}
