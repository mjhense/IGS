using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashAudio : MonoBehaviour {

    public AudioSource DashSource;
    public AudioClip DashClip;
    
    void Start()
    {
        DashSource.clip = DashClip;
    }
    
    void Update()
    {
        if (GameObject.Find("UI").GetComponent<UI>().isPaused || GameObject.Find("Hunger Bar").GetComponent<Image>().fillAmount <= 0)
            DashSource.Stop();
    }

    public void PlayDash()
    {
        if (!DashSource.isPlaying)
            DashSource.PlayOneShot(DashClip, 1);
    }
}
