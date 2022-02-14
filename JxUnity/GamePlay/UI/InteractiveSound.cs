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
            UISoundManager.Instance.Play(this.soundGroupName, UISoundConfigGroupType.Enable);
        }

        private void OnDisable()
        {
            UISoundManager.Instance.Play(this.soundGroupName, UISoundConfigGroupType.Disable);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            UISoundManager.Instance.Play(this.soundGroupName, UISoundConfigGroupType.Enter);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UISoundManager.Instance.Play(this.soundGroupName, UISoundConfigGroupType.Down);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UISoundManager.Instance.Play(this.soundGroupName, UISoundConfigGroupType.Exit);
        }
    }
}
