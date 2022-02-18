using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Typewriter")]
[RequireComponent(typeof(Text))]
public class UIEff_Typewriter : BaseMeshEffect
{
    //当前播放的字
    [SerializeField]
    private int currentIndex = 0;
    private int renderDoneIndex = 0;

    //每个字完成的透明度
    [SerializeField]
    private byte advanceInterval = 16;
    public byte AdvanceInterval { get => advanceInterval; }

    [SerializeField]
    private bool isPlaying = false;
    public bool IsPlaying { get => isPlaying; }

    //每个网格的透明度
    private byte[] opacity;

    //播放结束
    public event Action EndHandler;

    [SerializeField]
    private float speed = 0.5f;
    public float Speed
    {
        get => speed;
        set
        {
            speed = Math.Min(Math.Max(value, 0f), 1f);
        }
    }

    protected override void Awake()
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying) return;
#endif
        if (!IsActive()) return;
        base.Awake();
        Refresh();
    }

    public void Refresh()
    {
        currentIndex = 0;
        renderDoneIndex = 0;
        opacity = null;
    }

    private void FixedUpdate()
    {
        if (!IsActive()) return;
        if (!isPlaying) return;
        if (opacity == null || opacity.Length == 0)
        {
            graphic.SetVerticesDirty();
            return;
        }

        for (int w = 0; w < Mathf.Lerp(1, 10, speed); w++)
        {
            for (int i = renderDoneIndex; i < opacity.Length; i++)
            {
                //文字索引超过当前渲染文字
                if (i > currentIndex) break;
                //如果当前渲染字超过设定的完成度则前进
                if (opacity[currentIndex] >= this.advanceInterval)
                {
                    //不超过最大字数就前进一格字符
                    if (currentIndex < opacity.Length - 1)
                        ++currentIndex;
                }
                if (opacity[i] < 255)
                {
                    byte opacityStep = 2;
                    if ((int)opacity[i] + opacityStep > 255)
                    {
                        opacity[i] = 255;
                    }
                    else
                    {
                        opacity[i] += opacityStep;
                    }
                }
                if (opacity[i] == 255)
                {
                    ++renderDoneIndex;
                }
            }

            graphic.SetVerticesDirty();

            //最后一个透明度为1时结束
            if (opacity[opacity.Length - 1] == 255)
            {
                Stop();
            }
        }
    }

    public void Play()
    {
        Refresh();
        isPlaying = true;
    }
    public void Stop()
    {
        isPlaying = false;
        if (opacity != null)
        {
            //显示全部文字
            for (int i = 0; i < opacity.Length; i++)
            {
                opacity[i] = 255;
            }
            currentIndex = opacity.Length - 1;
        }

        graphic.SetVerticesDirty();

        EndHandler?.Invoke();
    }

    public override void ModifyMesh(VertexHelper vh)
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying) return;
#endif
        if (!IsActive()) return;

        List<UIVertex> verts = new List<UIVertex>();
        vh.GetUIVertexStream(verts);

        //6点转4点
        List<UIVertex> vs = new List<UIVertex>();

        for (int i = 1; i <= verts.Count; i += 3)
        {
            vs.Add(verts[i - 1]);
            vs.Add(verts[i]);
        }

        //初始化每个字的数组长度
        if (opacity == null) opacity = new byte[vs.Count / 4];

        //设置透明
        for (int i = 0; i < vs.Count; i += 4)
        {
            UIVertex v1 = vs[i + 0];
            UIVertex v2 = vs[i + 1];
            UIVertex v3 = vs[i + 2];
            UIVertex v4 = vs[i + 3];

            v1.color.a = opacity[i / 4];
            v2.color.a = opacity[i / 4];
            v3.color.a = opacity[i / 4];
            v4.color.a = opacity[i / 4];

            vh.SetUIVertex(v1, i + 0);
            vh.SetUIVertex(v2, i + 1);
            vh.SetUIVertex(v3, i + 2);
            vh.SetUIVertex(v4, i + 3);
        }

    }

}
