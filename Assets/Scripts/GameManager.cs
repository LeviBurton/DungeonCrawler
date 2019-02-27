using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.ShortestPath;
using QuickGraph;
using System;

public class GameManager : MonoBehaviour
{
    public Graph masterGraph;
    public GraphView masterGraphView;
    public PlayerManager player;

    public List<Tile> allTiles = new List<Tile>();

    void Awake()
    {
        allTiles = FindObjectsOfType<Tile>().ToList();
        masterGraph = GetComponent<Graph>();
        masterGraphView = GetComponent<GraphView>();
    }

    void Start()
    {
        RebuildMasterGraph();
    }

    public void RebuildMasterGraph()
    {
        List<NodeView> nodeViews = new List<NodeView>();

        masterGraph.Init(50, 50);

        foreach (var tile in allTiles)
        {
            var tileGraph = tile.GetComponent<Graph>();

            foreach (var tileNode in tileGraph.Nodes)
            {
                var nodeView = tileNode.nodeView;
                var nodeWorldPos = nodeView.transform.position;
                var newNode = masterGraph.CreateNode(nodeWorldPos);

                newNode.position = new Vector3(nodeWorldPos.x, nodeWorldPos.y, nodeWorldPos.z );
                newNode.nodeType = tileNode.nodeType;
          
                masterGraph.AddNode(newNode);
            }

            //tile.gameObject.SetActive(false);
        }

        masterGraphView.CreateNodeViews(masterGraph);
        masterGraphView.CreateEdgesBetweenNodes();

        foreach (var node in masterGraph.Nodes)
        {
            node.nodeView = masterGraphView.nodeViews.Single(x => x.nodeIndex == node.NodeIndex);
        }

        player.SetWorldGraph(masterGraph);
    }


}

