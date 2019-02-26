using Burton.Lib.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Burton.Lib.Unity
{
    public class UnityDistanceHeuristic : IHeuristic<SparseGraph<UnityNode, UnityEdge>>
    {
        public double Calculate(SparseGraph<UnityNode, UnityEdge> Graph, int Node1, int Node2)
        {
            var UnityNode1 = Graph.GetNode(Node1);
            var UnityNode2 = Graph.GetNode(Node2);

            var Pos1 = UnityNode1.Position;
            var Pos2 = UnityNode2.Position;

            return Vector3.Distance(Pos1, Pos2);
        }
    }


    [Serializable]
    public class UnityGraph : MonoBehaviour, ISerializationCallbackReceiver
    {
        [NonSerialized]
        public SparseGraph<UnityNode, UnityEdge> Graph = new SparseGraph<UnityNode, UnityEdge>();
        public byte[] SerializedGraph;
        public LayerMask WallLayerMask;
        public Transform WalkableNodePrefab;
        public Transform UnWalkableNodePrefab;

        private Transform GraphNodePrefabsRoot;

        private IHeuristic<SparseGraph<UnityNode, UnityEdge>> Heuristic = new UnityDistanceHeuristic();

        public List<UnityNode> VisitedNodes;

        public List<UnityPathPlanner> Searches = new List<UnityPathPlanner>();

        public string Name = "UnityGraph";
        public int NumTilesX = 25;
        public int NumTilesY = 25;
        public int TileWidth = 10;      // TODO: What units do these represent?
        public int TileHeight = 10;     // TODO: What units do these represent?
        public float TilePadding = .01f;
        public float WalkableRadius = 0.25f;

        public Color TileColor;
        public Color EdgeColor;

        public Color WalkableColor;
        public Color BlockedColor;

        public Color DefaultRayColor;
        public Color RayHitColor;

        public float PathSphereSize = 1.0f;

        public bool bInGame_ShowGraphNodes = true;

        public bool DrawNodeIndex = true;
        public bool DrawEdges = true;
        public bool DrawWalkable = true;
        public bool DrawSearchPaths = true;
        public int StartNodeIndex = 0;
        public int EndNodeIndex = 0;

        private bool bEnabled = true;

        int Width = 0;
        int Height = 0;

        void OnEnable()
        {
            bEnabled = true;
        }

        void OnDisable()
        {
            bEnabled = false;
        }

        public void RemoveNode(int NodeIndex)
        {
            if (Graph == null)
                return;

            Graph.RemoveNode(NodeIndex);
        }

        void Update()
        {
            GraphNodePrefabsRoot.gameObject.SetActive(bInGame_ShowGraphNodes);
         
        }

        void Start()
        {
            GraphNodePrefabsRoot = transform.GetChild(0);

            foreach (var Node in Graph.Nodes)
            {
                UnityNode GraphNode = (UnityNode)Graph.GetNode(Node.NodeIndex);
                Vector3 NodePosition = new Vector3(transform.position.x + Node.Position.x, transform.position.y + Node.Position.y + 0.02f, transform.position.z + Node.Position.z);

                if (Node.NodeIndex != (int)ENodeType.InvalidNodeIndex)
                {
                    // Vector3 CubeSize = new Vector3(TileWidth * (1 - TilePadding), .01f, TileHeight * (1 - TilePadding));
                    Node.GraphNodePrefab = Instantiate(WalkableNodePrefab, NodePosition, Quaternion.identity, GraphNodePrefabsRoot);
                 //   Node.GraphNodePrefab.GetComponent<VisualUnityNode>().UnityNode = Node;
                }
                else
                {
                    // Node.GraphNodePrefab.GetComponent<GraphNode>().NodeIndex = (int)ENodeType.InvalidNodeIndex;
                    Node.GraphNodePrefab = Instantiate(UnWalkableNodePrefab, NodePosition, Quaternion.identity, GraphNodePrefabsRoot);
                }
            }
        }
        
        void OnGUI()
        {
        }

#if UNITY_EDITOR_WIN
            static void DrawString(string text, Vector3 worldPos, Color? colour = null)
        {
            UnityEditor.Handles.BeginGUI();
            if (colour.HasValue) GUI.color = colour.Value;
            var view = UnityEditor.SceneView.currentDrawingSceneView;
            Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height, size.x, size.y), text);
            UnityEditor.Handles.EndGUI();
        }

        private void OnDrawGizmos()
        {
            // Scene view debug drawing here...
            if (Graph == null || !bEnabled)
                return;

            foreach (var Node in Graph.Nodes)
            {
                UnityNode GraphNode = (UnityNode)Graph.GetNode(Node.NodeIndex);

                if (DrawNodeIndex && GraphNode != null)
                {
                    var StringPosition = new Vector3(transform.position.x + Node.Position.x, transform.position.y + Node.Position.y, transform.position.z + Node.Position.z);
                    StringPosition.y += 0.5f;
                    DrawString(Node.NodeIndex.ToString(), StringPosition, Color.white);
                }

                if (DrawEdges && GraphNode != null)
                {
                    foreach (var Edge in Graph.Edges[Node.NodeIndex])
                    {
                        var FromNode = Graph.GetNode(Edge.FromNodeIndex) as UnityNode;
                        var ToNode = Graph.GetNode(Edge.ToNodeIndex) as UnityNode;
                        var FromPosition = new Vector3(transform.position.x + FromNode.Position.x, transform.position.y, transform.position.z + FromNode.Position.z);
                        var ToPosition = new Vector3(transform.position.x + ToNode.Position.x, transform.position.y, transform.position.z + ToNode.Position.z);

                        Gizmos.color = EdgeColor;
                        Gizmos.DrawLine(FromPosition, ToPosition);
                    }
                }

                if (DrawWalkable)
                {
                    Gizmos.color = WalkableColor;

                    if (Node.NodeIndex == (int)ENodeType.InvalidNodeIndex)
                    {
                        Gizmos.color = BlockedColor;
                    }
     
                    Vector3 CubePosition = new Vector3(transform.position.x + Node.Position.x, transform.position.y + Node.Position.y, transform.position.z + Node.Position.z);
                    Vector3 CubeSize = new Vector3(TileWidth * (1 - TilePadding), .01f, TileHeight * (1 - TilePadding));
                    Gizmos.DrawCube(CubePosition, CubeSize);
                }
            }
        }
#endif

        #region Grid Graph Stuff
        public void RemoveUnWalkableNodesAndEdges()
        {
            float RayLength = 2.0f;

            var NodesToRemove = new List<int>();
            var EdgesToRemove = new List<UnityEdge>();

            foreach (var Node in Graph.Nodes)
            {
                RaycastHit Hit;
                var GraphNode = Graph.GetNode(Node.NodeIndex);
                var Origin = GraphNode.Position + (Vector3.up * RayLength);
                var bHitSomething = Physics.SphereCast(Origin, WalkableRadius, Vector3.down, out Hit, RayLength, WallLayerMask);

                if (bHitSomething)
                {
                    NodesToRemove.Add(Node.NodeIndex);
                    Debug.DrawRay(Origin, Vector3.down * RayLength, RayHitColor, 10);
                }
                else
                {
                    Debug.DrawRay(Origin, Vector3.down * RayLength, DefaultRayColor, 5);

                    // Check to see if we can move to the end of any edges attached to this node
                    foreach (var Edge in Graph.Edges[Node.NodeIndex])
                    {
                        var FromNode = Graph.GetNode(Edge.FromNodeIndex);
                        var ToNode = Graph.GetNode(Edge.ToNodeIndex);

                        var FromNodePosition = FromNode.Position;
                        FromNodePosition.y += 1;

                        var Direction = Vector3.Normalize(ToNode.Position - FromNode.Position);
                        var bHitWall = Physics.CapsuleCast(FromNodePosition, FromNodePosition + Vector3.up * 0.25f, WalkableRadius, Direction, 1.25f, WallLayerMask);

                        if (bHitWall)
                        {
                            EdgesToRemove.Add(Graph.GetEdge(FromNode.NodeIndex, ToNode.NodeIndex));
                            EdgesToRemove.Add(Graph.GetEdge(ToNode.NodeIndex, FromNode.NodeIndex));
                        }
                    }

                    foreach (var Edge in EdgesToRemove)
                    {
                        Graph.RemoveEdge(Edge.FromNodeIndex, Edge.ToNodeIndex);
                        Graph.RemoveEdge(Edge.ToNodeIndex, Edge.FromNodeIndex);
                    }
                }
            }

            foreach (var NodeIndex in NodesToRemove)
            {
                Graph.RemoveNode(NodeIndex);
            }
        }


        public void Rebuild()
        {
            // Reset the graph (resets nodes and edges connecting them)
            ResetGraph();
        }

        public void ResetGraph()
        {
            Graph = new SparseGraph<UnityNode, UnityEdge>(false);
            VisitedNodes = new List<UnityNode>();

            Width = TileWidth * NumTilesX;
            Height = TileHeight * NumTilesY;

            float MidX = TileWidth / 2;
            float MidY = TileHeight / 2;

            for (int Row = 0; Row < NumTilesY; ++Row)
            {
                for (int Col = 0; Col < NumTilesX; ++Col)
                {
                    var NodeIndex = Graph.AddNode(new UnityNode(Graph.GetNextFreeNodeIndex(), new Vector3(MidX + (Col * TileWidth), 0, MidY + (Row * TileWidth))));
                    Graph.Edges.Insert(NodeIndex, new List<UnityEdge>());
                }
            }

            for (int Row = 0; Row < NumTilesY; ++Row)
            {
                for (int Col = 0; Col < NumTilesX; ++Col)
                {
                    AddAllNeighborsToGridNode(Row, Col, NumTilesX, NumTilesY);
                }
            }
        }

        public static bool ValidNeighbor(int x, int y, int NumCellsX, int NumCellsY)
        {
            return !((x < 0) || (x >= NumCellsX) || (y < 0) || (y >= NumCellsY));
        }

        public void AddAllNeighborsToGridNode(int Row, int Col, int CellsX, int CellsY)
        {
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    int NodeX = (Col + j);
                    int NodeY = (Row + i);

                    if ((i == 0) && (j == 0))
                        continue;

                    if (ValidNeighbor(NodeX, NodeY, CellsX, CellsY))
                    {
                        var Node = (UnityNode)Graph.GetNode(Row * CellsX + Col);

                        if (Node.NodeIndex == -(int)ENodeType.InvalidNodeIndex)
                            continue;

                        var NeighborNode = (UnityNode)Graph.GetNode(NodeY * CellsX + NodeX);

                        if (NeighborNode.NodeIndex == (int)ENodeType.InvalidNodeIndex)
                            continue;

                        var PosNode = new Vector3(Node.X, 1, Node.Y);
                        var PosNeighborNode = new Vector3(NeighborNode.X, 1, NeighborNode.Y);

                        double Distance = Vector3.Distance(PosNode, PosNeighborNode);
                        var NewEdge = new UnityEdge(Node.NodeIndex, NeighborNode.NodeIndex, Distance);

                        Graph.AddEdge(NewEdge);

                        if (!Graph.IsDigraph())
                        {
                            UnityEdge Edge = new UnityEdge(NeighborNode.NodeIndex, Node.NodeIndex, Distance);
                            Graph.AddEdge(Edge);
                        }
                    }
                }
            }
        }
        #endregion

        public Vector3 WorldToLocal(Vector3 WorldPosition)
        {
            return transform.parent.InverseTransformPoint(WorldPosition);
        }

        public Vector3 WorldToLocalTile(Vector3 WorldPosition)
        {
            var LocalPosition = transform.parent.InverseTransformPoint(WorldPosition);

            // Snap to closest grid point
            return new Vector3(Mathf.Round((LocalPosition.x * TileWidth) / TileWidth), Mathf.Round((LocalPosition.y * TileWidth) / TileWidth), Mathf.Round((LocalPosition.z * TileHeight) / TileHeight));
        }

        public UnityNode GetNodeAtPosition(Vector3 Position)
        {
            int NodeIndex = ((int)(Position.z / TileHeight) * NumTilesX) + ((int)Position.x / TileWidth);
            var Node = Graph.GetNode(NodeIndex);
            return Node;
        }

        #region Serialization

        // This is called when this object is selected in the editor -- it serializes the 
        // graph into a byte array during property editing.  This allows us to work on our graph in edit mode.
        public void OnBeforeSerialize()
        {
            var StopWatch = new System.Diagnostics.Stopwatch();
            StopWatch.Start();

            BinaryFormatter bf = new BinaryFormatter();

            foreach (var Node in Graph.Nodes)
            {
                Node.OnBeforeSerialize();
            }

            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, Graph);
                SerializedGraph = ms.ToArray();
            }

            StopWatch.Stop();
            // Debug.Log("OnBeforeSerialize: " + StopWatch.Elapsed);
        }

        public void OnAfterDeserialize()
        {
            if (SerializedGraph == null)
                return;

            var StopWatch = new System.Diagnostics.Stopwatch();
            StopWatch.Start();

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(SerializedGraph))
            {
                Graph = (SparseGraph<UnityNode, UnityEdge>)bf.Deserialize(ms);
                foreach (var Node in Graph.Nodes)
                {
                    Node.OnAfterDeserialize();
                }
            }
            StopWatch.Stop();

            //   Debug.Log("OnAfterDeserialize: " + StopWatch.Elapsed);
        }
    }
    #endregion

}