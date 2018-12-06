using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTree : MonoBehaviour {

	private SpriteRenderer treeRend;

	private Color treeColor;

	// Use this for initialization
	void Start() {
		treeRend = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update() {
		
	}

	void OnTriggerStay2D(Collider2D trigger) {
		if (trigger.tag != "Player") return;
		treeColor = treeRend.color;
		treeColor.a = 0.5f;
		treeRend.color = treeColor;
	}

	void OnTriggerExit2D(Collider2D trigger) {
		if (trigger.tag != "Player") return;
		treeColor = treeRend.color;
		treeColor.a = 1.0f;
		treeRend.color = treeColor;
	}
}
