using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Workflow.Configuration;
using Workflow.Context;
using Workflow.Steps.Base;

namespace UnitTests.Workflow;

public class WorkflowTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IOptionsFactory<WorkflowConfiguration>> _optionsFactoryMock;
    private readonly ILoggerFactory _loggerFactory;
    private readonly WorkflowContext _context;

    public WorkflowTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _optionsFactoryMock = new Mock<IOptionsFactory<WorkflowConfiguration>>();
        _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _context = new WorkflowContext() { WorkflowName = "test" };

        Mock<IServiceScope> serviceScopeMock = new();
        Mock<IServiceScopeFactory> serviceScopeFactoryMock = new();
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactoryMock.Object);
        serviceScopeFactoryMock.Setup(ssf => ssf.CreateScope())
            .Returns(serviceScopeMock.Object);
        serviceScopeMock.Setup(ss => ss.ServiceProvider)
            .Returns(_serviceProviderMock.Object);

        _context.ServiceScope = serviceScopeMock.Object;
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallThenAsync_WhenConditionIsTrue()
    {
        // Arrange
        var handler = new TestStep(_loggerFactory, _optionsFactoryMock.Object)
        {
            ShouldExecute = true
        };

        // Act
        await handler.ExecuteAsync(_context);

        // Assert
        Assert.Equal("Then executed", _context.Response);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallOtherwiseAsync_WhenConditionIsFalse()
    {
        // Arrange
        var handler = new TestStep(_loggerFactory, _optionsFactoryMock.Object)
        {
            ShouldExecute = false
        };

        // Act
        await handler.ExecuteAsync(_context);

        // Assert
        Assert.Equal("Otherwise executed", _context.Response);
    }

    [Fact]
    public async Task OtherwiseAsync_ShouldCallOtherwiseHandler()
    {
        // Arrange
        var otherwiseHandlerMock = new Mock<BaseWorkflowStep>(_loggerFactory, _optionsFactoryMock.Object) { CallBase = true };
        var handlerMock = new Mock<BaseWorkflowStep>(_loggerFactory, _optionsFactoryMock.Object) { CallBase = true };

        var config = new WorkflowConfiguration();
        config.StepConfigurations.Add(new StepConfiguration()
        {
            CurrentStepType = handlerMock.Object.GetType(),
            OtherwiseStepType = otherwiseHandlerMock.Object.GetType()
        });

        _optionsFactoryMock.Setup(x => x.Create(It.IsAny<string>())).Returns(config);
        _serviceProviderMock.Setup(x => x.GetService(otherwiseHandlerMock.Object.GetType())).Returns(otherwiseHandlerMock.Object);

        var handler = handlerMock.Object;

        // Act
        await handler.OtherwiseAsync(_context);

        // Assert
        otherwiseHandlerMock.Verify(x => x.ExecuteAsync(It.IsAny<WorkflowContext>()), Times.Once);
    }
}
