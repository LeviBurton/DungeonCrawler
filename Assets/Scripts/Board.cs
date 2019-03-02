using Burton.Lib.Graph;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Board : MonoBehaviour
{
    public Graph graph;
    public GraphView graphView;
    public List<Tile> tiles = new List<Tile>();

    Dictionary<int, Tile> nodeToTile = new Dictionary<int, Tile>();
    NavGraphNode playerNode;
    PlayerMover player;

    public UnityEvent setupComplete;

    void Awake()
    {
        tiles = FindObjectsOfType<Tile>().ToList();
        graph = GetComponent<Graph>();
        graphView = GetComponent<GraphView>();
        player = FindObjectOfType<PlayerMover>();
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
            var tileGraphView = tileGraph.GetComponent<GraphView>();

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
                nodeToTile.Add(newNode.NodeIndex, tile);
            }

            var nodeViews = tile.GetComponentsInChildren<NodeView>();
            foreach (var nodeView in nodeViews)
            {
                nodeView.gameObject.SetActive(false);
            }
        }

        graphView.CreateNodeViews(graph);
        graphView.CreateEdgesBetweenNodes();

        foreach (var node in graph.Nodes)
        {
            node.nodeView = graphView.nodeViews.Single(x => x.nodeIndex == node.NodeIndex);
        }

        setupComplete.Invoke();
    }

    public NavGraphNode GetNavGraphNodeAtPosition(Vector3 position)
    {
        return graphView.GetNodeAtPosition(position);
    }

    public NavGraphNode GetNode(int nodeIndex)
    {
        return graph.GetNode(nodeIndex);
    }

    public NavGraphNode FindPlayerNode()
    {
        if (player != null && !player.isMoving)
        {
            return GetNavGraphNodeAtPosition(player.transform.position);
        }

        return null;
    }

    public void UpdatePlayerNode()
    {
        playerNode = FindPlayerNode();
    }

    public Tile GetTileForNode(int nodeIndex)
    {
        return nodeToTile[nodeIndex];
    }
}
