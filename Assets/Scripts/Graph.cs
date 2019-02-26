using Burton.Lib.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Graph : MonoBehaviour
{
    public SparseGraph<NavGraphNode, GraphEdge> m_sparseGraph;
    public List<NavGraphNode> Nodes { get { return m_sparseGraph.Nodes; } }

    public List<Node> m_nodeList = new List<Node>();

    public List<Node> walls = new List<Node>();
    public int Width { get { return m_width; } }
    public int Height { get { return m_height; } }

    int[,] m_mapData;
    int m_width;
    int m_height;

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
        m_sparseGraph = new SparseGraph<NavGraphNode, GraphEdge>(true);
        m_width = width;
        m_height = height;
    }

    public NavGraphNode CreateNode()
    {
        return new NavGraphNode(m_sparseGraph.GetNextFreeNodeIndex(), Vector3.zero);
    }

    public NavGraphNode CreateNode(Vector3 position)
    {
        return new NavGraphNode(m_sparseGraph.GetNextFreeNodeIndex(), position);
    }

    public void CreateEdgesBetweenNodes()
    {
        foreach (var node in Nodes)
        {
            foreach (var dir in allDirections)
            {
          
            }
            Debug.Log(node.NodeIndex);
        }
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

    List<Node> GetNeighbors(int x, int y, Node[,] nodeArray, Vector2[] directions)
    {
        List<Node> neighborNodes = new List<Node>();

        foreach (Vector2 dir in directions)
        {
            int newX = x + (int)dir.x;
            int newY = y + (int)dir.y;

            bool isValidNode =
                IsWithinBounds(newX, newY) &&
                nodeArray[newX, newY] != null &&
                nodeArray[newX, newY].nodeType != NodeType.Blocked;

            if (isValidNode)
            {
                neighborNodes.Add(nodeArray[newX, newY]);
            }
        }

        return neighborNodes;
    }

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
