namespace Workflow.Configuration;

public static class WorkflowExtensions
{
    public static IServiceCollection AddWorkflow(this IServiceCollection services, string workflowName,
        Action<WorkflowConfigurationBuilder> configure)
    {
        var builder = new WorkflowConfigurationBuilder();
        configure(builder);
        var workflowConfiguration = builder.Build();

        ValidateConfiguration(workflowConfiguration);

        ConfigurationVisualizer.GenerateDotFile(workflowConfiguration, workflowName);

        services.PostConfigure<WorkflowConfiguration>(workflowName, options =>
        {
            options.StartStepType = workflowConfiguration.StartStepType;
            options.StepConfigurations = workflowConfiguration.StepConfigurations;
        });
        return services;
    }

    private static void ValidateConfiguration(WorkflowConfiguration configuration)
    {
        if (configuration.StartStepType == null)
            throw new InvalidOperationException("Start step type must be specified.");

        foreach (var handlerConfig in configuration.StepConfigurations)
            if (handlerConfig.CurrentStepType == null)
                throw new InvalidOperationException("Step type must be specified.");
    }
}