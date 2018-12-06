using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedingAudio : MonoBehaviour {

    public AudioSource MusicSource;
    public AudioClip FeedingClip;
    private bool lastFeeding = false;
    
    void Start()
    {
        MusicSource.clip = FeedingClip;
    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (lastFeeding != player.transform.GetComponent<PlayerMovement>().isEating)
            if (player.transform.GetComponent<PlayerMovement>().isEating)
                MusicSource.PlayOneShot(FeedingClip, 0.2f);
        lastFeeding = player.transform.GetComponent<PlayerMovement>().isEating;
        if (GameObject.Find("UI").GetComponent<UI>().isPaused || GameObject.Find("Hunger Bar").GetComponent<Image>().fillAmount <= 0)
        {
            MusicSource.Stop();
        }
    }
}