using Burton.Lib.Graph;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeView : MonoBehaviour
{
    public string tileId;

    public GameObject arrow;

    // TODO: this doesn't quite work.  most of the time it is null.
    NavGraphNode m_node;
    public TileColors tileColors;

    Vector2 m_coordinate;
    public Vector2 Coordinate { get { return Utility.Vector2Round(m_coordinate); } }
    public int xIndex;
    public int yIndex;
    public int nodeIndex;

    [Range(0, 0.5f)]
    public float borderSize = 0.15f;    // TODO: not currently used.

    Color nodeColor;
    Color emptyColor = new Color(0, 0, 0, 0);   // TODO: hack for editor coloring to work.
    GraphView ownerGraph;

    [OnValueChanged("NodeTypeChanged")]
    public NodeType nodeType;
    void NodeTypeChanged()
    {
        nodeColor = tileColors.GetNodeTypeColor(nodeType);
    }

    void Start()
    {
        ownerGraph = this.GetComponentInParent<GraphView>();
    }

    public void SetFromNode(NavGraphNode node)
    {
        var parent = this.GetComponentInParent<GraphView>();

        gameObject.name = "Node (" + node.xIndex + "," + node.yIndex + ")";
        gameObject.transform.position = new Vector3(node.position.x + 0.5f, node.position.y, node.position.z + 0.5f);
        gameObject.transform.localScale = new Vector3(1f - borderSize, 1f, 1f - borderSize);

        m_node = node;
        m_coordinate = new Vector2(xIndex, yIndex);

        nodeType = node.nodeType;
        nodeColor = parent.m_tileColors.GetNodeTypeColor(nodeType);
        xIndex = node.xIndex;
        yIndex = node.yIndex;

        EnableObject(arrow, true);
    }

    public void ColorNode(Color color)
    {
        nodeColor = color;
    }

    void EnableObject(GameObject go, bool state)
    {
        if (go != null)
        {
            go.SetActive(state);
        }
    }

    //public void ShowArrow(Color color)
    //{
    //    if (m_node != null && arrow != null && m_node.previous != null)
    //    {
    //        EnableObject(arrow, true);
    //        Vector3 dirToPRevious = (m_node.previous.position - m_node.position).normalized;
    //        arrow.transform.rotation = Quaternion.LookRotation(dirToPRevious);
    //        Renderer arrowRenderer = arrow.GetComponent<Renderer>();

    //        if (arrowRenderer != null)
    //        {
    //            arrowRenderer.material.color = color;
    //        }
    //    }
    //}

    void OnDrawGizmos()
    {
        if (nodeColor == emptyColor)
        {
            Gizmos.color = tileColors.GetNodeTypeColor(nodeType);
        }
        else
        {
            Gizmos.color = nodeColor;
        }

        Vector3 drawPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 drawScale = new Vector3(0.95f, 0.15f, 0.95f);

        Gizmos.DrawCube(drawPos, drawScale);
    }
}
