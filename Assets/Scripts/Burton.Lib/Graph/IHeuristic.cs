using System;
using System.Collections.Generic;
using System.Text;

namespace Burton.Lib.Graph
{
    public interface IHeuristic<TGraph>
    {
        double Calculate(TGraph Graph, int Node1, int Node2);
    }
}
