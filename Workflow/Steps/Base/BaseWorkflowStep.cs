using Workflow.Configuration;
using Workflow.Context;

namespace Workflow.Steps.Base;
public abstract class BaseWorkflowStep
{
    private readonly ILogger _logger;
    private readonly IOptionsFactory<WorkflowConfiguration> _optionsFactory;

    protected BaseWorkflowStep(ILoggerFactory loggerFactory, IOptionsFactory<WorkflowConfiguration> optionsFactory)
    {
        _optionsFactory = optionsFactory;
        _logger = loggerFactory.CreateLogger(GetType());
    }

    public abstract string Name { get; }

    public virtual ValueTask<bool> IfAsync(WorkflowContext context)
    {
        return new ValueTask<bool>(true);
    }

    public virtual async Task ThenAsync(WorkflowContext context)
    {
        await ExecuteNextStepAsync(context, true);
    }

    public virtual async Task OtherwiseAsync(WorkflowContext context)
    {
        await ExecuteNextStepAsync(context, false);
    }

    public virtual async Task ExecuteAsync(WorkflowContext context)
    {
        var condition = await IfAsync(context);
        if (condition)
            await ThenAsync(context);
        else
            await OtherwiseAsync(context);
    }

    private async Task ExecuteNextStepAsync(WorkflowContext context, bool condition)
    {
        try
        {
            var currentHandlerConfig = _optionsFactory.Create(context.WorkflowName).StepConfigurations
                .FirstOrDefault(o => o.CurrentStepType == GetType());

            if (currentHandlerConfig == null)
            {
                _logger.LogWarning($"No step configuration found for {GetType().Name}");
                return;
            }

            var nextHandlerType =
                condition ? currentHandlerConfig.ThenStepType : currentHandlerConfig.OtherwiseStepType;
            if (nextHandlerType == null)
            {
                _logger.LogInformation("Execution ended.");
                return;
            }

            var nextHandler = context.ServiceScope.ServiceProvider.GetService(nextHandlerType);
            if (nextHandler is BaseWorkflowStep handler) await handler.ExecuteAsync(context);
        }
        catch (Exception ex)
        {
            context.Response = ex.Message;
            _logger.LogError(ex, $"{Name} {nameof(ExecuteNextStepAsync)} Error: {ex.Message}");
        }
    }
}