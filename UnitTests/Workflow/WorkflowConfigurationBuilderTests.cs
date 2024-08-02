using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Workflow.Configuration;
using Workflow.Steps.Base;

namespace UnitTests.Workflow;
public class WorkflowConfigurationBuilderTests
{
    [Fact]
    public void StartWith_ShouldSetStartStepType()
    {
        // Arrange
        var builder = new WorkflowConfigurationBuilder();

        // Act
        builder.StartWith<TestStep>();
        var configuration = builder.Build();

        // Assert
        Assert.Equal(typeof(TestStep), configuration.StartStepType);
    }

    [Fact]
    public void AddStep_ShouldAddStepConfiguration()
    {
        // Arrange
        var builder = new WorkflowConfigurationBuilder();

        // Act
        builder.StartWith<TestStep>()
            .Step<TestStep>().Then<Step2>().Otherwise<Step3>();
        var configuration = builder.Build();

        // Assert
        var stepConfiguration = configuration.StepConfigurations.FirstOrDefault();
        Assert.NotNull(stepConfiguration);
        Assert.Equal(typeof(TestStep), stepConfiguration.CurrentStepType);
        Assert.Equal(typeof(Step2), stepConfiguration.ThenStepType);
        Assert.Equal(typeof(Step3), stepConfiguration.OtherwiseStepType);
    }

    // Mock steps for testing
    public class Step2 : BaseWorkflowStep
    {
        public Step2(ILoggerFactory loggerFactory, IOptionsFactory<WorkflowConfiguration> optionsFactory)
            : base(loggerFactory, optionsFactory) { }

        public override string Name => "Step2";
    }

    public class Step3 : BaseWorkflowStep
    {
        public Step3(ILoggerFactory loggerFactory, IOptionsFactory<WorkflowConfiguration> optionsFactory)
            : base(loggerFactory, optionsFactory) { }

        public override string Name => "Step3";
    }
}

