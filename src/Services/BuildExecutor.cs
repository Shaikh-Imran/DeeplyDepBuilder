using System.Diagnostics;
using CodersTea.DeeplyDep.Models;
using CodersTea.DeeplyDep.Utils;

namespace CodersTea.DeeplyDep.Services;

public class BuildExecutor
{
    public async Task BuildProjects(List<List<Node>> projects, CliOptions options)
    {
        Logger.Info("Starting Build Execution for all projects in the graph");

        if (options.BuildInParallel)
        {
            Logger.Info("Building projects in parallel mode");
        }

        var i = -1;
        foreach (var level in projects)
        {
            i++;
            Logger.Info($"Building Level {i} with {level.Count} projects");

            var op = new List<Task>();
            foreach (var node in level)
            {
                Logger.Info($"Building project: {node.Name} at path: {node.FullPath}");
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"build {node.FullPath}",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };
                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                Logger.Trace($"Build output for {node.Name}: {output}");

                if (options.BuildInParallel)
                {
                    op.Add(process.WaitForExitAsync());
                }
                else
                {
                    await process.WaitForExitAsync();
                }
            }

            if (options.BuildInParallel)
                await Task.WhenAll(op.ToArray());
        }

        Logger.Info("Completed Build Execution for all projects in the graph");
    }
}