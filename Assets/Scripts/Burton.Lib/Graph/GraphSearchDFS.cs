using Burton.Lib.Graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace Burton.Lib.Graph
{
    public class GraphSearchDFS
    {
       public enum NodeStatus { Visited, Unvisited, NoParentAssigned };

        SparseGraph<GraphNode, GraphEdge> Graph;
        List<int> VisitedNodes;
        List<int> Route;

        public List<GraphEdge> SpanningTree;

        public int SourceNodeIndex;
        public int TargetNodeIndex;
        public bool bFound;

        public GraphSearchDFS(SparseGraph<GraphNode,GraphEdge> Graph, int Source, int Target)
        {
            this.Graph = Graph;
            this.bFound = false;
            this.SourceNodeIndex = Source;
            this.TargetNodeIndex = Target;

            SpanningTree = new List<GraphEdge>();

            VisitedNodes = new List<int>();
            for (int i = 0; i < Graph.NodeCount(); i++)
            {
                VisitedNodes.Insert(i, (int)NodeStatus.Unvisited);
            }

            Route = new List<int>(Graph.NodeCount());
            for (int i = 0; i < Graph.NodeCount(); i++)
            {
                Route.Insert(i, (int)NodeStatus.NoParentAssigned);
            }
        }

        public bool Search()
        {
            Stack<GraphEdge> Stack = new Stack<GraphEdge>();
            SpanningTree.Clear();
            bFound = false;
            GraphEdge Dummy = new GraphEdge(SourceNodeIndex, SourceNodeIndex, 0);

            Stack.Push(Dummy);

            while (Stack.Count > 0)
            {
                // pop the next edge from the stack of edges to examine
                GraphEdge Next = Stack.Pop();
                
                // make a note of the parent of the node this edge points to
                Route[Next.ToNodeIndex] = Next.FromNodeIndex;

                // ...and mark it visited
                VisitedNodes[Next.ToNodeIndex] = (int)NodeStatus.Visited;

                // keep track of which edges we have traversed to find a path
                if (Next != Dummy)
                {
                    SpanningTree.Add(Next);
                }

                // did we find the target?
                if (Next.ToNodeIndex == TargetNodeIndex)
                {
                    bFound = true;
                    return true;
                }

                // push edges leading from the node this edges points to onto the stack.
                foreach (var Edge in Graph.Edges[Next.ToNodeIndex])
                {
                    // if we havent visited this node, add it to the stack to be examined
                    if (VisitedNodes[Edge.ToNodeIndex] ==(int) NodeStatus.Unvisited)
                    {
                        Stack.Push(Edge);
                    }
                }
            }

            return false;
        }

        public Stack<int> GetPathToTarget()
        {
            var Path = new Stack<int>();

            if (!bFound || TargetNodeIndex < 0)
                return Path;

            int Node = TargetNodeIndex;

            Path.Push(Node);

            while (Node != SourceNodeIndex)
            {
                Node = Route[Node];
                Path.Push(Node);
            }

            return Path;
        }
    }
}
