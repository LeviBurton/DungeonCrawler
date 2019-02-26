using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burton.Lib.Graph
{
    public class PathEdge
    {
        public int FromIndex;
        public int ToIndex;
        public int Behavior;

        // FIXME: what is a door
        public int DoorID;

        public PathEdge(int FromIndex, int ToIndex, int Behavior, int DoorID = 0)
        {
            this.FromIndex = FromIndex;
            this.ToIndex = ToIndex;
            this.Behavior = Behavior;
            this.DoorID = DoorID;
        }
    }
}
