namespace Workflow.Configuration;
public class StepConfiguration
{
    public Type CurrentStepType { get; set; } = null!;
    public Type? ThenStepType { get; set; }
    public Type? OtherwiseStepType { get; set; }
}