using System;
using System.Collections.Generic;
using UnityEngine;

namespace Burton.Lib.Graph
{
    public enum ENodeType { InvalidNodeIndex = -1 }

    [Serializable]
    public class SparseGraph<TNode, TEdge> where TNode : GraphNode
                                           where TEdge : GraphEdge
    {
        public SparseGraph()
        {
          //  Edges.Add(new List<TEdge>());
        }

        public List<TNode> Nodes = new List<TNode>();

        // Graph edges -- this is maintained as a list of edges indexed by node id.
        public List<List<TEdge>> Edges = new List<List<TEdge>>();

        /// <summary>
        /// Is this a directed graoh?
        /// </summary>
        private bool bIsDigraph;

        /// <summary>
        /// The index of the next node to be added;
        /// </summary>
        private int NextNodeIndex;


        /// <summary>
        /// Returns the node at the given index
        /// </summary>
        /// <param name="NodeIndex"></param>
        /// <returns>Node at nodeIndex</returns>
        public TNode GetNode(int NodeIndex)
        {
            if (NodeIndex >= Nodes.Count || NodeIndex == -1)
            {
                return null;
            }

            TNode node = null;

            try
            {
                node = Nodes[NodeIndex];
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("GetNode() {0} {1}", NodeIndex, ex.ToString());
                
            }

            return node;
        }

        /// <summary>
        /// Returns the edge From To
        /// </summary>
        /// <param name="From"></param>
        /// <param name="To"></param>
        /// <returns>The edge connecting From to To</returns>
        public TEdge GetEdge(int From, int To)
        {
            TEdge EdgeToReturn = null;

            foreach (var Edge in Edges[From])
            {
                if (Edge.ToNodeIndex == To)
                {
                    EdgeToReturn = (TEdge) Edge;
                }
            }

            return (TEdge)EdgeToReturn;
        }

        /// <summary>
        /// Returns the next free node index
        /// </summary>
        /// <returns>Next valid node index</returns>
        public int GetNextFreeNodeIndex()
        {
            return NextNodeIndex;
        }

        public bool IsNodePresent(int NodeIndex)
        {
            if (Nodes[NodeIndex].NodeIndex == (int)ENodeType.InvalidNodeIndex || (NodeIndex >= Nodes.Count))
            {
                return false;
            }

            return true;
        }
       
        /// <summary>
        /// Adds a node to the graph and returns it
        /// </summary>
        /// <param name="Node"></param>
        /// <returns>New Node Index</returns>
        public int AddNode(TNode Node)
        {
            if (Node.NodeIndex < Nodes.Count)
            {
                if (Nodes[Node.NodeIndex].NodeIndex != (int)ENodeType.InvalidNodeIndex)
                {
                    throw new ArgumentException("Invalid Node Index: " + Node.NodeIndex);
                }

                Nodes[Node.NodeIndex] = Node;

                return Node.NodeIndex;
            }
            else
            {
                if (Node.NodeIndex != NextNodeIndex)
                {
                    throw new ArgumentException("Invalid Node Index: " + Node.NodeIndex);
                }

                Nodes.Add(Node);

                int nextNode = NextNodeIndex++;

               Edges.Insert(nextNode, new List<TEdge>());

                return nextNode;
            }
        }

        /// <summary>
        /// Sets the node at NodeIndex to invalid and removes any edges connected to this node.
        /// </summary>
        /// <param name="NodeIndex"></param>
        public void RemoveNode(int NodeIndex)
        {
            if (NodeIndex > Nodes.Count)
            {
                throw new ArgumentException("Invalid Node Index: " + NodeIndex);
            }

            Nodes[NodeIndex].NodeIndex = (int)ENodeType.InvalidNodeIndex;

            foreach (var FromEdge in Edges[NodeIndex])
            {
                var EdgesToRemove = new List<TEdge>();
                foreach (var ToEdge in Edges[FromEdge.ToNodeIndex])
                {
                    if (ToEdge.ToNodeIndex == NodeIndex)
                    {
                        EdgesToRemove.Add(ToEdge);
                    }
                }

                foreach (var EdgeToRemove in EdgesToRemove)
                {
                    Edges[FromEdge.ToNodeIndex].Remove(EdgeToRemove);
                }
            }

            Edges[NodeIndex].Clear();
        }

        public void AddEdge(TEdge Edge)
        {
            try
            {
                if (!Edges[Edge.FromNodeIndex].Contains(Edge))
                {
                    Edges[Edge.FromNodeIndex].Add(Edge);
                }
            }
            catch (Exception outOfRangeException)
            {
                Debug.LogFormat(outOfRangeException.Message);
            }
        }

        public void RemoveEdge(int From, int To)
        {
            TEdge EdgeToRemove = null;

            foreach (var Edge in Edges[From])
            {
                if (Edge.ToNodeIndex == To)
                {
                    EdgeToRemove = Edge;
                    break;
                }
            }

            Edges[From].Remove(EdgeToRemove);
        }

        public void SetEdgeCost(int From, int To, double Cost)
        {
            foreach (var Edge in Edges[From])
            {
                if (Edge.ToNodeIndex == To)
                {
                    Edge.EdgeCost = Cost;
                }
            }
        }

        /// <summary>
        /// Returns the number of active + inactive enodes present in the graph.
        /// </summary>
        /// <returns></returns>
        public int NodeCount()
        {
            return Nodes.Count;
        }

        // returns the number of active nodes present in the graph
        public int ActiveNodeCount()
        {
            int ActiveNodeCount = 0;

            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].NodeIndex != (int)ENodeType.InvalidNodeIndex)
                    ActiveNodeCount++;
            }

            return ActiveNodeCount;
        }

        /// <summary>
        /// Returns the number of edges in the graph
        /// </summary>
        /// <returns>Count of edges in the graph</returns>
        public int EdgeCount()
        {
            int EdgeCount = 0;
            for (int i = 0; i < NodeCount(); i++)
            {
                if (Nodes[i].NodeIndex != (int)ENodeType.InvalidNodeIndex)
                {
                    EdgeCount += Edges[Nodes[i].NodeIndex].Count;
                }
            }

            return EdgeCount;
        }

        // returns true if this is a directed graph
        public bool IsDigraph()
        {
            return bIsDigraph;
        }

        // returns true if the graph contains no nodes
        public bool IsEmpty()
        {
            return true;
        }

        // methods for loading saving graphs from an open file
        // we may not need to even worry about this -- 
        // i don't think ill ever need to load/save just the graph.
        public bool Save(string FileName)
        {

            return false;
        }

        public bool Load(string FileName)
        {
            int NumNodes, NumEdges;

            NumNodes = 100;
            NumEdges = 100 * 8;

            // Add nodes
            for (int n = 0; n < NumNodes; n++)
            {
                var NewNode = new GraphNode();
                if (NewNode.NodeIndex != (int)ENodeType.InvalidNodeIndex)
                {
                    AddNode((TNode)NewNode);
                }
                else
                {
                    Nodes.Add((TNode)NewNode);
                    Edges.Add(new List<TEdge>());
                    ++NextNodeIndex;
                }
            }

            // Add Edges

            return true;
        }

        public SparseGraph(bool bIsDigraph)
        {
            this.bIsDigraph = bIsDigraph;
            NextNodeIndex = 0;
            Edges = new List<List<TEdge>>();
        }
    }
}
