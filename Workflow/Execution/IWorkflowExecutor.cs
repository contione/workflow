using Workflow.Context;

namespace Workflow.Execution;

public interface IWorkflowExecutor
{
    Task<string> ExecuteAsync(string name, WorkflowContext context);
}