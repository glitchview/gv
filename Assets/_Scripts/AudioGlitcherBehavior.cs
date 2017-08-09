using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioGlitcherBehavior : MonoBehaviour
{
    public Glitcher Glitcher = null;

    private bool isPlaying = false;

    private AudioSource audioSource;
    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.spatialize = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Glitcher != null && Glitcher.EnableGlitch)
        {
            this.transform.position = Glitcher.HitPoint;

            if (isPlaying != Glitcher.EnableGlitch)
            {
                audioSource.Play();
                isPlaying = true;
            }
        }
        else
        {
            if (enabled)
            {
                audioSource.Stop();
                isPlaying = false;
            }
        }
    }
}
