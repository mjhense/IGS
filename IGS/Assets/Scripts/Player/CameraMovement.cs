using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	public GameObject player;
	private Vector2 velocity;
	public float smoothTimeX = 0.15f;
	public float smoothTimeY = 0.15f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate() {
		float posX = Mathf.SmoothDamp(transform.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);
		float posY = Mathf.SmoothDamp(transform.position.y, player.transform.position.y, ref velocity.y, smoothTimeY);

		transform.position = new Vector3(posX, posY, transform.position.z);

		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			if (this.transform.position.z < -32)
			this.transform.Translate(0, 0, 16);
		} else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			if (this.transform.position.z > -256)
			this.transform.Translate(0, 0, -16);
		}
	}
}
