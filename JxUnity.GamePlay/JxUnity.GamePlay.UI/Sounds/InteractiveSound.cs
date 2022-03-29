using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JxUnity.GamePlay.UI
{

    public class InteractiveSound : MonoBehaviour
        , IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
    {
        [SerializeField]
        public string soundGroupName;

        private Selectable selectable;

        private bool IsInteracting()
        {
            if (selectable != null)
            {
                return selectable.interactable;
            }
            return true;
        }

        private void Awake()
        {
            selectable = this.GetComponent<Selectable>();
        }

        private void OnEnable()
        {
            if (IsInteracting())
            {
                UISoundManager.Instance.PlayByGroup(this.soundGroupName, UISoundConfigGroupType.Enable);
            }
        }

        private void OnDisable()
        {
            if (IsInteracting() && UISoundManager.HasInstance)
            {
                UISoundManager.Instance.PlayByGroup(this.soundGroupName, UISoundConfigGroupType.Disable);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsInteracting())
                UISoundManager.Instance.PlayByGroup(this.soundGroupName, UISoundConfigGroupType.Enter);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsInteracting())
                UISoundManager.Instance.PlayByGroup(this.soundGroupName, UISoundConfigGroupType.Down);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsInteracting())
                UISoundManager.Instance.PlayByGroup(this.soundGroupName, UISoundConfigGroupType.Exit);
        }
    }
}
