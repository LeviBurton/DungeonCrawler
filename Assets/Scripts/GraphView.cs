﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Graph))]
public class GraphView : MonoBehaviour
{
    public GameObject nodeViewPrefab;
    public NodeView[,] m_nodeViews;
    public Graph m_graph;

    TileData m_tileData;

    void Awake()
    {
        m_tileData = GetComponent<TileData>();
    }

    public void Reset(Graph graph)
    {
        m_nodeViews = new NodeView[graph.Width, graph.Height];
        m_tileData = GetComponent<TileData>();

        foreach (var n in graph.nodes)
        {
            var instance = Instantiate(nodeViewPrefab, Vector3.zero, Quaternion.identity, transform);
            NodeView nodeView = instance.GetComponent<NodeView>();

            if (nodeView != null)
            {
                nodeView.SetFromNode(n);
                m_nodeViews[n.xIndex, n.yIndex] = nodeView;

                Color originalColor = TileData.GetColorFromNodeType(n.nodeType);
                nodeView.ColorNode(originalColor);
            }
        }
    }

    public void Init(Graph graph, NodeView[] nodeViews)
    {
        m_graph = graph;
        m_nodeViews = new NodeView[m_graph.Width, m_graph.Height];

        m_tileData = GetComponent<TileData>();

        foreach (var view in nodeViews)
        {
            Color originalColor = TileData.GetColorFromNodeType(view.nodeType);

            m_nodeViews[view.xIndex, view.yIndex] = view;
            view.ColorNode(originalColor);
        }
    }

    public void ColorNodes(List<Node> nodes, Color color, bool lerpColor = false, float lerpValue = 0.5f)
    {
        foreach (Node n in nodes)
        {
            if (n != null)
            {
                NodeView nodeView = m_nodeViews[n.xIndex, n.yIndex];
                Color newColor = color;

                if (lerpColor)
                {
                    Color originalColor = TileData.GetColorFromNodeType(n.nodeType);
                    newColor = Color.Lerp(originalColor, newColor, lerpValue);
                }

                if (nodeView != null)
                {
                    nodeView.ColorNode(newColor);
                }
            }
        }
    }

    public void ShowNodeArrows(Node node, Color color)
    {
        if (node != null)
        {
            NodeView nodeView = m_nodeViews[node.xIndex, node.yIndex];
            if (nodeView != null)
            {
                nodeView.ShowArrow(color);
            }
        }
    }

    public void ShowNodeArrows(List<Node> nodes, Color color)
    {
        foreach (Node n in nodes)
        {
            ShowNodeArrows(n, color);
        }
    }
}