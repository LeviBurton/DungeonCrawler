using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Pathfinder m_pathFinder;

    void Awake()
    {
        m_pathFinder = GetComponent<Pathfinder>();
    }

    void Start()
    {
        //if (m_graph.IsWithinBounds(startX, startY) && m_graph.IsWithinBounds(goalX, goalY) && m_pathFinder != null)
        //{
        //    Node startNode = m_graph.nodes[startX, startY];
        //    Node goalNode = m_graph.nodes[goalX, goalY];
        //    m_pathFinder.Init(m_graph, m_graphView, startNode, goalNode);
        //    StartCoroutine(m_pathFinder.SearchRoutine(timeStep));
        //}
    }
}


