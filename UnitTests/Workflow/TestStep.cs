using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Workflow.Configuration;
using Workflow.Context;
using Workflow.Steps.Base;

namespace UnitTests.Workflow;
public class TestStep : BaseWorkflowStep
{
    public TestStep(ILoggerFactory loggerFactory, IOptionsFactory<WorkflowConfiguration> optionsFactory)
        : base(loggerFactory, optionsFactory) { }

    public override string Name => "TestStep";

    public bool ShouldExecute { get; set; } = true;

    public override ValueTask<bool> IfAsync(WorkflowContext context)
    {
        return new ValueTask<bool>(ShouldExecute);
    }

    public override Task ThenAsync(WorkflowContext context)
    {
        context.Response = "Then executed";
        return Task.CompletedTask;
    }

    public override Task OtherwiseAsync(WorkflowContext context)
    {
        context.Response = "Otherwise executed";
        return Task.CompletedTask;
    }
}
