namespace Workflow.Configuration;

public class WorkflowConfiguration
{
    public Type? StartStepType { get; set; } = null!;
    public List<StepConfiguration> StepConfigurations { get; set; } = new();
}