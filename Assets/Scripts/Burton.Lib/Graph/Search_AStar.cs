using System;
using System.Collections.Generic;
using System.Text;
using Burton.Lib.Alg;

namespace Burton.Lib.Graph
{
    public class Search_AStar<TNode, TEdge> where TNode : GraphNode 
                                            where TEdge : GraphEdge
    {
       public enum NodeStatus { Visited, Unvisited, NoParentAssigned };

        SparseGraph<TNode, TEdge> Graph;
        IHeuristic<SparseGraph<TNode, TEdge>> Heuristic;

        public List<TEdge> ShortestPathTree;
        public List<TEdge> SearchFrontier;

        public List<double> CostToThisNode;
        public List<double> GCosts;
        public List<double> FCosts;

        public int SourceNodeIndex;
        public int TargetNodeIndex;
        public bool bFound;
        private IndexedPriorityQueueLow<double> TimeSlicedQ;

        public Search_AStar(SparseGraph<TNode, TEdge> Graph, IHeuristic<SparseGraph<TNode, TEdge>> Heuristic, int Source, int Target)
        {
            this.Graph = Graph;
            this.bFound = false;
            this.Heuristic = Heuristic;

            SourceNodeIndex = Source;
            TargetNodeIndex = Target;

            int ActiveNodeCount = Graph.ActiveNodeCount();
            int NodeCount = Graph.NodeCount();

            ShortestPathTree = new List<TEdge>(NodeCount);
            SearchFrontier = new List<TEdge>(NodeCount);
            CostToThisNode = new List<double>(NodeCount);
            GCosts = new List<double>(NodeCount);
            FCosts = new List<double>(NodeCount);

            // not sure i need to initialize these...nt);
            for (int i = 0; i < NodeCount; i++)
            {
                ShortestPathTree.Insert(i, null);
                SearchFrontier.Insert(i, null);
                CostToThisNode.Insert(i, 0);
                FCosts.Insert(i, 0);
                GCosts.Insert(i, 0);
            }

            TimeSlicedQ = new IndexedPriorityQueueLow<double>(FCosts, Graph.NodeCount());
            TimeSlicedQ.Insert(SourceNodeIndex);
        }

        public ESearchStatus CycleOnce()
        {
            if (SourceNodeIndex > Graph.NodeCount())
                return ESearchStatus.TargetNotFound;

            if (TimeSlicedQ.IsEmpty())
            {
                return ESearchStatus.TargetNotFound;
            }

            int NextClosestNode = TimeSlicedQ.Pop();

            ShortestPathTree[NextClosestNode] = SearchFrontier[NextClosestNode];

            if (NextClosestNode == TargetNodeIndex)
            {
                return ESearchStatus.TargetFound;
            }

            foreach (var Edge in Graph.Edges[NextClosestNode])
            {
                double HCost = Heuristic.Calculate(Graph, TargetNodeIndex, Edge.ToNodeIndex);
                double GCost = GCosts[NextClosestNode] + Edge.EdgeCost;

                if (SearchFrontier[Edge.ToNodeIndex] == null)
                {
                    FCosts[Edge.ToNodeIndex] = GCost + HCost;
                    GCosts[Edge.ToNodeIndex] = GCost;
                    TimeSlicedQ.Insert(Edge.ToNodeIndex);
                    SearchFrontier[Edge.ToNodeIndex] = Edge;
                }
                else if (GCost < GCosts[Edge.ToNodeIndex] && 
                        (ShortestPathTree[Edge.ToNodeIndex] == null))
                {
                    FCosts[Edge.ToNodeIndex] = GCost + HCost;
                    GCosts[Edge.ToNodeIndex] = GCost;
                    TimeSlicedQ.ChangePriority(Edge.ToNodeIndex);
                    SearchFrontier[Edge.ToNodeIndex] = Edge;
                }
            }

            return ESearchStatus.SearchIncomplete;
        }

        public bool Search()
        {
            var Q = new IndexedPriorityQueueLow<double>(FCosts, Graph.NodeCount());

            if (SourceNodeIndex > Graph.NodeCount())
                return false;

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
                    double HCost = Heuristic.Calculate(Graph, TargetNodeIndex, Edge.ToNodeIndex);
                    double GCost = GCosts[NextClosestNode] + Edge.EdgeCost;

                    if (SearchFrontier[Edge.ToNodeIndex] == null)
                    {
                        FCosts[Edge.ToNodeIndex] = GCost + HCost;
                        GCosts[Edge.ToNodeIndex] = GCost;
                        Q.Insert(Edge.ToNodeIndex);
                        SearchFrontier[Edge.ToNodeIndex] = Edge;
                    }
                    //else if ( (GCost < GCosts[Edge.ToNodeIndex]) &&
                    //          (ShortestPathTree[Edge.ToNodeIndex] == null) )
                    //{
                          else if (GCost < GCosts[Edge.ToNodeIndex])
                        {
                            FCosts[Edge.ToNodeIndex] = GCost + HCost;
                        GCosts[Edge.ToNodeIndex] = GCost;
                        Q.ChangePriority(Edge.ToNodeIndex);
                        SearchFrontier[Edge.ToNodeIndex] = Edge;
                    }
                }
            }
            
            return false;
        }

        public List<int> GetPathToTarget()
        {
            var Path = new List<int>();

            if (TargetNodeIndex < 0)
                return Path;

            int CurNodeIndex = TargetNodeIndex;

            Path.Insert(0, CurNodeIndex);

            int NodeCount = Graph.NodeCount();

            while ((CurNodeIndex != SourceNodeIndex) && (ShortestPathTree[CurNodeIndex] != null) && Path.Count <= NodeCount) 
            {
                CurNodeIndex = ShortestPathTree[CurNodeIndex].FromNodeIndex;
                Path.Insert(0, CurNodeIndex);
            }
            
            return Path;
        }

        public double GetCostToTarget()
        {
            return GCosts[TargetNodeIndex];
        }

        public double GetCostToNode(int NodeIndex)
        {
            return CostToThisNode[NodeIndex];
        }

        public List<PathEdge> GetPathAsPathEdges()
        {
            List<PathEdge> Path = new List<PathEdge>();

            if (TargetNodeIndex < 0) return Path;

            int Node = TargetNodeIndex;

            while ((Node != SourceNodeIndex) && (ShortestPathTree[Node] != null) && Path.Count <= Graph.NodeCount())
            {
                Path.Add(new PathEdge(ShortestPathTree[Node].FromNodeIndex, ShortestPathTree[Node].ToNodeIndex, 0, 0));
                Node = ShortestPathTree[Node].FromNodeIndex;
            }

            return Path;
        }
    }
}
