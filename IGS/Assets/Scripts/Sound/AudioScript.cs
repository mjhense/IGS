using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioScript : MonoBehaviour {

    public AudioSource MusicSource;
    public AudioClip MusicClip;

    // Use this for initialization
    void Start()
    {
        MusicSource.clip = MusicClip;
        MusicSource.loop = true;
        PlayTheme();
    }

    public void PlayTheme()
    {
        MusicSource.PlayOneShot(MusicClip, 0.75f);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Hunger Bar").GetComponent<Image>().fillAmount <= 0 || GameObject.Find("UI").GetComponent<UI>().isPaused)
        {
            MusicSource.Stop();
        }
    }
    
}