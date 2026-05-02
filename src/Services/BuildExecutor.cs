using System.Diagnostics;
using CodersTea.DeeplyDep.Models;
using CodersTea.DeeplyDep.Utils;

namespace CodersTea.DeeplyDep.Services;

public class BuildExecutor
{
    private static string DotnetAction(CliOptions options) => options.Clean ? "clean" : "build";

    public async Task BuildProjects(List<List<Node>> projects, CliOptions options)
    {
        var dotnetActionMsg = DotnetAction(options) + "ing";

        Logger.Info($"Starting dotnet {DotnetAction(options)} for all projects in the graph");

        Logger.Info(options.NoParallelBuild
            ? $"{dotnetActionMsg} Project in Sequential Mode."
            : $"{dotnetActionMsg} projects in parallel mode. Please note that Output may get jumbled.");

        var i = -1;
        foreach (var level in projects)
        {
            i++;
            Logger.Info($"{dotnetActionMsg} Level {i} with {level.Count} projects");

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

            Logger.Info($"Completed {dotnetActionMsg} Level {i} with {level.Count} projects");
        }

        Logger.Info("Completed Build Execution for all projects in the graph");
    }

    private static async Task BuildProject(Node node, CliOptions options)
    {
        Logger.Trace($"{DotnetAction(options)}ing project: {node.Name} at path: {node.FullPath}");
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"{DotnetAction(options)} {node.FullPath}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();

        await StreamOutputAndExit(process, options);

        if (process.ExitCode != 0)
            Logger.Error(
                $"{DotnetAction(options)}ing  Failed (ExitCode: {process.ExitCode}). Use the -v flag or build individually");

        Logger.Trace($"{DotnetAction(options)}ing Completed for: {node.Name} at path: {node.FullPath}");
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