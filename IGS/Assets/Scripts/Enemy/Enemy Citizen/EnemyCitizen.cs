using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCitizen : MonoBehaviour {

	[HideInInspector]
	public bool isBeingEaten = false;

	private Rigidbody2D enemyRB;
	private GameObject player;
	private Animator animator;

	// Use this for initialization
	void Start () {
		enemyRB = GetComponent<Rigidbody2D>();
		player = GameObject.FindGameObjectWithTag("Player");
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
