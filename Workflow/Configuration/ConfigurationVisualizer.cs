namespace Workflow.Configuration;

public static class ConfigurationVisualizer
{
    public static string GenerateDotContent(WorkflowConfiguration configuration)
    {
        using var writer = new StringWriter();
        writer.WriteLine("digraph G {");

        // 设置节点的默认形状
        writer.WriteLine("node [shape=box];");

        // 输出开始节点，单独设置其形状为椭圆以便区分
        if (configuration.StartStepType != null)
        {
            writer.WriteLine("  start [shape=ellipse, label=\"start\"];");
            writer.WriteLine($"  start -> {configuration.StartStepType.Name};");
        }

        foreach (var handlerConfig in configuration.StepConfigurations)
        {
            // 设置有 Then 和 Otherwise 逻辑分支的节点为菱形
            if (handlerConfig.ThenStepType != null && handlerConfig.OtherwiseStepType != null)
                writer.WriteLine($"{handlerConfig.CurrentStepType.Name} [shape=diamond];");

            if (handlerConfig.ThenStepType != null)
                writer.WriteLine(
                    $"  {handlerConfig.CurrentStepType.Name} -> {handlerConfig.ThenStepType.Name} [label=\"Then\"];");
            if (handlerConfig.OtherwiseStepType != null)
                writer.WriteLine(
                    $"  {handlerConfig.CurrentStepType.Name} -> {handlerConfig.OtherwiseStepType.Name} [label=\"Otherwise\"];");
        }

        writer.WriteLine("}");

        return writer.ToString();
    }

    public static void GenerateDotFile(WorkflowConfiguration configuration, string fileName)
    {
        var dotContent = GenerateDotContent(configuration);
        File.WriteAllText($"workflow_{fileName}.dot", dotContent);
    }

    public static void PrintDotContent(WorkflowConfiguration configuration)
    {
        var dotContent = GenerateDotContent(configuration);
        Console.WriteLine(dotContent);
    }
}