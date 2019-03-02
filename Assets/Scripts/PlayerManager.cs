using Burton.Lib.Graph;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: eventually we will need to consider controlling multiple "pawns"
public class PlayerManager : TurnManager
{
    public Stats stats;
    public GameManager gameManager;
    public int goalIndex;

    Board m_board;
    PlayerMover m_playerMover;
    Pathfinder m_pathFinder;
    NavGraphNode m_currentNode;
    List<int> pathToTarget = new List<int>();

    protected override void Awake()
    {
        base.Awake();

        m_pathFinder = GetComponent<Pathfinder>();
        m_board = FindObjectOfType<Board>();
        m_playerMover = GetComponent<PlayerMover>();
    }

    void Start()
    {
        m_pathFinder.SetGraph(m_board.graph);
        m_pathFinder.SetGraphView(m_board.graphView);
        m_currentNode =  m_board.GetNavGraphNodeAtPosition(transform.position);
    }

    [Button]
    public void FindPath()
    {
        m_playerMover.UpdateCurrentNode();
        m_pathFinder.SetGraph(m_board.graph);
        m_pathFinder.SetGraphView(m_board.graphView);
        m_currentNode = m_board.GetNavGraphNodeAtPosition(transform.position);

        if (m_board.graph != null)
        {
            m_board.graph.ResetNodeColors();
        }

        if (m_pathFinder.FindPath(m_currentNode.NodeIndex, goalIndex))
        {
            var pathNodes = m_pathFinder.pathToTarget;

            // Remove current node from the path, since we are already on it.
            pathNodes.Remove(m_currentNode.NodeIndex);

            MoveAlongPath(pathNodes);
        }
    }

    public void MoveAlongPath(List<int> nodes)
    {
        StartCoroutine(MoveAlongPathRoutine(nodes));
    }

    IEnumerator MoveAlongPathRoutine(List<int> nodes)
    {
        foreach (var node in nodes)
        {
            m_playerMover.Move(m_board.GetNode(node).position, 0f);

            while (m_playerMover.isMoving)
            {
                yield return null;
            }

            FinishTurn();
        }
    }

    public override void FinishTurn()
    {
  //      CaptureEnemies();
        base.FinishTurn();
    }
}


