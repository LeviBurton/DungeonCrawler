using System;
using System.Collections.Generic;

namespace Burton.Lib.Graph
{
    [Serializable]
    public class AdjacencyList
    {
        public List<GraphEdge>[] EdgeVector;
        
        public AdjacencyList() { }

        // ctor: create empty adjacency list
        public AdjacencyList(int Vertices) 
        {
            EdgeVector = new List<GraphEdge>[Vertices];

            for (int i = 0; i < EdgeVector.Length; ++i)
            {
                EdgeVector[i] = new List<GraphEdge>();
            }
        }

        // Appends an edge to the linked list
        public void AddEdgeAtEnd(GraphEdge Edge)
        {
            //  EdgeVector[Edge.FromNodeIndex].AddLast(Edge);
            if (!EdgeVector[Edge.FromNodeIndex].Contains(Edge))
            {
                EdgeVector[Edge.FromNodeIndex].Add(Edge);
            }
        }

        // Adds a new Edge to the linked list from the front
        public void AddEdgeAtBegin(GraphEdge Edge)
        {
        //    EdgeVector[Edge.FromNodeIndex].AddFirst(Edge);
            EdgeVector[Edge.FromNodeIndex].Insert(0, Edge);
        }

        public GraphEdge GetEdge(int From, int To)
        {
            GraphEdge EdgeToReturn = null;

            foreach (var Edge in EdgeVector[From])
            {
                if (Edge.ToNodeIndex == To)
                {
                    EdgeToReturn = Edge;
                }
            }

            return EdgeToReturn;
        }

        public void RemoveNodeEdges(int From)
        {
            // Foreach edge leading from this node...
            foreach (var FromEdge in EdgeVector[From])
            {
                //// get the list of edges it is pointing to
                //var ToEdge = EdgeVector[FromEdge.ToNodeIndex][0];
                var EdgesToRemove = new List<GraphEdge>();

                // foreach edge it is pointing to, check if it is us.
                foreach (var ToEdge in EdgeVector[FromEdge.ToNodeIndex])
                {
                    // if the node it is pointg to is the node we want to delete, then delete that edge from the node.
                    if (ToEdge.ToNodeIndex == From)
                    {
                        EdgesToRemove.Add(ToEdge);
                    }
                }

                foreach (var EdgeToRemove in EdgesToRemove)
                {
                    EdgeVector[FromEdge.ToNodeIndex].Remove(EdgeToRemove);
                }
            }

            // Clear the nodes edges
            EdgeVector[From].Clear();
        }

        public void RemoveEdge(int From, int To)
        {
            GraphEdge EdgeToRemove = null;

            foreach (var Edge in EdgeVector[From])
            {
                if (Edge.ToNodeIndex == To)
                {
                    EdgeToRemove = Edge;
                    break;
                }
            }

            EdgeVector[From].Remove(EdgeToRemove);
        }

        // Removmes the first occurence of an edge and returns true if there was any change 
        // in the collection, otherwise false.
        public bool RemoveEdge(GraphEdge Edge)
        {
            return EdgeVector[Edge.FromNodeIndex].Remove(Edge);
        }

        // Returns the number of vertices
        public int NumVertices()
        {
            return EdgeVector.Length;
        }

        // Returns a copy of the linked list of outward edges from a vertex
        public List<GraphEdge> this[int index]
        {
            get
            {
                List<GraphEdge> EdgeList = new List<GraphEdge>(EdgeVector[index]);
                return EdgeList;
            }
        }

        public void PrintAdjacencyList()
        {
            int i = 0;
            foreach (List<GraphEdge> list in EdgeVector)
            {
                Console.Write("AdjacencyList[" + i + "] -> ");
                foreach(GraphEdge edge in list)
                {
                    Console.Write(edge.FromNodeIndex + "(" + edge.ToNodeIndex + ")");
                }
                ++i;
                Console.WriteLine();
            }
        }

    }
}
