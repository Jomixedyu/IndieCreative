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

        private void OnEnable()
        {
            UISoundManager.Instance.PlayByGroup(this.soundGroupName, UISoundConfigGroupType.Enable);
        }

        private void OnDisable()
        {
            UISoundManager.Instance.PlayByGroup(this.soundGroupName, UISoundConfigGroupType.Disable);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            UISoundManager.Instance.PlayByGroup(this.soundGroupName, UISoundConfigGroupType.Enter);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UISoundManager.Instance.PlayByGroup(this.soundGroupName, UISoundConfigGroupType.Down);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UISoundManager.Instance.PlayByGroup(this.soundGroupName, UISoundConfigGroupType.Exit);
        }
    }
}
