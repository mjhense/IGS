using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWalkAudio : MonoBehaviour {

    public AudioSource PlayerSource;
    public AudioClip PlayerWalkingClip;

    // Use this for initialization
    void Start () {
        PlayerSource.clip = PlayerWalkingClip;
    }
	
	// Update is called once per frame
	void Update () {
        if (GameObject.Find("UI").GetComponent<UI>().isPaused || GameObject.Find("Hunger Bar").GetComponent<Image>().fillAmount <= 0)
        {
            PlayerSource.Stop();
        }
    }

    public void PlayPlayerWalk()
    {
        if (!PlayerSource.isPlaying)
            PlayerSource.PlayOneShot(PlayerWalkingClip, 1);
    }
}