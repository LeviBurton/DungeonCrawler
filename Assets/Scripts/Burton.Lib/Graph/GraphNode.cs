using System;
using System.Collections.Generic;

using System.Text;

namespace Burton.Lib.Graph
{ 
  
    [Serializable]
    public class GraphNode
    {
        int _NodeIndex;
        public int NodeIndex
        {
            get { return _NodeIndex; }
            set { _NodeIndex = value; }
        }

        public GraphNode() { NodeIndex = (int)ENodeType.InvalidNodeIndex; }
        public GraphNode(int Index) { NodeIndex = Index; }
    }
}
