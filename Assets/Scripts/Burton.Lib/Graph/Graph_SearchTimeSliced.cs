using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burton.Lib.Graph
{
    public abstract class Graph_SearchTimeSliced<TEdgeType>
    {
        public enum ESearchType { AStar, Dijkstra };

        private ESearchType SearchType;

        public Graph_SearchTimeSliced(ESearchType Type)
        {
            SearchType = Type;
        }

        public abstract ESearchStatus CycleOnce();
        public abstract List<TEdgeType> GetSPT();
        public abstract double GetCostToTarget();
        public abstract List<int> GetPathToTarget();
        public abstract List<PathEdge> GetPathAsPathEdges();
        public ESearchType GetSearchType() { return SearchType; }

    }
}
