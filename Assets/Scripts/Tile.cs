using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Graph))]
public class Tile : MonoBehaviour
{
    public string tileId;

    public Pathfinder m_pathFinder;

    public int width = 5;
    public int height = 5;

    List<NodeView> nodeViews = new List<NodeView>();
    Graph m_graph;
    GraphView m_graphView;

    int[,] m_tileData;

    public int startX = 0;
    public int startY = 0;
    public int goalX = 4;
    public int goalY = 3;
    public float timeStep = 0.1f;

    void Awake()
    {
        m_graph = GetComponent<Graph>();
        m_graphView = GetComponent<GraphView>();
        m_pathFinder = GetComponent<Pathfinder>();

        NodeView[] nodes = CreatGraphFromNodeViews();

        if (m_graphView != null)
        {
            m_graphView.Init(m_graph, nodes);
        }
    }

    void Start()
    {
        if (m_graph.IsWithinBounds(startX, startY) && m_graph.IsWithinBounds(goalX, goalY) && m_pathFinder != null)
        {
            Node startNode = m_graph.nodes[startX, startY];
            Node goalNode = m_graph.nodes[goalX, goalY];
            m_pathFinder.Init(m_graph, m_graphView, startNode, goalNode);
            StartCoroutine(m_pathFinder.SearchRoutine(timeStep));
        }
    }

    private NodeView[] CreatGraphFromNodeViews()
    {
        int[,] tileData = new int[width, height];

        var nodes = GetComponentsInChildren<NodeView>();

        foreach (var node in nodes)
        {
            NodeType nodeType = node.nodeType;

            int xIndex = node.xIndex;
            int yIndex = node.yIndex;
            
            tileData[xIndex, yIndex] = (int)node.nodeType;
        }

        m_graph.Init(tileData);

        return nodes;
    }

    [Button(Name = "Reset Graph Nodes")]
    void ResetGraphNodes()
    {
        m_graph = GetComponent<Graph>();

        var tempList = transform.Cast<Transform>().ToList();

        nodeViews.Clear();

        foreach (var child in tempList)
        {
            if (child.GetComponent<NodeView>())
            {
                DestroyImmediate(child.gameObject);
            }
        }

        ResetTileData(width, height);

        if (m_graphView == null)
        {
            m_graphView = GetComponent<GraphView>();
        }

        if (m_graphView != null)
        {
            m_graphView.Reset(m_graph, tileId);

            foreach (var node in m_graphView.m_nodeViews)
            {
                node.tileId = tileId;
            }
        }
    }

    void ResetTileData(int width, int height)
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
