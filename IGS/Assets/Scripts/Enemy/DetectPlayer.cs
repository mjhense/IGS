using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour {

	private GameObject player;
	private GameObject enemy;
	private GameObject lastLocation;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		enemy = transform.parent.gameObject;
		lastLocation = this.transform.parent.transform.GetChild(4).gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay2D(Collider2D trigger) {
		if (trigger.tag == "Player" && !player.GetComponent<PlayerMovement>().isHidden) {
			RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, player.transform.position - enemy.transform.position, 256, LayerMask.GetMask("Player", "Wall"));
			if (hit.collider && hit.collider.tag == "Player") {
				lastLocation.transform.position = hit.point;
                if (enemy.tag == "Enemy Knight") {
                    enemy.GetComponent<EnemyKnight>().state = "chasing";
					enemy.GetComponent<EnemyKnight>().wasChasing = true;
                }
			} else if (enemy.GetComponent<EnemyKnight>().wasChasing) {
				enemy.GetComponent<EnemyKnight>().startPatrol();
			}
		}
	}

	void OnTriggerExit2D(Collider2D trigger) {
		if (trigger.tag == "Player" && enemy.GetComponent<EnemyKnight>().state == "chasing") {
			enemy.GetComponent<EnemyKnight>().wasChasing = false;
			enemy.GetComponent<EnemyKnight>().startPatrol();
		}
	}
}
