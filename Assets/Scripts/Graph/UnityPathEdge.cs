using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityPathEdge
{
    public Vector3 FromPosition;
    public Vector3 ToPosition;

    public int Behavior;

    public UnityPathEdge(Vector3 From, Vector3 To, int Behavior)
    {
        this.FromPosition = From;
        this.ToPosition = To;
        this.Behavior = Behavior;
    }
}
