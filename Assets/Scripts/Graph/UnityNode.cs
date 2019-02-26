using Burton.Lib.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnityNode : NavGraphNode, ISerializationCallbackReceiver
{
    [NonSerialized]
    public Vector3 Position = new Vector3();

    public List<GameObject> GameObjects = new List<GameObject>();

    [NonSerialized]
    public Transform GraphNodePrefab;

    public UnityNode(int NodeIndex, Vector3 Position)
        : base(NodeIndex, Position.x, Position.z, Position.y)
    {
        this.Position = Position;
    }

    public void OnBeforeSerialize()
    {
        Y = Position.y;
        X = Position.x;
        Z = Position.z;
    }

    public void OnAfterDeserialize()
    {
        Position.x = X;
        Position.y = Y;
        Position.z = Z;
    }
}
