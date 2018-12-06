using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextMoveScript : MonoBehaviour {

   private bool Enabled = true;
    private TextMesh text;
    // Use this for initialization
    void Start()
    {
        text = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        bool WASD = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.W);
        if (WASD && Enabled == true)
        {
            Enabled = false;
            text.text = "";
            TextMesh a = GameObject.Find("EatText").GetComponent<TextMesh>();
            a.text = "Press space in front of\nenemies to eat them";
        }
    }
}