using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {

    public float moveSpeedX;                    // The current horizontal movement speed of the player
    public float moveSpeedY;                    // The current vertical movement speed of the player
    private float maxMoveSpeed = 40.0f;         // The maximum speed the player can move
//    private float maxHiddenSpeed = 16.0f;       // The maximum speed the player can move while hidden
    public float currentMaxMoveSpeed;			// The current maximum speed the player can move
    private float acceleration = 128.0f;        // How fast the player can accelerate
	public bool isHidden = false;               // If the player is hidden in a shadow

	// Eating Variables
	private float hungerGainedEating = 0.1f;	// How much hunger is gained from eating an enemy
	public bool isEating = false;               // If the player is currently eating an enemy
	public bool canEat = true;					// If the player is able to eat an enemy
	private int eatCooldown = 2;				// How many seconds until the player can eat again
	private SpriteRenderer eatRend;
	private Image eatImage;

	// Dashing Variables
	private float hungerLostDashing = 0.1f;		// How much hunger is lost from dashing to a shadow    
    public bool canShadowDash = true;          	// If the player is able to dash to a shadow
	public bool inShadow = false;
	private int dashCooldown = 1;				// How many seconds until the player can dash again
	private SpriteRenderer dashRend;
	private Image dashImage;
    
	private Rigidbody2D player;
	private Collider2D[] enemiesInRange;
    private Animator animator;

    // Teleported Variables
    private GameObject teleported;
    private GameObject teleporter;


    // Start is called upon initialized
    void Start () {
		player = GetComponent<Rigidbody2D>();
		eatRend = GameObject.Find("Kill Circle").GetComponent<SpriteRenderer>();
		eatImage = GameObject.Find("Eat Icon").transform.Find("Cooldown Icon").GetComponent<Image>();
		dashRend = GameObject.Find("Dash Circle").GetComponent<SpriteRenderer>();
		dashImage = GameObject.Find("Dash Icon").transform.Find("Cooldown Icon").GetComponent<Image>();
//		Physics2D.IgnoreLayerCollision(this.gameObject.layer, GameObject.Find("Enemy Knight").layer);
        animator = GetComponent<Animator>();

        animator.SetLayerWeight(0, .99f);
        animator.speed = 0.5f;
    }
	
	// Update is called once per frame
	void Update () {
		if (!isHidden)
			dashRend.enabled = false;
		else
			dashRend.enabled = true;

        if (GameObject.Find("Hunger Bar").GetComponent<Image>().fillAmount <= 0)
            return;

		if (!isEating) {
			bool UP = Input.GetKey(KeyCode.W);
			bool DOWN = Input.GetKey(KeyCode.S);
			bool LEFT = Input.GetKey(KeyCode.A);
			bool RIGHT = Input.GetKey(KeyCode.D);
			bool LEFT_CLICK = Input.GetMouseButtonDown(0);
			bool SPACE = Input.GetKeyDown(KeyCode.Space);

            //handle animation directions
            if (UP) {
                animator.SetFloat("y", 0.10f);
            } else if (DOWN)  {
                animator.SetFloat("y", 0.6f);
            } else if (RIGHT) {
                animator.SetFloat("y", 0.4f);
            } else if (LEFT) {
                animator.SetFloat("y", 0.95f);
            }
            if (!UP && !DOWN && !RIGHT && !LEFT)
            {
                animator.SetLayerWeight(2, 1.0f);
                GameObject.Find("Player Walking Audio").GetComponent<PlayerWalkAudio>().PlayerSource.Stop();
            }
            else
            {
                animator.SetLayerWeight(2, 0.0f);
                GameObject.Find("Player Walking Audio").GetComponent<PlayerWalkAudio>().PlayPlayerWalk();
            }

			// Left and Right Movement
			if (LEFT) {
				moveSpeedX -= acceleration * Time.deltaTime;
			} if (RIGHT) {
				moveSpeedX += acceleration * Time.deltaTime;
			} if (!LEFT && !RIGHT) {
				moveSpeedX = 0;
			}

			// Up and Down Movement
			if (UP) {
				moveSpeedY += acceleration * Time.deltaTime;
			} if (DOWN) {
				moveSpeedY -= acceleration * Time.deltaTime;
			} if (!UP && !DOWN) {
				moveSpeedY = 0;
			}

            if (isHidden)
                currentMaxMoveSpeed += 0;
            //currentMaxMoveSpeed = maxHiddenSpeed;
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

			// Eat enemy raycast
			if ((LEFT_CLICK || SPACE) && canEat) {
                var direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
				RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 16, LayerMask.GetMask("Enemy", "Wall"));
				if (hit.collider != null) {
					if (hit.transform.tag.Contains("Enemy")) {
						//if (hit.transform.tag == "Enemy Knight" && hit.transform.gameObject.GetComponent<EnemyKnight>().state == "chasing") return;
						StartCoroutine(EatEnemy(hit.transform.gameObject, hit.point));
					}
				}
			}

            if (Input.GetKeyDown(KeyCode.LeftShift) && canShadowDash && inShadow) {
                var direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 24, LayerMask.GetMask( "Wall", "Shadows"));
                if (hit.collider != null) {
                    if (hit.transform.tag == "Shadow") {
						Debug.Log("Dashing!");
                        StartCoroutine(ShadowDash(hit.transform.gameObject));
                    }
                }
            }
		}
	}

	void OnTriggerStay2D(Collider2D trigger) {
		Debug.Log(trigger.tag);
		if (trigger.tag == "Shadow" || trigger.tag == "Tree") {
			if (isEating)  {
                isHidden = false;
            } else {
                isHidden = true;
            }

			inShadow = true;

            if (trigger.tag == "Shadow") {
                trigger.gameObject.layer = 0;
            }
		}

        if (trigger.tag == "TeleporterA") {
            if (Input.GetKeyDown(KeyCode.E))
            {
                teleported = GameObject.FindGameObjectWithTag("TeleportedA");
                StartCoroutine(TeleportLevel(teleported));
            }
        }

        if (trigger.tag == "TeleporterB")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                teleported = GameObject.FindGameObjectWithTag("TeleportedB");
                StartCoroutine(TeleportLevel(teleported));
            }
        }

        if (trigger.tag == "TeleporterC")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                teleported = GameObject.FindGameObjectWithTag("TeleportedC");
                StartCoroutine(TeleportLevel(teleported));
            }
        }

		if (trigger.tag == "Win") {
			if (Input.GetKeyDown(KeyCode.E)) {
				GameObject.Find("UI").GetComponent<UI>().winGame();
			}
		}
    }

	void OnTriggerExit2D(Collider2D trigger) {
		if (trigger.tag == "Shadow" || trigger.tag == "Tree") {
			isHidden = false;
			inShadow = false;
            if (trigger.tag == "Shadow")  {
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
		if (enemy.tag == "Enemy Knight") {
			enemy.GetComponent<EnemyKnight>().isBeingEaten = true;
		}
		else if (enemy.tag == "Enemy Citizen")
			enemy.GetComponent<EnemyCitizen>().isBeingEaten = true;
		transform.position = new Vector2(point.x, point.y);
		player.velocity = Vector2.zero;
		eatRend.enabled = false;
		canEat = false;
        canShadowDash = false;
        Destroy(enemy);
        //show the enemy eating
        animator.SetLayerWeight(0, 0f);
        animator.SetLayerWeight(2, 0f);
        animator.SetLayerWeight(1, 1f);

		yield return new WaitForSeconds(1);

        //return the enemy to walking
        animator.SetLayerWeight(1, 0f);
		GameObject.Find("Hunger Bar").GetComponent<Image>().fillAmount += hungerGainedEating;
		isEating = false;
		eatImage.fillAmount = 1.0f;
		StartCoroutine(ChangeCooldown(eatImage, (float) eatCooldown));

		yield return new WaitForSeconds(eatCooldown);

		eatRend.enabled = true;
		canEat = true;
		canShadowDash = true;
	}

	/// <summary>
	/// Smoothly lowers fill amount of image over specified duration.
	/// </summary>
	/// <returns>void</returns>
	/// <param name="image">Image to lower fill amount of.</param>
	/// <param name="duration">How long it should take to fully lower fill amount.</param>
	IEnumerator ChangeCooldown(Image image, float duration) {
		float timer = 0.0f;
		while (timer <= duration) {
			image.fillAmount = Mathf.SmoothStep(1.0f, 0.0f, timer / duration);
			timer += Time.deltaTime;
			yield return null;
		}
	}

	/// <summary>
	/// Moves the player to the shadow GameObject that has collided with the raycast.
	/// </summary>
	/// <returns>void</returns>
	/// <param name="shadow">The shadow that is being dashed to.</param>
    IEnumerator ShadowDash(GameObject shadow) {
        transform.position = shadow.transform.position;
        player.velocity = Vector2.zero;
        canShadowDash = false;
		dashRend.enabled = false;
		GameObject.Find("Hunger Bar").GetComponent<Image>().fillAmount -= hungerLostDashing;
		dashImage.fillAmount = 1.0f;
		StartCoroutine(ChangeCooldown(dashImage, (float) dashCooldown));

        //play dash sound
        GameObject.Find("Dash Audio").GetComponent<DashAudio>().PlayDash();

        yield return new WaitForSeconds(dashCooldown);

        canShadowDash = true;
		dashRend.enabled = true;
    }

    IEnumerator TeleportLevel(GameObject teleported)
    {
        GameObject go = GameObject.Find("FadeQuad");
        if (go == null)
            yield return null;
        FadeScrip scrip = (FadeScrip)go.GetComponent(typeof(FadeScrip));
        scrip.FadeOutCall();
        yield return new WaitForSeconds(2f);
        transform.position = teleported.transform.position;
        player.velocity = Vector2.zero;
        canShadowDash = true;
        dashRend.enabled = false;
        GameObject.Find("Hunger Bar").GetComponent<Image>().fillAmount = 1;

        yield return null;
    }
}