using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tiles/TerrainCosts")]
public class TerrainCosts : SerializedScriptableObject
{
    [SerializeField]
    public Dictionary<NodeType, float> costs = new Dictionary<NodeType, float>();

    public float GetCost(NodeType nodeType)
    {
        if (costs.ContainsKey(nodeType))
        {
            return costs[nodeType];
        }
        else
        {
            throw new System.Exception("No cost found for " + nodeType.ToString());
        }
    }
}
