using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace JxUnity.GamePlay.UI
{
    public class PressAnyKey : MonoBehaviour
    {
        [SerializeField]
        private AudioClip soundEffect = null;

        [SerializeField]
        private float Speed = 2f;

        [SerializeField]
        private GameObject OnPressShowObject = null;
        [SerializeField]
        public UnityEvent OnPressHandler;

        private UIEff_Twinkle twinkle = null;
        private bool isPressed;

        private void Awake()
        {

            twinkle = GetComponent<UIEff_Twinkle>();
            if (twinkle == null)
            {
                twinkle = gameObject.AddComponent<UIEff_Twinkle>();
            }
        }

        private void OnEnable()
        {
            isPressed = false;
            twinkle.Speed = Speed;
            twinkle.IsInactiveAfterHide = false;
            twinkle.Play();
        }
        private void OnPress()
        {
            twinkle.Speed = Speed * 5;
            isPressed = true;
            if (soundEffect != null)
            {
                UISoundManager.Instance.Play(soundEffect);
            }
            StartCoroutine(Delay());
        }

        private void Update()
        {
            if (!isPressed && Input.anyKey)
            {
                OnPress();
            }
        }

        IEnumerator Delay()
        {
            yield return new WaitForSeconds(1f);

            twinkle.Stop();
            twinkle.IsInactiveAfterHide = true;
            twinkle.SetHide();

            if (OnPressShowObject != null)
            {
                OnPressShowObject.SetActive(true);
                OnPressHandler?.Invoke();
            }
        }

    }

}
