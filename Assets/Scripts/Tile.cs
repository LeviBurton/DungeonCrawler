using Burton.Lib.Graph;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Graph))]
public class Tile : MonoBehaviour
{
    public string tileId;

    public int width = 5;
    public int height = 5;

    public int startX = 0;
    public int startY = 0;
    public int goalX = 4;
    public int goalY = 3;

    public float timeStep = 0.1f;

    public Pathfinder m_pathFinder;

    SparseGraph<NavGraphNode, GraphEdge> m_sparseGraph;

    Graph m_graph;
    GraphView m_graphView;
    List<NodeView> nodeViews = new List<NodeView>();    // TODO: consider moving this to GraphView.
    int[,] m_tileData;

    void Awake()
    {
        m_graph = GetComponent<Graph>();
        m_graphView = GetComponent<GraphView>();
        m_pathFinder = GetComponent<Pathfinder>();
        m_sparseGraph = new SparseGraph<NavGraphNode, GraphEdge>();
    }

    void Start()
    {
        NodeView[] nodes = CreatGraphFromNodeViews();

        if (m_graphView != null)
        {
            m_graphView.Init(m_graph, nodes);
        }

        
        var node1 = m_sparseGraph.AddNode(new NavGraphNode(m_sparseGraph.GetNextFreeNodeIndex(), Vector3.zero));
        var node2 = m_sparseGraph.AddNode(new NavGraphNode(m_sparseGraph.GetNextFreeNodeIndex(), Vector3.zero));

        m_sparseGraph.AddEdge(new GraphEdge(node1, node2, 1.0f));

        Debug.Log(m_sparseGraph.NodeCount());
    }

    NodeView[] CreatGraphFromNodeViews()
    {
        int[,] tileData = new int[width, height];

        var nodeViews = this.GetComponentsInChildren<NodeView>();

        foreach (var node in nodeViews)
        {
            try
            {
                tileData[node.xIndex, node.yIndex] = (int)node.nodeType;
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.LogFormat("{0} {1} {2}", node.name, node.xIndex, node.yIndex);
            }
        }

        m_graph.Init(tileData);

        return nodeViews;
    }

    [Button(Name = "Reset Graph Nodes")]
    void ResetGraphNodes()
    {
        m_graph = GetComponent<Graph>();
        m_graphView = GetComponent<GraphView>();

        var tempList = transform.Cast<Transform>().ToList();

        nodeViews.Clear();

        foreach (var child in tempList)
        {
            if (child.GetComponent<NodeView>())
            {
                DestroyImmediate(child.gameObject);
            }
        }

        ResetGraphTileData(width, height);

        if (m_graphView != null)
        {
            m_graphView.RebuildNodeViews(m_graph, tileId);

            foreach (var node in m_graphView.m_nodeViews)
            {
                node.tileId = tileId;
            }
        }
    }

    void ResetGraphTileData(int width, int height)
    {
        m_tileData = new int[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                m_tileData[x, y] = (int)NodeType.Open;
            }
        }

        m_graph.Init(m_tileData);
    }
}
