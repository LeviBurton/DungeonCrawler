using System.Collections;
using System.Collections.Generic;
using Burton.Lib.Graph;
using UnityEngine;
using System;

[Serializable]
public class UnityEdge : GraphEdge, ISerializationCallbackReceiver
{
    public UnityEdge() { }

    public UnityEdge(int FromNodeIndex, int ToNodeIndex, double EdgeCost)
        :base(FromNodeIndex, ToNodeIndex, EdgeCost)
    {
    }

    public void OnAfterDeserialize()
    {
        throw new NotImplementedException();
    }

    public void OnBeforeSerialize()
    {
        throw new NotImplementedException();
    }
}
