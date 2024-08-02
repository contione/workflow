# Workflow

This repository contains a Workflow implementation that allows users to configure, execute, and visualize workflows. The Workflow is designed to be modular and flexible, providing a fluent API for defining workflows, handling different execution branches, and logging the execution flow.

## Features

- **Modular Design**: The Workflow is built with a modular architecture, making it easy to extend and maintain.
- **Fluent API**: Configure workflows with a clear and concise API.
- **Execution Context**: Manage workflow execution with context passing.
- **Logging**: Integrated logging to help with debugging and monitoring.
- **Visualization**: Generate visual representations of the workflow using Graphviz.

## Project Structure

- **Configuration**: Contains classes and builders for defining workflows.
- **Context**: Manages the execution context for workflows.
- **Execution**: Contains classes for executing the workflows.
- **Steps**: Defines the base Step and specific workflow Steps.
- **Visualizer**: Generates visual representations of the workflow configuration.

## Getting Started

### Prerequisites

- .NET Core 8.0 or later
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Logging
- Microsoft.Extensions.Options

### Installation

Clone the repository and navigate to the project directory:

```bash
git clone <repository-url>
cd Workflow
```
### Configuration
Define your workflow configuration in the Configuration directory. The WorkflowConfiguration class stores the workflow configuration, including the start Step and Step configurations.

### Example Configuration

```c#
using Workflow.Configuration;

builder.Services.AddWorkflow("Example", configure =>
{
    configure
        .StartWith<Step1>()
        .AddStep<Step1>().Then<Step2>()
        .AddStep<Step2>().Then<Step3>()
        .AddStep<Step3>().Then<Step4>().Otherwise<Step5>();
});

```

### Execution
Use the WorkflowExecutor to execute the configured workflow. The executor resolves and invokes the appropriate Steps based on the defined workflow.

### Example Execution
```c#
using Workflow.Execution;
using Workflow.Context;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = new ServiceCollection()
    .AddLogging()
    .AddOptions()
    .AddWorkflow("Transfer", configure =>
    {
        configure
            .StartWith<Step1>()
            .AddStep<Step1>().Then<Step2>()
            .AddStep<Step2>().Then<Step3>()
            .AddStep<Step3>().Then<Step4>().Otherwise<Step5>();
    })
    .BuildServiceProvider();

var workflowExecutor = serviceProvider.GetRequiredService<IWorkflowExecutor>();
var context = new WorkflowContext();
var result = await workflowExecutor.ExecuteAsync("Example", context);

Console.WriteLine(result);

```

### Steps
Steps are the core of the workflow execution. Each Step inherits from the WorkflowStep base class and implements the necessary logic for processing the workflow step.

### Example Step
```C#
using Workflow.Steps.Base;
using Workflow.Context;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class Step1 : BaseWorkflowStep
{
    public Step1(ILoggerFactory loggerFactory, IOptionsFactory<WorkflowConfiguration> optionsFactory)
        : base(loggerFactory, optionsFactory)
    {
    }

    public override string Name => "Step1";

    public override async Task ExecuteAsync(WorkflowContext context)
    {
        // Your execution logic here
        await base.ExecuteAsync(context);
    }
}
```

### Visualization
Generate a visual representation of the workflow configuration using the ConfigurationVisualizer class.

### Example Visualization
```c#
using Workflow.Configuration;
using Workflow;

var workflowConfiguration = new WorkflowConfigurationBuilder()
    .StartWith<Step1>()
    .AddStep<Step1>().Then<Step2>()
    .AddStep<Step2>().Then<Step3>()
    .AddStep<Step3>().Then<Step4>().Otherwise<Step5>()
    .Build();

ConfigurationVisualizer.GenerateDotFile(workflowConfiguration, "workflow.dot");

```
Use Graphviz to render the DOT file:
```
dot -Tpng workflow.dot -o workflow.png
```

### Conclusion
This Workflow Workflow provides a flexible and modular approach to defining, executing, and visualizing workflows. By following the examples provided, you can quickly set up and customize workflows to meet your specific needs. For more detailed documentation, please refer to the source code and comments within the project.

### License
This project is licensed under the MIT License.
```c#
This `README.md` provides a comprehensive introduction to the project, including the project structure, installation instructions, configuration examples, execution examples, and visualization instructions. Feel free to customize it further based on your specific requirements or preferences.
```