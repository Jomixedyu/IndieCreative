using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ef_Typewriter : MonoBehaviour
{
    public Text text;

    public string TargetText = string.Empty;

    public bool IsUseNormalText = true;
    [SerializeField]
    [Range(0, 0.2f)]
    private float interval = .1f;
    public float Interval { get => interval; set => interval = value; }

    public float Speed
    {
        get
        {
            return 1 - interval;
        }
        set
        {
            if (value > 1) value = 1;
            interval = 1 - value;
        }
    }

    [SerializeField]
    private float pageTurningSpeed = 1f;
    public float PageTurningSpeed { get => pageTurningSpeed; set => pageTurningSpeed = value; }

    private void Start()
    {
        if (IsUseNormalText)
            TargetText = text.text;
    }
    private void OnEnable()
    {

    }
    private float now = 0;
    public bool isResume = false;
    private void Update()
    {
        if (isResume)
        {
            if (interval == 0)
                ForceDone();
            now += Time.deltaTime;
            if (now > interval)
            {
                //打印一个字
                printChar();
                now = 0;
            }
        }
    }

    private int CharCurrent = 0;
    private void printChar()
    {
        if (text.text.Length >= TargetText.Length)
        {
            isResume = false;
            CharCurrent = 0;
            StartCoroutine(delay());
        }
        else
        {
            text.text += TargetText[CharCurrent];
            CharCurrent++;
        }
    }
    IEnumerator delay()
    {
        yield return new WaitForSeconds(pageTurningSpeed);
        text.text = string.Empty;
        isResume = true;
    }
    public void Play()
    {
        isResume = true;
    }
    public void Stop()
    {
        isResume = false;
    }
    public void ForceDone()
    {
        text.text = TargetText;
    }
}
