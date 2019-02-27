using Burton.Lib.Graph;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnityDistanceHeuristic : IHeuristic<SparseGraph<NavGraphNode, GraphEdge>>
{
    public double Calculate(SparseGraph<NavGraphNode, GraphEdge> graph, int srcNodeIndex, int destNodeIndex)
    {

        var srcNode = graph.GetNode(srcNodeIndex);
        var destNode = graph.GetNode(destNodeIndex);

        var srcPos = srcNode.position;
        var destPos = destNode.position;

        int dx =(int) Mathf.Abs(srcPos.x - destPos.x);
        int dy = (int)Mathf.Abs(srcPos.z - destPos.z);

        // the min of the deltas is the diagonal steps
        int min = Mathf.Min(dx, dy);

        // the max of the deltas is used to calculate the straight steps,
        // which is just max - min 
        int max = Mathf.Max(dx, dy);

        int diagonalSteps = min;
        int straightSteps = max - min;

        // diagonal steps cost 1.4, straight steps cost 1.
        return 1f * diagonalSteps + straightSteps;
    }
}

public class Pathfinder : MonoBehaviour
{
    Graph m_graph;

    public List<int> pathToTarget = new List<int>();
    public int startNodeIndex = 0;
    public int goalNodeIndex = 0;

    public void SetGraph(Graph graph)
    {
        m_graph = graph;
    }

    [Button]
    public void FindPath(int startNodeIndex, int goalNodeIndex)
    {
        this.startNodeIndex = startNodeIndex;
        this.goalNodeIndex = goalNodeIndex;

        pathToTarget.Clear();

        var startNode = m_graph.GetNode(startNodeIndex);
        var goalNode = m_graph.GetNode(goalNodeIndex);

        IHeuristic<SparseGraph<NavGraphNode, GraphEdge>> Heuristic = new UnityDistanceHeuristic();

        var search = new Search_AStar<NavGraphNode, GraphEdge>(m_graph.m_sparseGraph, Heuristic, startNode.NodeIndex, goalNode.NodeIndex);

        if (search.Search())
        {
            pathToTarget.AddRange(search.GetPathToTarget());

            foreach (var idx in pathToTarget)
            {
                var node = m_graph.m_sparseGraph.GetNode(idx);

                if (node != null)
                {
                    node.nodeView.ColorNode(Color.green);
                }
                else
                {

                }
            }
        }
    }
}
