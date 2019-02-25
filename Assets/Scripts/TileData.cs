using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: this class is getting really messy -- consider a redesign.
public class TileData : MonoBehaviour
{
    [OnValueChanged("OnColorChanged")]
    public static Color openColor = new Color(1f, 1f, 1f, 0.9f);

    [OnValueChanged("OnColorChanged")]
    public static Color blockColor = new Color(0f, 0f, 0f, 0.9f);

    [OnValueChanged("OnColorChanged")]
    public static Color lightTerrainColor = new Color(0f, 1f, 0f, 0.9f);

    [OnValueChanged("OnColorChanged")]
    public static Color mediumTerrainColor = new Color(1f, 1f, 0f, 0.9f);

    [OnValueChanged("OnColorChanged")]
    public static Color heavyTerrainColor = new Color(1f, 0.75f, 0f, 0.9f);

    [OnValueChanged("OnColorChanged")]
    public static Color waterColor = new Color(0f, 0f, 1f, 0.9f);

    [OnValueChanged("OnColorChanged")]
    public static Color lavaColor = new Color(1f, 0f, 0f, 0.9f);

    [OnValueChanged("OnColorChanged")]
    public static Color connectorColor = new Color(1f, 1f, 1f, 0.9f);

    public static Dictionary<Color, NodeType> terrainLookupTable = new Dictionary<Color, NodeType>();

    void OnColorChanged()
    {
        SetupLookupTable();
    }

    public void SetupLookupTable()
    {
        terrainLookupTable.Clear();

        terrainLookupTable.Add(openColor, NodeType.Open);
        terrainLookupTable.Add(blockColor, NodeType.Blocked);
        terrainLookupTable.Add(lightTerrainColor, NodeType.LightTerrain);
        terrainLookupTable.Add(mediumTerrainColor, NodeType.MediumTerrain);
        terrainLookupTable.Add(heavyTerrainColor, NodeType.HeavyTerrain);
        terrainLookupTable.Add(waterColor, NodeType.Water);
        terrainLookupTable.Add(lavaColor, NodeType.Lava);
        terrainLookupTable.Add(connectorColor, NodeType.Connector);
    }

    public int[,] MakeTileData(int width, int height)
    {
        int[,] map = new int[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, y] = (int)NodeType.Open;
            }
        }

        return map;
    }

    public static Color GetColorFromNodeType(NodeType nodeType)
    {
        if (terrainLookupTable.Count == 0)
        {
            terrainLookupTable.Clear();

            terrainLookupTable.Add(openColor, NodeType.Open);
            terrainLookupTable.Add(blockColor, NodeType.Blocked);
            terrainLookupTable.Add(lightTerrainColor, NodeType.LightTerrain);
            terrainLookupTable.Add(mediumTerrainColor, NodeType.MediumTerrain);
            terrainLookupTable.Add(heavyTerrainColor, NodeType.HeavyTerrain);
            terrainLookupTable.Add(waterColor, NodeType.Water);
            terrainLookupTable.Add(connectorColor, NodeType.Connector);
        }

        if (terrainLookupTable.ContainsValue(nodeType))
        {
            Color colorKey = terrainLookupTable.FirstOrDefault(x => x.Value == nodeType).Key;
            return colorKey;
        }

        return Color.white;
    }
}
