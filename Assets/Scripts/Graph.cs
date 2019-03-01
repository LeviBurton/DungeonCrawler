using Burton.Lib.Graph;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: this class might be redundant and can mostly be moved into the GraphView class.
[Serializable]
public class Graph : MonoBehaviour
{
    public SparseGraph<NavGraphNode, GraphEdge> m_sparseGraph;
    public List<NavGraphNode> Nodes { get { return m_sparseGraph.Nodes; } }

    public List<List<GraphEdge>> Edges { get { return m_sparseGraph.Edges; } }
    public TileColors tileColors;
    public TerrainCosts m_terrainCosts;
    public List<Node> walls = new List<Node>();
    public int width { get { return m_width; } }
    public int height { get { return m_height; } }
    int m_width;
    int m_height;

    // 8 directions we can move.
    public static readonly Vector2[] allDirections =
    {
     new Vector2(0f, 1f),
        new Vector2(1f, 1f),
        new Vector2(1f, 0f),
        new Vector2(1f,-1f),
        new Vector2(0f, -1f),
        new Vector2(-1f, -1f),
        new Vector2(-1f, 0f),
        new Vector2(-1f, 1f)
    };

    public void Init(int width, int height)
    {
        m_sparseGraph = new SparseGraph<NavGraphNode, GraphEdge>(false);
        m_width = width;
        m_height = height;
    }

    [Button(Name = "Node Color Defaults")]
    public void ResetNodeColors()
    {
        foreach (var node in Nodes)
        {
            Color originalColor = tileColors.GetNodeTypeColor(node.nodeType);
            node.nodeView.ColorNode(originalColor);
        }
    }

    public NavGraphNode CreateNode()
    {
        return new NavGraphNode(m_sparseGraph.GetNextFreeNodeIndex(), Vector3.zero);
    }

    public NavGraphNode CreateNode(Vector3 position)
    {
        return new NavGraphNode(m_sparseGraph.GetNextFreeNodeIndex(), position);
    }

    public GraphEdge AddEdge(int fromNodeIndex, int toNodeIndex, float cost)
    {
        var edge = new GraphEdge(fromNodeIndex, toNodeIndex, cost);

        m_sparseGraph.AddEdge(edge);

        return edge;
    }

    public int AddNode(NavGraphNode node)
    {
        return m_sparseGraph.AddNode(node);
    }

    public NavGraphNode GetNode(int nodeIndex)
    {
        return m_sparseGraph.GetNode(nodeIndex);
    }

    public bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < m_width && y >= 0 && y < m_height);
    }

    // Only used in the prefabs when generating the local grid -- the prefab uses a grid layout, 
    // so this works there just fine.
    public void AddAllNeighborsToGridNode(int row, int col, int width, int height)
    {
        for (int i = -1; i < 2; ++i)
        {
            for (int j = -1; j < 2; ++j)
            {
                int nodeX = (row + j);
                int nodeY = (col + i);

                if ((i == 0) && (j == 0))
                    continue;

                if (IsWithinBounds(nodeX, nodeY))
                {
                    int nodeIdx = col * width + row;

                    var node = m_sparseGraph.GetNode(nodeIdx);

                    if (node == null || node.NodeIndex == (int)ENodeType.InvalidNodeIndex)
                        continue;

                    var neighborNode = m_sparseGraph.GetNode(nodeY * width + nodeX);

                    if (neighborNode == null || neighborNode.NodeIndex == (int)ENodeType.InvalidNodeIndex || neighborNode.nodeType == NodeType.Blocked)
                        continue;

                    var pos = new Vector3(node.position.x, node.position.y, node.position.z);
                    var neighborPos = new Vector3(neighborNode.position.x, neighborNode.position.y, neighborNode.position.z);

                    double distance = Vector3.Distance(pos, neighborPos);
                    var newEdge = new GraphEdge(node.NodeIndex, neighborNode.NodeIndex, distance);

                    m_sparseGraph.AddEdge(newEdge);

                    if (!m_sparseGraph.IsDigraph())
                    {
                        UnityEdge Edge = new UnityEdge(neighborNode.NodeIndex, node.NodeIndex, distance);
                        m_sparseGraph.AddEdge(Edge);
                    }
                }

            }
        }
    }

}
