using Burton.Lib.Alg;
using System.Collections.Generic;

namespace Burton.Lib.Graph
{
    public class GraphSearchDijkstra
    {
       public enum NodeStatus { Visited, Unvisited, NoParentAssigned };

        SparseGraph<GraphNode, GraphEdge> Graph;
        public List<GraphEdge> ShortestPathTree;
        public List<GraphEdge> SearchFrontier;
        public List<double> CostToThisNode;

        public int SourceNodeIndex;
        public int TargetNodeIndex;

        public bool bFound;

        public GraphSearchDijkstra(SparseGraph<GraphNode,GraphEdge> Graph, int Source, int Target)
        {
            this.Graph = Graph;
            this.bFound = false;

            SourceNodeIndex = Source;
            TargetNodeIndex = Target;

            int ActiveNodeCount = Graph.ActiveNodeCount();
            int NodeCount = Graph.NodeCount();

            ShortestPathTree = new List<GraphEdge>(NodeCount);
            SearchFrontier = new List<GraphEdge>(NodeCount);
            CostToThisNode = new List<double>(ActiveNodeCount);
        
            for (int i = 0; i < NodeCount; i++)
            {
                ShortestPathTree.Insert(i, null);
                SearchFrontier.Insert(i, null);
                CostToThisNode.Insert(i, 0);
            }

        }

        public bool Search()
        {
            var Q = new IndexedPriorityQueueLow<double>(CostToThisNode, Graph.NodeCount());

            Q.Insert(SourceNodeIndex);
           
            while (!Q.IsEmpty())
            {
                int NextClosestNode = Q.Pop();

                ShortestPathTree[NextClosestNode] = SearchFrontier[NextClosestNode];

                if (NextClosestNode == TargetNodeIndex)
                {
                    bFound = true;
                    return true;
                }

                foreach (var Edge in Graph.Edges[NextClosestNode])
                {
                    double NewCost = CostToThisNode[NextClosestNode] + Edge.EdgeCost;

                    if (SearchFrontier[Edge.ToNodeIndex] == null)
                    {
                        CostToThisNode[Edge.ToNodeIndex] = NewCost;
                        Q.Insert(Edge.ToNodeIndex);
                        SearchFrontier[Edge.ToNodeIndex] = Edge;
                    }
                    else if ( (NewCost < CostToThisNode[Edge.ToNodeIndex]) &&
                              (ShortestPathTree[Edge.ToNodeIndex] == null) )
                    {
                        CostToThisNode[Edge.ToNodeIndex] = NewCost;
                        Q.ChangePriority(Edge.ToNodeIndex);
                        SearchFrontier[Edge.ToNodeIndex] = Edge;
                    }
                }
            }
            
            return false;
        }

        public List<int> GetPathToTarget()
        {
            var Path = new List<int>(Graph.NodeCount());

            if (TargetNodeIndex < 0)
                return Path;

            int Node = TargetNodeIndex;

            Path.Insert(0, Node);

            while ( (Node != SourceNodeIndex) && (ShortestPathTree[Node] != null) && (Path.Count < Graph.NodeCount() ) )
            {
                Node = ShortestPathTree[Node].FromNodeIndex;
                Path.Insert(0, Node);
            }
            
            return Path;
        }

        public double GetCostToTarget()
        {
            return CostToThisNode[TargetNodeIndex];
        }

        public double GetCostToNode(int NodeIndex)
        {
            return CostToThisNode[NodeIndex];
        }
    }
}
