using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeometrySize : MonoBehaviour, IMeshModifier
{
    public void ModifyMesh(Mesh mesh)
    {
        throw new System.NotImplementedException();
    }
    private List<UIVertex> vert = new List<UIVertex>();

    [SerializeField]
    private Vector2 m_size;

    public void ModifyMesh(VertexHelper verts)
    {
        vert.Clear();
        verts.GetUIVertexStream(vert);

        float left = 0, top = 0, right = 0, bottom = 0;
        foreach (var v in vert)
        {
            if (v.position.x < left)
            {
                left = v.position.x;
            }
            if (v.position.x > right)
            {
                right = v.position.x;
            }
            if (v.position.y > top)
            {
                top = v.position.y;
            }
            if (v.position.y < bottom)
            {
                bottom = v.position.y;
            }
        }
        this.m_size = new Vector2(right - left, top - bottom);
    }

}
