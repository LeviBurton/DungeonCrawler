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

    Graph m_graph;
    GraphView m_graphView;
 
    int[,] m_tileData;

    void Awake()
    {
        m_graph = GetComponent<Graph>();
        m_graphView = GetComponent<GraphView>();
        m_graph.Init(width, height);
        CreatGraphFromNodeViews();
    }

    void Start()
    {
        // initalize our graph from our node view setup.
        // the node views act as the "editor" for our graph.
        // we generate the node views, then set properties on them.
        // then the graph we can search is generated from those.

    }

    [Button(Name = "Reset Graph Nodes")]
    void ResetGraphNodes()
    {
        m_graph = GetComponent<Graph>();
        m_graphView = GetComponent<GraphView>();

        m_graphView.ClearNodeViews();
        m_graphView.CreateNodeViews(width, height);

        CreatGraphFromNodeViews();
        m_graphView.RebuildNodeViews(m_graph, tileId);

        foreach (var node in m_graphView.m_nodeViews)
        {
            node.tileId = tileId;
        }
    }

    void CreatGraphFromNodeViews()
    {
        var nodeViews = this.GetComponentsInChildren<NodeView>();

        foreach (var nodeView in nodeViews)
        {
            var navGraphNode = m_graph.CreateNode(nodeView.transform.position);
            var newNodeIndex = m_graph.AddNode(navGraphNode);

            navGraphNode.nodeView = nodeView;
            navGraphNode.xIndex = nodeView.xIndex;
            navGraphNode.yIndex = nodeView.yIndex;
            navGraphNode.nodeType = nodeView.nodeType;
            nodeView.nodeIndex = newNodeIndex;
        }

        // populate the graph by visiting each node and adding all of its neighbors.
        // TODO: the AddAllNeighborsToGridNode code is old and ugly -- there is a much better way.
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                m_graph.AddAllNeighborsToGridNode(x, y, width, height);
            }
        }
    }
}
