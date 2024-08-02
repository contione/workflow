using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Workflow.Configuration;
using Workflow.Context;
using Workflow.Execution;

namespace UnitTests.Workflow;
public class WorkflowExecuteTests
{
    private readonly Mock<IOptionsFactory<WorkflowConfiguration>> _optionsFactoryMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly ILogger<WorkflowExecutor> _logger;
    private readonly ILoggerFactory _loggerFactory;
    public WorkflowExecuteTests()
    {
        _optionsFactoryMock = new Mock<IOptionsFactory<WorkflowConfiguration>>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _serviceScopeMock = new();
        Mock<IServiceScopeFactory> serviceScopeFactoryMock = new();
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<WorkflowExecutor>();

        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactoryMock.Object);
        _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldExecuteHandlerSuccessfully()
    {
        // Arrange
        var context = new WorkflowContext() { WorkflowName = "test" };
        var handlerMock = new Mock<TestStep>(_loggerFactory, _optionsFactoryMock.Object) { CallBase = true };
        var config = new WorkflowConfiguration { StartStepType = typeof(TestStep) };

        _optionsFactoryMock.Setup(x => x.Create(It.IsAny<string>())).Returns(config);
        _serviceProviderMock.Setup(x => x.GetService(typeof(TestStep))).Returns(handlerMock.Object);

        var workflowExecutor = new WorkflowExecutor(_logger, _optionsFactoryMock.Object, _serviceProviderMock.Object);

        // Act
        context.ServiceScope = _serviceScopeMock.Object;
        var result = await workflowExecutor.ExecuteAsync("test", context);

        // Assert
        handlerMock.Verify(x => x.ThenAsync(It.IsAny<WorkflowContext>()), Times.Once);
        Assert.Equal("Then executed", result);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnError_WhenHandlerNotRegistered()
    {
        // Arrange
        var context = new WorkflowContext { WorkflowName = "test" };
        var config = new WorkflowConfiguration { StartStepType = null };

        _optionsFactoryMock.Setup(x => x.Create(It.IsAny<string>())).Returns(config);

        var workflowExecutor = new WorkflowExecutor(_logger, _optionsFactoryMock.Object, _serviceProviderMock.Object);

        // Act
        var result = await workflowExecutor.ExecuteAsync("test", context);

        // Assert
        Assert.Equal("Unregistered handler!", result);
    }
}