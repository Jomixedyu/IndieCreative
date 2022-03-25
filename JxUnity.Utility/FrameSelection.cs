
using UnityEngine;

public class FrameSelection : MonoBehaviour
{
    public static void AddToMainCamera()
    {
        if (Camera.main != null)
        {
            Camera.main.gameObject.AddComponent<FrameSelection>();
        }
    }

    protected void OnValidate()
    {
        if (GetComponent<Camera>() == null)
        {
            Debug.LogError("camera not found.");
        }
    }
    protected void Awake()
    {
        if (GetComponent<Camera>() == null)
        {
            Debug.LogError("camera not found.");
        }
    }

    [SerializeField]
    protected Color color = Color.white;
    public Color Color => color;

    protected Vector2Int start = Vector2Int.zero;

    protected Material drawMaterial = null;

    protected bool modal = false;

    protected void Start()
    {
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        drawMaterial = new Material(shader);
        drawMaterial.hideFlags = HideFlags.HideAndDontSave;

        drawMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        drawMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        drawMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

        drawMaterial.SetInt("_ZWrite", 0);

        drawMaterial.hideFlags = HideFlags.HideAndDontSave;

        drawMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
    }

    protected Vector2Int CursorPos()
    {
        var pos = Input.mousePosition;
        return new Vector2Int((int)pos.x, (int)pos.y);
    }

    protected void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.modal = true;
            this.start = CursorPos();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            this.modal = false;
        }
    }


    protected void OnPostRender()
    {
        if (modal)
        {
            if (!drawMaterial)
                return;

            Vector2Int end = CursorPos();
            GL.PushMatrix();
            drawMaterial.SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.QUADS);
            GL.Color(new Color(color.r, color.g, color.b, 0.1f));
            GL.Vertex3(start.x, start.y, 0);
            GL.Vertex3(end.x, start.y, 0);
            GL.Vertex3(end.x, end.y, 0);
            GL.Vertex3(start.x, end.y, 0);
            GL.End();

            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex3(start.x, start.y, 0);
            GL.Vertex3(end.x, start.y, 0);
            GL.Vertex3(end.x, start.y, 0);
            GL.Vertex3(end.x, end.y, 0);
            GL.Vertex3(end.x, end.y, 0);
            GL.Vertex3(start.x, end.y, 0);
            GL.Vertex3(start.x, end.y, 0);
            GL.Vertex3(start.x, start.y, 0);
            GL.End();

            GL.PopMatrix();

        }

    }
}

