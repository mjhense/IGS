using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {

    public float moveSpeedX;                    // The current horizontal movement speed of the player
    public float moveSpeedY;                    // The current vertical movement speed of the player
    public float maxMoveSpeed = 64.0f;          // The maximum speed the player can move
    public float maxHiddenSpeed = 16.0f;        // The maximum speed the player can move while hidden
    public float currentMaxMoveSpeed;
    public float acceleration = 128.0f;         // How fast the player can accelerate
    public bool isHidden = false;               // If the player is hidden in a shadow
    public bool isEating = false;               // If the player is currently eating an enemy
    public bool canEat = true;					// If the player is able to eat an enemy
    public bool canShadowDash = false;           // If the player is able to use the Eat Dash Ability
    public bool isDashing = false;

    private SpriteRenderer killRend;
	private Rigidbody2D player;

	private Collider2D[] enemiesInRange;

	// Start is called upon initialized
	void Start () {
		player = GetComponent<Rigidbody2D>();
		killRend = GameObject.Find("Kill Circle").GetComponent<SpriteRenderer>();
		Physics2D.IgnoreLayerCollision(this.gameObject.layer, GameObject.Find("Enemy").layer);
	}
	
	// Update is called once per frame
	void Update () {

		if (!isEating && !isDashing) {
			bool UP = Input.GetKey(KeyCode.W);
			bool DOWN = Input.GetKey(KeyCode.S);
			bool LEFT = Input.GetKey(KeyCode.A);
			bool RIGHT = Input.GetKey(KeyCode.D);
			bool LEFT_CLICK = Input.GetMouseButtonDown(0);
			bool SPACE = Input.GetKeyDown(KeyCode.Space);

			// Left and Right Movement
			if (LEFT) {
				moveSpeedX -= acceleration * Time.deltaTime;
			}
			if (RIGHT) {
				moveSpeedX += acceleration * Time.deltaTime;
			}
			if (!LEFT && !RIGHT) {
				moveSpeedX = 0;
			}

			// Up and Down Movement
			if (UP) {
				moveSpeedY += acceleration * Time.deltaTime;
			}
			if (DOWN) {
				moveSpeedY -= acceleration * Time.deltaTime;
			}
			if (!UP && !DOWN) {
				moveSpeedY = 0;
			}

			if (isHidden)
				currentMaxMoveSpeed = maxHiddenSpeed;
			else
				currentMaxMoveSpeed = maxMoveSpeed;
			moveSpeedX = Mathf.Clamp(moveSpeedX, -currentMaxMoveSpeed, currentMaxMoveSpeed);
			moveSpeedY = Mathf.Clamp(moveSpeedY, -currentMaxMoveSpeed, currentMaxMoveSpeed);

			player.velocity = new Vector2(moveSpeedX, moveSpeedY);

			// Rotate player towards mouse
			var mouse = Input.mousePosition;
			var screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
			var offset = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y);
			var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0, 0, angle - 90);

			// Eat enemy raycast
			if ((LEFT_CLICK || SPACE) && canEat) {
				var direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
				RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 16, LayerMask.GetMask("Enemy", "Wall"));
				if (hit.collider != null) {
					if (hit.transform.tag == "Enemy") {
						StartCoroutine(EatEnemy(hit.transform.gameObject, hit.point));
					}
				}
			}

            if (Input.GetKeyDown(KeyCode.LeftShift) && canShadowDash)
            {
                var direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 24, LayerMask.GetMask( "Wall", "Shadows"));
                if (hit.collider != null)
                {
                    if (hit.transform.tag == "Shadow")
                    {
                        StartCoroutine(ShadowDash(hit.transform.gameObject, hit.point));
                    }
                }

            }
		}
	}

	void OnTriggerStay2D(Collider2D trigger) {
		if (trigger.tag == "Shadow" || trigger.tag == "Tree") {
			if (isEating)
            {
                isHidden = false;
                canShadowDash = false;
            }
			else{
                isHidden = true;
                canShadowDash = true;
            }
			
            if (trigger.tag == "Shadow")
            {
                trigger.gameObject.layer = 0;
            }
		}
	}

	void OnTriggerExit2D(Collider2D trigger) {
		if (trigger.tag == "Shadow" || trigger.tag == "Tree") {
			isHidden = false;
            canShadowDash = true;
            if (trigger.tag == "Shadow")
            {
                trigger.gameObject.layer = 10;
            }
		}
	}

	/// <summary>
	/// Destroys the enemy GameObject that has collided with the raycast.
	/// </summary>
	/// <returns>void</returns>
	/// <param name="enemy">Enemy that is to be eaten.</param>
	/// <param name="point">Point in which the raycast collides with the enemy.</param>
	IEnumerator EatEnemy(GameObject enemy, Vector2 point) {
		isEating = true;
		enemy.GetComponent<EnemyMovement>().isBeingEaten = true;
		transform.position = new Vector2(point.x, point.y);
		player.velocity = Vector2.zero;
		killRend.enabled = false;
		canEat = false;
        canShadowDash = false;

		yield return new WaitForSeconds(1);

		Destroy(enemy);
		//GameObject.Find("Hunger Bar").GetComponent<Image>().fillAmount += 0.1f;
		isEating = false;

		yield return new WaitForSeconds(2);

		killRend.enabled = true;
		canEat = true;
	}

    /* <summary>
     * Allows player to dash from one shadow to the next using health as a resource
     * 
     */

    IEnumerator ShadowDash(GameObject shadow, Vector2 point)
    {
        isDashing = true;
        transform.position = shadow.transform.position;
        player.velocity = Vector2.zero;
        canEat = false;
        canShadowDash = false;

       // GameObject.Find("Hunger Bar").GetComponent<Image>().fillAmount -= 0.25f;

        yield return new WaitForSeconds(1);
        isDashing = false;
        canEat = true;
        canShadowDash = true;
    }
}
