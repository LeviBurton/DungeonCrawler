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

    public List<Tile> allTiles = new List<Tile>();

    void Awake()
    {
        allTiles = FindObjectsOfType<Tile>().ToList();
        masterGraph = GetComponent<Graph>();
        masterGraphView = GetComponent<GraphView>();
    }

    private void Start()
    {
        //RebuildMasterGraph();
        //TestQuickGraph();
    }

    void TestQuickGraph()
    {
        //var graph = new AdjacencyGraph<int, Edge<int>>(false);

        //// Add some vertices to the graph
        //graph.AddVertex(1);
        //graph.AddVertex(2);
        //graph.AddVertex(3);

        //// Create the edges
        //var edge12 = new Edge<int>(1, 2);
        //var edge13 = new Edge<int>(1, 3);
        //var edge23 = new Edge<int>(2, 3);

        //int target = 3;

        //// Add the edges
        //graph.AddEdge(edge12);
        //graph.AddEdge(edge13);
        //graph.AddEdge(edge23);

        //Func<Edge<int>, double> costHeuristic = e => 1f;

        //Func<Edge<int>, double> edgeWeights = e => e.Target + e.Source;

        //var tryGetPath = graph.ShortestPathsAStar(edgeWeights, costHeuristic, 1);
        //IEnumerable<Edge<int>> path;
        //if (tryGetPath(target, out path))
        //    foreach (var e in path)
        //    {
        //        Debug.Log(e);
        //    }
    }

    public void RebuildMasterGraph()
    {
        int tilesWidth = 0;
        int tilesHeight = 0;

        foreach (var tile in allTiles)
        {
            tilesWidth += tile.width;
            tilesHeight += tile.height;
        }

        int[,] tiledata = new int[tilesWidth, tilesHeight];

        for (int tileIndex = 1; tileIndex < 5; tileIndex++)
        {
            foreach (var nodeView in allTiles[tileIndex - 1].GetComponentsInChildren<NodeView>())
            {
                int newX = tileIndex * nodeView.xIndex;
                int newY = tileIndex * nodeView.yIndex;

                tiledata[newX,newY] = (int)nodeView.nodeType;
            }
        }

        masterGraphView.RebuildNodeViews(masterGraph, "AllTiles");
        masterGraphView.Init(masterGraph);
    }
}

