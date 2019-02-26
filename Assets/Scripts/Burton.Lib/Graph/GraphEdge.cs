using System;
using System.Collections.Generic;

using System.Text;

namespace Burton.Lib.Graph
{
    [Serializable]
    public class GraphEdge
    {
        // Default Accessor Implementation
        public int this[int index]
        {
            get
            {

                return 0;
            }
            set { /* Do Nothing */ }
        }

        /// <summary>
        /// An edge connects two nodes.  Valid node indices are always positive
        /// </summary>
        public int FromNodeIndex;
        public int ToNodeIndex;

        /// <summary>
        /// The cost of traversing the Edge
        /// </summary>
        public double EdgeCost;


        public GraphEdge(int FromNodeIndex, int ToNodeIndex, double EdgeCost)
        {
            this.FromNodeIndex = FromNodeIndex;
            this.ToNodeIndex = ToNodeIndex;
            this.EdgeCost = EdgeCost;
        }

        public GraphEdge(int FromNodeIndex, int ToNodeIndex)
        {
            this.FromNodeIndex = FromNodeIndex;
            this.ToNodeIndex = ToNodeIndex;
            this.EdgeCost = 1.0f;
        }

        public GraphEdge()
        {
            this.FromNodeIndex = (int)NodeType.InvalidNodeIndex;
            this.ToNodeIndex = (int)NodeType.InvalidNodeIndex;
            this.EdgeCost = 1.0f;
        }
    }
}
