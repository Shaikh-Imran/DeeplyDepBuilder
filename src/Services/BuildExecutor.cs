using System.Diagnostics;
using CodersTea.DeeplyDep.Models;
using CodersTea.DeeplyDep.Utils;

namespace CodersTea.DeeplyDep.Services;

public class BuildExecutor
{
    public async Task BuildProjects(List<List<Node>> projects, CliOptions options)
    {
        Logger.Info("Starting Build Execution for all projects in the graph");

        Logger.Info(options.NoParallelBuild
            ? "Building Project in Sequential Mode."
            : "Building projects in parallel mode. Please note that Build Output may get jumbled.");

        var i = -1;
        foreach (var level in projects)
        {
            i++;
            Logger.Info($"Building Level {i} with {level.Count} projects");

            if (options.NoParallelBuild)
            {
                foreach (var node in level)
                {
                    await BuildProject(node, options);
                }
            }
            else
            {
                await Task.WhenAll(level.Select(n => BuildProject(n, options)));
            }
            
            Logger.Info($"Completed Building Level {i} with {level.Count} projects");
        }

        Logger.Info("Completed Build Execution for all projects in the graph");
    }

    private static async Task BuildProject(Node node, CliOptions options)
    {
        Logger.Trace($"Building project: {node.Name} at path: {node.FullPath}");
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build {node.FullPath}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();

        await StreamOutputAndExit(process, options);

        if (process.ExitCode != 0)
            Logger.Error($"Build Failed (ExitCode: {process.ExitCode}). Use the -v flag or build individually");

        Logger.Trace($"Building Completed for: {node.Name} at path: {node.FullPath}");
    }

    private static async Task StreamOutputAndExit(Process process, CliOptions options)
    {
        var outputTask = Task.Run(async () =>
        {
            if (!options.ShowBuildOutput)
            {
                await Task.CompletedTask;
                return;
            }

            while (!process.StandardOutput.EndOfStream)
            {
                var line = await process.StandardOutput.ReadLineAsync();
                if (line != null)
                    Logger.Info($"[Process]: {line}");
            }
        });

        var errorTask = Task.Run(async () =>
        {
            while (!process.StandardError.EndOfStream)
            {
                var line = await process.StandardError.ReadLineAsync();
                if (line != null)
                    Logger.Error($"[Process]: {line}");
            }
        });

        await Task.WhenAll(
            process.WaitForExitAsync(),
            outputTask,
            errorTask);
    }
}