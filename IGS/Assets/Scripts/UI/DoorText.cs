using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorText : MonoBehaviour {

    // private bool Enabled = true;
    private TextMesh text;
    // Use this for initialization
    void Start()
    {
        text = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        int count = GameObject.FindGameObjectsWithTag("Enemy Citizen").Length;
        if (count == 0)
        {
            text.text = "Press E in front\nof doors to access\nnext level";
            TextMesh a = GameObject.Find("EatText").GetComponent<TextMesh>();
            a.text = "";
        }
    }
}
