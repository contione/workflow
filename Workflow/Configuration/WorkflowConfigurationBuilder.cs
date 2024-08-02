using Workflow.Steps.Base;

namespace Workflow.Configuration;

public class WorkflowConfigurationBuilder
{
    private readonly WorkflowConfiguration _workflowConfiguration = new();

    public WorkflowConfigurationBuilder StartWith<T>() where T : BaseWorkflowStep
    {
        _workflowConfiguration.StartStepType = typeof(T);
        return this;
    }

    public StepConfigurator<T> Step<T>() where T : BaseWorkflowStep
    {
        return new StepConfigurator<T>(this, _workflowConfiguration, typeof(T));
    }

    public WorkflowConfiguration Build()
    {
        _workflowConfiguration.StepConfigurations = _workflowConfiguration.StepConfigurations
            .Where(h => h.ThenStepType != null || h.OtherwiseStepType != null)
            .ToList();
        return _workflowConfiguration;
    }
}