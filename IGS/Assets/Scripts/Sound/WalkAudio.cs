using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalkAudio : MonoBehaviour {

    public AudioSource KnightSource;
    public AudioClip KnightWalkingClip;

    // Use this for initialization
    void Start()
    {
        KnightSource.clip = KnightWalkingClip;
    }

    void Update()
    {
        if (GameObject.Find("UI").GetComponent<UI>().isPaused || GameObject.Find("Hunger Bar").GetComponent<Image>().fillAmount <= 0)
        {
            KnightSource.Stop();
        }
    }

    public void PlayKnightWalk()
    {
        if(!KnightSource.isPlaying)
            KnightSource.PlayOneShot(KnightWalkingClip, 0.4f);
    }
}