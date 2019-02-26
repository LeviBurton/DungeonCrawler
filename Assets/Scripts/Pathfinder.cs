using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public TerrainCosts terrainCosts;

    Node m_startNode;
    Node m_goalNode;
    Graph m_graph;
    GraphView m_graphView;
    
    PriorityQueue<Node> m_frontierNodes;
    List<Node> m_exploredNodes;
    List<Node> m_pathNodes;

    public Color startColor = Color.green;
    public Color goalColor = Color.red;
    public Color frontierColor = Color.magenta;
    public Color exploredColor = Color.gray;
    public Color pathColor = Color.cyan;
    public Color arrowColor = new Color(0.85f, 0.85f, 0.85f, 1f);
    public Color highlightColor = new Color(1f, 1f, 0.5f, 1f);
    public bool showIterations = true;
    public bool showColors = true;
    public bool showArrows = true;
    public bool exitOnGoal = true;
    public bool isComplete = false;
    public Mode mode = Mode.BreadFirstSearch;

    int m_iterations = 0;
    public enum Mode
    {
        BreadFirstSearch = 0,
        Dijkstra = 1,
        GreedyBestFirst = 2,
        AStar = 3
    }


    public void Init(Graph graph, GraphView graphView, Node start, Node goal)
    {
        if (start == null || goal == null || graph == null || graphView == null)
        {
            Debug.LogWarning("PathFinder Init error");
            return;
        }

        if (start.nodeType == NodeType.Blocked || goal.nodeType == NodeType.Blocked)
        {
            Debug.LogWarning("start or goal is blocked");
            return;
        }

        m_graph = graph;
        m_graphView = graphView;
        m_startNode = start;
        m_goalNode = goal;

        ShowColors(graphView, start, goal);

        m_frontierNodes = new PriorityQueue<Node>();
        m_frontierNodes.Enqueue(start);
        m_exploredNodes = new List<Node>();
        m_pathNodes = new List<Node>();

        for (int x = 0; x < m_graph.Width; x++)
        {
            for (int y = 0; y < m_graph.Height; y++)
            {
                m_graph.nodes[x, y].Reset();
            }
        }

        isComplete = false;
        m_iterations = 0;
        m_startNode.distanceTraveled = 0;
    }

    void ShowColors(bool lerpColor = false, float lerpValue = 0.5f)
    {
        ShowColors(m_graphView, m_startNode, m_goalNode, lerpColor, lerpValue);
    }

    void ShowColors(GraphView graphView, Node start, Node goal, bool lerpColor = false, float lerpValue = 0.5f)
    {
        if (graphView == null || start == null || goal == null)
        {
            return;
        }

        if (m_frontierNodes != null)
        {
            graphView.ColorNodes(m_frontierNodes.ToList(), frontierColor, lerpColor, lerpValue);
        }

        if (m_exploredNodes != null)
        {
            graphView.ColorNodes(m_exploredNodes.ToList(), exploredColor, lerpColor, lerpValue);
        }

        if (m_pathNodes != null && m_pathNodes.Count > 0)
        {
            graphView.ColorNodes(m_pathNodes, pathColor, lerpColor, lerpValue * 2f);
        }

        NodeView startNodeView = graphView.m_nodeViews[start.xIndex, start.yIndex];

        if (startNodeView != null)
        {
            startNodeView.ColorNode(startColor);
        }

        NodeView goalNodeView = graphView.m_nodeViews[goal.xIndex, goal.yIndex];
        if (goalNodeView)
        {
            goalNodeView.ColorNode(goalColor);
        }
    }

    public IEnumerator SearchRoutine(float timeStep = 0.1f)
    {
        float timeStart = Time.realtimeSinceStartup;

        yield return null;

        while (!isComplete)
        {
            if (m_frontierNodes.Count > 0)
            {
                Node currentNode = m_frontierNodes.Dequeue();
                m_iterations++;

                if (!m_exploredNodes.Contains(currentNode))
                {
                    m_exploredNodes.Add(currentNode);
                }

                if (mode == Mode.BreadFirstSearch)
                {
                    ExpandFrontierBreadthFirst(currentNode);
                }
                else if (mode == Mode.Dijkstra)
                {
                    ExpandFrontierDijkstra(currentNode);
                }
                else if (mode == Mode.GreedyBestFirst)
                {
                    ExpandFrontierGreedyBestFirst(currentNode);
                }
                else if (mode == Mode.AStar)
                {
                    ExpandFrontierAStar(currentNode);
                }

                if (m_frontierNodes.Contains(m_goalNode))
                {
                    m_pathNodes = GetPathNodes(m_goalNode);
                    if (exitOnGoal)
                    {
                        isComplete = true;
                        Debug.Log("Pathfinder mode: " + mode.ToString() +
                            "   path length = " + m_goalNode.distanceTraveled.ToString());
                    }
                }

                if (showIterations)
                {
                    ShowDiagnostics(true, 0.5f);

                    yield return new WaitForSeconds(timeStep);
                }
            }
            else
            {
                isComplete = true;
            }

        }

        ShowDiagnostics(true, 0.5f);

        Debug.Log("Pathfinder SearchRouting: elapse time = " + (Time.realtimeSinceStartup - timeStart).ToString() + " seconds");
    }

    void ShowDiagnostics(bool lerpColor = false, float lerpValue = 0.5f)
    {
        if (showColors)
        {
            ShowColors(lerpColor, lerpValue);
        }

        if (m_graphView != null && showArrows == true)
        {
            m_graphView.ShowNodeArrows(m_frontierNodes.ToList(), arrowColor);
            if (m_frontierNodes.Contains(m_goalNode))
            {
                m_graphView.ShowNodeArrows(m_pathNodes, highlightColor);
            }
        }
    }

    void ExpandFrontierBreadthFirst(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i]) &&
                    !m_frontierNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbor = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    float newDistanceTraveled = distanceToNeighbor + node.distanceTraveled + terrainCosts.GetCost(node.nodeType);

                    node.neighbors[i].distanceTraveled = newDistanceTraveled;

                    node.neighbors[i].previous = node;
                    node.neighbors[i].priority = m_exploredNodes.Count;

                    m_frontierNodes.Enqueue(node.neighbors[i]);
                }
            }
        }
    }

    void ExpandFrontierDijkstra(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbor = m_graph.GetNodeDistance(node, node.neighbors[i]);

                    float newDistanceTraveled = 
                        distanceToNeighbor + 
                        node.distanceTraveled + 
                        terrainCosts.GetCost(node.nodeType);

                    if (float.IsPositiveInfinity(node.neighbors[i].distanceTraveled) ||
                        newDistanceTraveled < node.neighbors[i].distanceTraveled)
                    {
                        node.neighbors[i].previous = node;
                        node.neighbors[i].distanceTraveled = newDistanceTraveled;
                    }

                    if (!m_frontierNodes.Contains(node.neighbors[i]))
                    {
                        node.neighbors[i].priority = node.neighbors[i].distanceTraveled;
                        m_frontierNodes.Enqueue(node.neighbors[i]);
                    }
                }
            }
        }
    }

    void ExpandFrontierGreedyBestFirst(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i]) &&
                    !m_frontierNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbor = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    float newDistanceTraveled = distanceToNeighbor + node.distanceTraveled + terrainCosts.GetCost(node.nodeType);
                    node.neighbors[i].distanceTraveled = newDistanceTraveled;

                    node.neighbors[i].previous = node;
                    if (m_graph != null)
                    {
                        node.neighbors[i].priority = m_graph.GetNodeDistance(node.neighbors[i], m_goalNode);
                    }

                    m_frontierNodes.Enqueue(node.neighbors[i]);
                }
            }
        }
    }

    void ExpandFrontierAStar(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbor = m_graph.GetNodeDistance(node, node.neighbors[i]);

                    // TODO: Compute node cost here!
                    float newDistanceTraveled = 
                        distanceToNeighbor + 
                        node.distanceTraveled + 
                        terrainCosts.GetCost(node.nodeType);

                    if (float.IsPositiveInfinity(node.neighbors[i].distanceTraveled) ||
                        newDistanceTraveled < node.neighbors[i].distanceTraveled)
                    {
                        node.neighbors[i].previous = node;
                        node.neighbors[i].distanceTraveled = newDistanceTraveled;
                    }

                    if (!m_frontierNodes.Contains(node.neighbors[i]) && m_graph != null)
                    {
                        float distanceToGoal = m_graph.GetNodeDistance(node.neighbors[i], m_goalNode);
                        node.neighbors[i].priority = node.neighbors[i].distanceTraveled + distanceToGoal;

                        m_frontierNodes.Enqueue(node.neighbors[i]);
                    }
                }
            }
        }
    }

    List<Node> GetPathNodes(Node endNode)
    {
        List<Node> path = new List<Node>();
        if (endNode == null)
        {
            return path;
        }

        path.Add(endNode);

        Node currentNode = endNode.previous;
        while (currentNode != null)
        {
            path.Insert(0, currentNode);
            currentNode = currentNode.previous;
        }

        return path;
    }
}
