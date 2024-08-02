using Workflow.Steps.Base;

namespace Workflow.Configuration;

public class StepConfigurator<TStep> where TStep : BaseWorkflowStep
{
    private readonly WorkflowConfigurationBuilder _builder;
    private readonly StepConfiguration _stepConfiguration;

    public StepConfigurator(WorkflowConfigurationBuilder builder, WorkflowConfiguration workflowConfiguration,
        Type handlerType)
    {
        _builder = builder;

        _stepConfiguration = workflowConfiguration.StepConfigurations
            .FirstOrDefault(h => h.CurrentStepType == handlerType) ?? new StepConfiguration
        {
            CurrentStepType = handlerType
        };

        if (!workflowConfiguration.StepConfigurations.Contains(_stepConfiguration))
            workflowConfiguration.StepConfigurations.Add(_stepConfiguration);
    }

    public StepConfigurator<TStep> Then<TNextHandler>() where TNextHandler : BaseWorkflowStep
    {
        _stepConfiguration.ThenStepType = typeof(TNextHandler);
        return this;
    }

    public StepConfigurator<TStep> Otherwise<TOtherwiseHandler>() where TOtherwiseHandler : BaseWorkflowStep
    {
        _stepConfiguration.OtherwiseStepType = typeof(TOtherwiseHandler);
        return this;
    }

    public StepConfigurator<T> Step<T>() where T : BaseWorkflowStep
    {
        return _builder.Step<T>();
    }
}