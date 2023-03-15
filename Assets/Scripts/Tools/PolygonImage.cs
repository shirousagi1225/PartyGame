using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

[AddComponentMenu("LFramework/UI/PolygonImage",12)]
[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonImage : Image
{
    protected PolygonImage()
    {

    }

    PolygonCollider2D m_polygonCollider2D;

    protected override void Awake()
    {
        m_polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
    }

    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, eventCamera, out worldPos);
        return m_polygonCollider2D.OverlapPoint(worldPos);
    }
}
