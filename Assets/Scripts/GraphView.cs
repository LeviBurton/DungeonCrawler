using Burton.Lib.Graph;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphView : MonoBehaviour
{
    public GameObject nodeViewPrefab;
    public NodeView[,] m_nodeViews;
    public Graph m_graph;
    public TileColors m_tileColors;
    public TerrainCosts m_terrainCosts;

    public List<NodeView> nodeViews = new List<NodeView>();    // TODO: consider moving this to GraphView.

    string tileId;

    void Awake()
    {
        m_graph = GetComponent<Graph>();
    }

    public void Init(Graph graph)
    {
        m_graph = graph;

        foreach (var view in m_nodeViews)
        {
            Color originalColor = m_tileColors.GetNodeTypeColor(view.nodeType);
            view.tileColors = m_tileColors;
            view.ColorNode(originalColor);
        }
    }

    public void Init(Graph graph, NodeView[] nodeViews)
    {
        m_graph = graph;

        m_nodeViews = new NodeView[m_graph.width, m_graph.height];

        foreach (var view in nodeViews)
        {
            Color originalColor = m_tileColors.GetNodeTypeColor(view.nodeType);
            view.tileColors = m_tileColors;
            view.ColorNode(originalColor);
        }
    }

    public NavGraphNode GetNodeAtPosition(Vector3 position)
    {
        NavGraphNode graphNode = null;

        var nodeView = nodeViews.SingleOrDefault(x => x.transform.position == position);

        if (nodeView != null)
        {
            graphNode = m_graph.GetNode(nodeView.nodeIndex);
        }

        return graphNode;
    }

    public void CreateNodeViews(Graph graph)
    {
        var gameObject = new GameObject("Master Graph Nodes");

        foreach (var node in graph.Nodes)
        {
            var instance = Instantiate(nodeViewPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);
            var nodeView = instance.GetComponent<NodeView>();

            nodeView.nodeType = node.nodeType;
            nodeView.nodeIndex = node.NodeIndex;
            nodeView.tileColors = m_tileColors;
            nodeViews.Add(nodeView);

            instance.name = string.Format("Node {0}", node.NodeIndex);
            instance.transform.position = node.position;
        }
    }

    public void CreateNodeViews(int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var instance = Instantiate(nodeViewPrefab, Vector3.zero, Quaternion.identity, transform);
                var nodeView = instance.GetComponent<NodeView>();

                nodeView.nodeType = NodeType.Open;
                nodeView.tileColors = m_tileColors;
                nodeViews.Add(nodeView);
                instance.name = "Node (" + nodeView.xIndex + "," + nodeView.yIndex + ")";
                instance.transform.position = new Vector3(x,0, y);
            }
        }
    }

    public void CreateEdgesBetweenNodes()
    {
        foreach (var node in m_graph.Nodes)
        {
            foreach (var dir in Graph.allDirections)
            {
                var nextPos = new Vector3(node.position.x + dir.x, 0f, node.position.z + dir.y);
                var neighborNode = GetNodeAtPosition(nextPos);
                
                if (neighborNode != null && neighborNode.nodeType != NodeType.Blocked)
                {
                    m_graph.AddEdge(node.NodeIndex, neighborNode.NodeIndex, m_terrainCosts.GetCost(neighborNode.nodeType));
                }
            }
        }
    }

    public void ClearNodeViews()
    {
        var tempList = transform.Cast<Transform>().ToList();

        nodeViews.Clear();

        foreach (var child in tempList)
        {
            if (child.GetComponent<NodeView>())
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    public void RebuildNodeViews(Graph graph, string tileId = "")
    {
        foreach (var n in graph.Nodes)
        {
            var instance = Instantiate(nodeViewPrefab, Vector3.zero, Quaternion.identity, transform);
            NodeView nodeView = instance.GetComponent<NodeView>();

            if (nodeView != null)
            {
                nodeView.SetFromNode(n);
                nodeView.xIndex = n.xIndex;
                nodeView.yIndex = n.yIndex;
                nodeView.tileId = tileId;
                nodeView.tileColors = m_tileColors;

                Color originalColor = m_tileColors.GetNodeTypeColor(n.nodeType);
                nodeView.ColorNode(originalColor);
            }
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
                    Color originalColor = m_tileColors.GetNodeTypeColor(n.nodeType);
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
                //nodeView.ShowArrow(color);
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
