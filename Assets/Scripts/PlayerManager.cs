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
    Pathfinder m_pathFinder;
    List<Tile> m_allTiles;
    public List<int> pathToTarget = new List<int>();

    void Awake()
    {
        m_pathFinder = GetComponent<Pathfinder>();
        m_graph = GetComponent<Graph>();
        m_graphView = GetComponent<GraphView>();
        m_allTiles = FindObjectsOfType<Tile>().ToList();
        
    }

    void Start()
    {
        
    }

    [Button]
    void BuildMasterGraph()
    {
        List<NodeView> nodeViews = new List<NodeView>();

        // TODO: determine graph size before creating.  should be easy.
        m_graph.Init(50, 50);

        foreach (var tile in m_allTiles)
        {
            var tileGraph = tile.GetComponent<Graph>();

            foreach (var tileNode in tileGraph.Nodes)
            {
                NodeView nodeView = tileNode.nodeView;
                var nodeWorldPos = nodeView.transform.position;
                var newNode = m_graph.CreateNode(nodeWorldPos);
                newNode.position = new Vector3(nodeWorldPos.x - 0.5f, nodeWorldPos.y, nodeWorldPos.z - 0.5f);
                newNode.nodeType = tileNode.nodeType;

                m_graph.AddNode(newNode);
             
                nodeViews.Add(nodeView);

                //     Debug.LogFormat("{0} {1} local: {2} world: {3}", tile.tileId, tileNode.nodeView.name, nodeView.transform.localPosition, nodeWorldPos);
            }

            tile.gameObject.SetActive(false);
        }


        m_graphView.CreateNodeViews(m_graph);
        m_graphView.CreateEdgesBetweenNodes();


        var goalNode = m_graphView.GetNodeAtPosition(new Vector3(14, 0, -5));
        var startNode = m_graphView.GetNodeAtPosition(transform.position);

        pathToTarget.Clear();
        IHeuristic<SparseGraph<NavGraphNode, GraphEdge>> Heuristic = new UnityDistanceHeuristic();

        var search = new Search_AStar<NavGraphNode, GraphEdge>(m_graph.m_sparseGraph, Heuristic, startNode.NodeIndex, goalNode.NodeIndex);
        if (search.Search())
        {
            pathToTarget.AddRange(search.GetPathToTarget());

            foreach (var idx in pathToTarget)
            {
                var node = m_graph.m_sparseGraph.GetNode(idx);
                var nodeView = m_graphView.nodeViews.SingleOrDefault(x => x.nodeIndex == idx);

                nodeView.ColorNode(Color.green);
            }
        }
    }
}


