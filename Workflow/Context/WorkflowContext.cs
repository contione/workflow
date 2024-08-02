namespace Workflow.Context;

public class WorkflowContext
{
    public string WorkflowName { get; set; } = null!;
    public IServiceScope ServiceScope { get; set; } = null!;
    public string Request { get; set; } = null!;
    public string Response { get; set; } = null!;
}