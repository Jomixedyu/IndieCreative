using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandle : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private string dragTag;
    public string DragTag => dragTag;

    private CanvasGroup cg;

    private Vector3 startPosition;
    public Vector3 StartPosition => startPosition;

    private static GameObject dragGameObject;
    public static GameObject DragGameObject => dragGameObject;

    private static GameObject currentSlot;

    public void ResetPosition()
    {
        transform.position = startPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.startPosition = transform.position;
        currentSlot = null;
        dragGameObject = this.thisgo;
        this.cg = GetComponent<CanvasGroup>();
        if (this.cg == null)
        {
            this.cg = this.thisgo.AddComponent<CanvasGroup>();
        }
        this.cg.blocksRaycasts = false;
    }

    private GameObject thisgo;
    private void OnEnable()
    {
        this.thisgo = this.gameObject;
    }

    private void DiscardCurrentSlot()
    {
        if (currentSlot != null)
        {
            currentSlot.GetComponent<IDragSlot>().OnDragEnd(null);
        }
        currentSlot = null;
    }
    private void ChangeSlot(GameObject go)
    {
        DiscardCurrentSlot();
        currentSlot = go;
    }

    public void OnDrag(PointerEventData e)
    {
        var raygo = e.pointerCurrentRaycast.gameObject;
        var slot = raygo?.GetComponent<IDragSlot>();
        if (slot != null)
        {
            //射到东西并且不是自己
            if (currentSlot != raygo)
            {
                ChangeSlot(raygo);
                slot.OnDragBegin(this);
            }
        }
        else
        {
            DiscardCurrentSlot();
        }

        transform.position = e.position;
    }

    public void OnEndDrag(PointerEventData e)
    {
        var raygo = currentSlot;
        if (raygo != null)
        {
            var slot = raygo.GetComponent<IDragSlot>();
            if (slot != null)
            {
                slot.OnDragEnd(this);
            }
        }

        this.cg.blocksRaycasts = true;
        dragGameObject = null;
        currentSlot = null;
    }


}
