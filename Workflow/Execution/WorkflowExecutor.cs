using Workflow.Configuration;
using Workflow.Context;
using Workflow.Steps.Base;

namespace Workflow.Execution;

public class WorkflowExecutor : IWorkflowExecutor
{
    private readonly ILogger<WorkflowExecutor> _logger;
    private readonly IOptionsFactory<WorkflowConfiguration> _optionsFactory;
    private readonly IServiceProvider _serviceProvider;

    public WorkflowExecutor(ILogger<WorkflowExecutor> logger, IOptionsFactory<WorkflowConfiguration> optionsFactory,
        IServiceProvider serviceProvider)
    {
        _optionsFactory = optionsFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(string name, WorkflowContext context)
    {
        try
        {
            context.WorkflowName = name;
            var executeHandlerType = _optionsFactory.Create(context.WorkflowName).StartStepType;
            if (executeHandlerType == null) throw new InvalidOperationException("Unregistered handler!");

            using var scope = _serviceProvider.CreateScope();
            context.ServiceScope = scope;
            var executeHandler = scope.ServiceProvider.GetService(executeHandlerType) as BaseWorkflowStep;

            if (executeHandler == null)
                throw new InvalidOperationException(
                    $"Handler of type {executeHandlerType.Name} could not be resolved.");

            await executeHandler.ExecuteAsync(context);
            return context.Response;
        }
        catch (Exception ex)
        {
            context.Response = ex.Message;
            _logger.LogError(ex, $"{name} {nameof(ExecuteAsync)} Error: {ex.Message}");
            return context.Response;
        }
    }
}