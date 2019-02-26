using Burton.Lib.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityHeuristicEuclid<T> : IHeuristic<T> where T : SparseGraph<UnityNode, UnityEdge>
{
    public double Calculate(T Graph, int Start, int End)
    {
        return 0;
    }
}
