using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Graph graph;
    public GraphView graphView;
    public PlayerManager player;

    public List<Tile> tiles = new List<Tile>();

    void Awake()
    {
        tiles = FindObjectsOfType<Tile>().ToList();
        graph = GetComponent<Graph>();
        graphView = GetComponent<GraphView>();
     
    }

    void Start()
    {
        RebuildGraph();

    }


    public void RebuildGraph()
    {
        graph.Init(50, 50);

        foreach (var tile in tiles)
        {
            var tileGraph = tile.GetComponent<Graph>();

            foreach (var wall in tile.walls)
            {
                graphView.AddWall(wall);
            }

            foreach (var tileNode in tileGraph.Nodes)
            {
                var nodeView = tileNode.nodeView;
                nodeView.tileId = tile.tileId;

                var nodeWorldPos = nodeView.transform.position;
                var newNode = graph.CreateNode(nodeWorldPos);

                newNode.position = new Vector3(nodeWorldPos.x, nodeWorldPos.y, nodeWorldPos.z);
                newNode.nodeType = tileNode.nodeType;

                tile.GetComponent<GraphView>().drawGizmo = false;

                graph.AddNode(newNode);
            }

           // tile.gameObject.SetActive(false);
        }

        graphView.CreateNodeViews(graph);
        graphView.CreateEdgesBetweenNodes();

        foreach (var node in graph.Nodes)
        {
            node.nodeView = graphView.nodeViews.Single(x => x.nodeIndex == node.NodeIndex);
        }
    }
}
