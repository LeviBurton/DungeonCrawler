using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Burton.Lib.Graph
{
    [Serializable]
    public class NavGraphNode : GraphNode
    {
        public Vector3 position;
        public NodeView nodeView;
        public NodeType nodeType;
        public int xIndex;
        public int yIndex;

        public float X;
        public float Y;
        public float Z;
            
        public NavGraphNode() { }

        public NavGraphNode(int Index, Vector3 pos)
        {
            this.NodeIndex = Index;
            this.position = pos;
        }

        public NavGraphNode(int Index, float LocationX, float LocationY, float LocationZ)
        {
            this.NodeIndex = Index;
         
        }
    }
}
