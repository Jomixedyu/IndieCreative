using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIEff_AlphaTransition : MonoBehaviour
{
    protected CanvasGroup canvasGroup;
    public event Action OnShowDone;
    public event Action OnHideDone;

    [SerializeField]
    protected bool playOnAwake = false;

    [SerializeField]
    protected float speed = 1f;
    public float Speed { get => speed; set => speed = value; }

    protected bool isToShow = false;
    protected bool isToHide = false;

    private bool isShowing = false;
    public bool IsShowing { get => isShowing; }

    public bool IsToShow { get => isToShow; }
    public bool IsToHide { get => isToHide; }

    public float Alpha { get => canvasGroup.alpha; }

    [SerializeField]
    private bool isInactiveAfterHide;
    public bool IsInactiveAfterHide
    {
        get => isInactiveAfterHide;
        set => isInactiveAfterHide = value;
    }

    private bool isInit = false;
    private void Init()
    {
        if (isInit)
        {
            return;
        }
        isInit = true;
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        if (playOnAwake)
            isToShow = true;
    }

    protected virtual void Awake()
    {
        Init();
    }

    public void Show()
    {
        Init();
        isToShow = true;
        isToHide = false;
    }

    public void HideToShow()
    {
        Init();
        isToShow = true;
        isToHide = false;
        canvasGroup.alpha = 0;
    }
    public void Hide()
    {
        Init();
        isToShow = false;
        isToHide = true;
        isShowing = false;
    }

    public void SetShow()
    {
        Init();
        this.Internal_OnShow();
        OnShowDone?.Invoke();
    }

    public void SetHide()
    {
        Init();
        this.Internal_OnHide();
        OnHideDone?.Invoke();
    }

    public void InitShow()
    {
        Init();
        this.Internal_OnShow();
    }
    public void InitHide()
    {
        Init();
        this.Internal_OnHide();
    }

    private void Internal_OnShow()
    {
        canvasGroup.alpha = 1;
        isToShow = false;
        isToHide = false;
        isShowing = true;
        OnShow();
    }
    private void Internal_OnHide()
    {
        canvasGroup.alpha = 0;
        isToShow = false;
        isToHide = false;
        isShowing = false;
        OnHide();
        if (this.IsInactiveAfterHide)
        {
            this.gameObject.SetActive(true);
        }
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
    private void Update()
    {
        if (isToShow)
        {
            if (canvasGroup.alpha >= 1)
            {
                this.SetShow();
            }
            else
            {
                canvasGroup.alpha += Time.deltaTime * speed;
            }
        }
        else if (isToHide)
        {
            if (canvasGroup.alpha <= 0)
            {
                this.SetHide();
            }
            else
            {
                canvasGroup.alpha -= Time.deltaTime * speed;
            }
        }
    }
}
