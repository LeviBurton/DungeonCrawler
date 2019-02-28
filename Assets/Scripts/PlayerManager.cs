using Burton.Lib.Graph;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Graph m_graph;
    GraphView m_graphView;
    public GameManager gameManager;

    Pathfinder m_pathFinder;
    List<Tile> m_allTiles;
    public List<int> pathToTarget = new List<int>();

    public int startIndex;
    public int goalIndex;

    void Awake()
    {
        m_pathFinder = GetComponent<Pathfinder>();
    }

    void Start()
    {
        m_pathFinder.SetGraph(m_graph);
        m_pathFinder.SetGraphView(gameManager.masterGraphView);
        FindPath();
    }

    public void SetWorldGraph(Graph worldGraph)
    {
        m_graph = worldGraph;
        m_pathFinder.SetGraph(gameManager.masterGraph);
    }

    [Button]
    void FindPath()
    {
        m_pathFinder.SetGraph(gameManager.masterGraph);
        m_pathFinder.SetGraphView(gameManager.masterGraphView);
        if (m_graph != null)
        {
            m_graph.ResetNodeColors();
        }
        m_pathFinder.FindPath(startIndex, goalIndex);
    }
}


