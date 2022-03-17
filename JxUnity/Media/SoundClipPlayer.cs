using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundClipPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;

    private AudioSource source;

    private static SoundClipPlayer instance;
    public static SoundClipPlayer Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }

    public void Play(string name)
    {
        foreach (var item in clips)
        {
            if (item.name == name)
            {
                source.clip = item;
                source.Play();
                break;
            }
        }
    }
}
