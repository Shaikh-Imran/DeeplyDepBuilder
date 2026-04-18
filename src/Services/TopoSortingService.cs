using CodersTea.DeeplyDep.Models;
using CodersTea.DeeplyDep.Utils;

namespace CodersTea.DeeplyDep.Services;

public class TopoSortingService
{
    public List<List<Node>> TopoSortWithLevels(DependencyGraph graph)
    {
        Logger.Info("Doing Topo Sort with levels on the graph");

        Logger.Trace("Setting up initial Indegree to 0 for all nodes");
        var inDegree = graph.AllNodes.ToDictionary(node => node.Key, _ => 0 );
       
        foreach (var node in graph.AllNodes)
        {
            foreach (var dep in node.Value.Dependencies)
            {
                inDegree[dep.FullPath]++;
            }
        }

        var levels = new List<List<Node>>();

        var queue = new Queue<Node>();
        queue.Enqueue(graph.RootNode); // At first itration, only the root node will have indegree 0 

        while (queue.Count > 0)
        {
            Logger.Trace($"Processing Level {levels.Count} with {queue.Count} nodes");
            var currentLevel = new List<Node>();
            var levelSize = queue.Count;
            for (int i = 0; i < levelSize; i++)
            {
                var currentNode = queue.Dequeue();
                currentLevel.Add(currentNode);
                foreach (var dep in currentNode.Dependencies)
                {
                    inDegree[dep.FullPath]--;
                    if (inDegree[dep.FullPath] == 0)
                    {
                        queue.Enqueue(dep);
                    }
                }
            }

            levels.Add(currentLevel);
            Logger.Trace($"Completed Level {levels.Count - 1} with {currentLevel.Count} nodes");
        }

        Logger.Info($"Completed Topo Sort with Total Levels: {levels.Count} for {inDegree} for {inDegree.Count} nodes");
        // levels.Reverse(); // as we we want to build from leaf to root as per our dependency graph
        return levels;
    }
}