using Burton.Lib.Graph;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Board board;
    public GameManager gameManager;

    Pathfinder m_pathFinder;
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
        m_pathFinder.SetGraph(board.graph);
        m_pathFinder.SetGraphView(board.graphView);

        StartCoroutine(WaitRoutine());

    }

    IEnumerator WaitRoutine()
    {
        yield return new WaitForSeconds(3f);

        FindPath();
    }

    [Button]
    void FindPath()
    {
        m_pathFinder.SetGraph(board.graph);
        m_pathFinder.SetGraphView(board.graphView);

        if (board.graph != null)
        {
            board.graph.ResetNodeColors();
        }

        m_pathFinder.FindPath(startIndex, goalIndex);
    }
}


