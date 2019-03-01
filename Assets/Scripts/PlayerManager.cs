using Burton.Lib.Graph;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Board board;
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
        board = FindObjectOfType<Board>();
    }

    void Start()
    {
        m_pathFinder.SetGraph(m_graph);
        m_pathFinder.SetGraphView(board.graphView);
        FindPath();
    }

    public void SetWorldGraph(Graph worldGraph)
    {
        m_graph = worldGraph;
        m_pathFinder.SetGraph(board.graph);
    }

    [Button]
    void FindPath()
    {
        m_pathFinder.SetGraph(board.graph);
        m_pathFinder.SetGraphView(board.graphView);
        if (m_graph != null)
        {
            m_graph.ResetNodeColors();
        }

        m_pathFinder.FindPath(startIndex, goalIndex);
    }
}


