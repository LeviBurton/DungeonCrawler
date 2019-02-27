using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(menuName = "Tiles/TileColors")]
public class TileColors : ScriptableObject
{
    public Color openColor = new Color(1f, 1f, 1f);
    public Color blockColor = new Color(0f, 0f, 0f);
    public Color lightTerrainColor = new Color(0f, 1f, 0f);
    public Color mediumTerrainColor = new Color(1f, 1f, 0f);
    public Color heavyTerrainColor = new Color(1f, 0.75f, 0f);
    public Color waterColor = new Color(0f, 0f, 1f);
    public Color lavaColor = new Color(1f, 0f, 0f);
    public Color connectorColor = new Color(1f, 1f, 1f);
    public Color portalColor = new Color(0f, 1f, 0f, 1f);
    public Color entranceColor = new Color(0f, 1f, 0f, 1f);
    public Color exitColor = new Color(0f, 1f, 0f, 1f);

    public  Color GetNodeTypeColor(NodeType nodeType)
    {
        switch (nodeType)
        {
            case NodeType.Open:
                return openColor;
            case NodeType.Blocked:
                return blockColor;
            case NodeType.LightTerrain:
                return lightTerrainColor;
            case NodeType.MediumTerrain:
                return mediumTerrainColor;
            case NodeType.HeavyTerrain:
                return heavyTerrainColor;
            case NodeType.Water:
                return waterColor;
            case NodeType.Lava:
                return lavaColor;
            case NodeType.Connector:
                return connectorColor;
            case NodeType.Portal:
                return portalColor;
            case NodeType.Entrance:
                return entranceColor;
            case NodeType.Exit:
                return exitColor;
            default:

                return openColor;
        }
    }

}
