using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioGlitcherBehavior : MonoBehaviour
{
    public Glitcher Glitcher = null;

    private bool isPlaying = false;
    // Use this for initialization
    void Start()
    {
        GetComponent<AudioSource>().loop = true;
        GetComponent<AudioSource>().spatialize = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Glitcher != null && Glitcher.EnableGlitch)
        {
            this.transform.position = Glitcher.HitPoint;

            if (isPlaying != Glitcher.EnableGlitch)
            {
                GetComponent<AudioSource>().Play();
                isPlaying = true;
            }
        }
        else
        {
            if (enabled)
            {
                GetComponent<AudioSource>().Stop();
                isPlaying = false;
            }
        }
    }
}
