using System;
using System.Collections.Generic;

using System.Text;

namespace Burton.Lib.Graph
{ 
    public enum NodeType
    {
        InvalidNodeIndex = -1
    }

    [Serializable]
    public class GraphNode
    {
        int _NodeIndex;
        public int NodeIndex
        {
            get { return _NodeIndex; }
            set { _NodeIndex = value; }
        }

        public GraphNode() { NodeIndex = (int)NodeType.InvalidNodeIndex; }
        public GraphNode(int Index) { NodeIndex = Index; }
    }
}
