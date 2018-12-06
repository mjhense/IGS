using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyKnight : MonoBehaviour
{

    public string state; // States: idle, roamingLeft, roamingRight, roamingUp, roamingDown, chasing, patrolling

    [HideInInspector]
    public bool isBeingEaten = false;

    [HideInInspector]
    public bool wasChasing = false;

    private int stateNum;
    private float moveSpeed = 11.0f;
    private float chaseSpeed = 18.0f;
    private float repeatRate;
    private float minStateDur = 2.0f;
    private float maxStateDur = 5.0f;
    private float patrolRadius = 32.0f;
    private bool gettingNewPatrol = false;
    public int patrolNum = 0;

    private Rigidbody2D enemyRB;
    private GameObject enemyState;
    private GameObject player;
    private GameObject lastLocation;
    private GameObject viewRange;
    public GameObject patrolLocation;
    private Vector2 patrolPosition;
    private Animator animator;

    // Use this for initialization
    void Start()
    {
        enemyRB = GetComponent<Rigidbody2D>();
        enemyState = this.transform.GetChild(1).gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        patrolLocation = this.transform.GetChild(3).gameObject;
        lastLocation = this.transform.GetChild(4).gameObject;
        viewRange = this.transform.GetChild(2).gameObject;
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, this.gameObject.layer);

        state = "idle";
        repeatRate = Random.Range(5.0f, 10.0f);
        InvokeRepeating("changeState", 2.0f, repeatRate);
        animator = GetComponentInChildren<Animator>();
        animator.speed = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Hunger Bar").GetComponent<Image>().fillAmount <= 0)
            return;
        enemyState.GetComponent<TextMesh>().text = state;
        if (GameObject.Find("UI").GetComponent<UI>().isDebugging)
        {
            enemyState.GetComponent<MeshRenderer>().enabled = true;
            patrolLocation.GetComponent<SpriteRenderer>().enabled = true;
            lastLocation.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            enemyState.GetComponent<MeshRenderer>().enabled = false;
            patrolLocation.GetComponent<SpriteRenderer>().enabled = false;
            lastLocation.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (isBeingEaten)
            state = "idle";

        if (state == "idle")
        {
            enemyRB.velocity = Vector2.zero;
        }
        else if (state == "roamingLeft")
        {
            animator.SetFloat("y", 0.95f);
            enemyRB.velocity = new Vector2(-moveSpeed, 0);
            viewRange.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
        }
        else if (state == "roamingRight")
        {
            animator.SetFloat("y", 0.3f);
            enemyRB.velocity = new Vector2(moveSpeed, 0);
            viewRange.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
        else if (state == "roamingUp")
        {
            animator.SetFloat("y", 0.1f);
            enemyRB.velocity = new Vector2(0, moveSpeed);
            viewRange.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        else if (state == "roamingDown")
        {
            animator.SetFloat("y", 0.6f);
            enemyRB.velocity = new Vector2(0, -moveSpeed);
            viewRange.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (state == "chasing")
        {
            if (player.GetComponent<PlayerMovement>().isHidden)
            {
                state = "roamingDown";
                changeState();
            }
            else
            {
                enemyRB.velocity = Vector2.zero;
                viewRange.transform.up = transform.position - player.transform.position;
                transform.position -= viewRange.transform.up * chaseSpeed * Time.deltaTime;

                lastLocation.transform.position += viewRange.transform.up * chaseSpeed * Time.deltaTime;
                patrolLocation.transform.position += viewRange.transform.up * chaseSpeed * Time.deltaTime; // Make last and patrol location objects retain their position

                Vector2 dir = player.transform.position - this.transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                if (angle > -45 && angle < 45)
                    animator.SetFloat("y", 0.3f);           // Rotate enemy sprite right
                else if (angle >= 45 && angle <= 135)
                    animator.SetFloat("y", 0.1f);           // Rotate enemy sprite up
                else if (angle > 135 || angle < -135)
                    animator.SetFloat("y", 0.95f);          // Rotate enemy sprite left
                else if (angle <= -45 && angle >= -135)
                    animator.SetFloat("y", 0.6f);           // Rotate enemy sprite down
            }
        }
        else if (state == "patrolling")
            changeState();
        /*{
            if (Vector2.Distance(transform.position, patrolPosition) < 2.0f)
            {
                if (!gettingNewPatrol) patrolNum += 1;
                if (patrolNum > 5)
                {
                    state = "idle";
                    patrolNum = 0;
                }
                if (!gettingNewPatrol) Invoke("getPatrolPosition", 1);
                gettingNewPatrol = true;
            }
            else
            {
                viewRange.transform.up = (Vector2)transform.position - patrolPosition;
                transform.position -= viewRange.transform.up * chaseSpeed * Time.deltaTime;

                lastLocation.transform.position += viewRange.transform.up * chaseSpeed * Time.deltaTime;
                patrolLocation.transform.position += viewRange.transform.up * chaseSpeed * Time.deltaTime;
            }
        }*/
        Vector3 screenPoint = GameObject.Find("Player Camera").GetComponent<Camera>().WorldToViewportPoint(transform.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        if (state != "patrolling" && !isBeingEaten && onScreen && GameObject.Find("Hunger Bar").GetComponent<Image>().fillAmount > 0)
            GameObject.Find("Walking Audio").GetComponent<WalkAudio>().PlayKnightWalk();
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.tag == "Wall")
        {
            switchDirection();
        }
    }

    void OnTriggerStay2D(Collider2D trigger)
    {
        if (trigger.tag == "Player" && state == "chasing")
        {
            GameObject.Find("Hunger Bar").GetComponent<Image>().fillAmount -= 0.01f;
        }
    }

    void changeState()
    {
        if (state == "chasing" || state == "patrolling")
            return;
        stateNum = Random.Range(1, 5);
        if (stateNum == 0)
            state = "idle";
        else if (stateNum == 1)
            state = "roamingLeft";
        else if (stateNum == 2)
            state = "roamingRight";
        else if (stateNum == 3)
            state = "roamingUp";
        else if (stateNum == 4)
            state = "roamingDown";
        repeatRate = Random.Range(minStateDur, maxStateDur);
    }

    void switchDirection()
    {
        stateNum = Random.Range(1, 4);
        if (state == "roamingUp")
        {
            enemyRB.transform.Translate(0, -1, 0);
            if (stateNum == 1)
                state = "roamingDown";
            else if (stateNum == 2)
                state = "roamingLeft";
            else if (stateNum == 3)
                state = "roamingRight";
        }
        else if (state == "roamingDown")
        {
            enemyRB.transform.Translate(0, 1, 0);
            if (stateNum == 1)
                state = "roamingUp";
            else if (stateNum == 2)
                state = "roamingLeft";
            else if (stateNum == 3)
                state = "roamingRight";
        }
        else if (state == "roamingLeft")
        {
            enemyRB.transform.Translate(1, 0, 0);
            if (stateNum == 1)
                state = "roamingUp";
            else if (stateNum == 2)
                state = "roamingDown";
            else if (stateNum == 3)
                state = "roamingRight";
        }
        else if (state == "roamingRight")
        {
            enemyRB.transform.Translate(-1, 0, 0);
            if (stateNum == 1)
                state = "roamingUp";
            else if (stateNum == 2)
                state = "roamingDown";
            else if (stateNum == 3)
                state = "roamingLeft";
        }
    }

    public void startPatrol()
    {
        state = "idle";//"patrolling";
        changeState();
        patrolNum = 0;
        getPatrolPosition();
    }

    void getPatrolPosition()
    {
        gettingNewPatrol = false;
        Vector2 circlePosition;
        Vector2 actualPatrolPosition = new Vector2(0, 0);
        bool positionFree = false;
        int loop_count = 0;

        while (!positionFree)
        {

            loop_count += 1;
            if (loop_count >= 100)
            {
                //Debug.LogError("LOOP TIMEOUT");
                return;
            }

            circlePosition = Random.insideUnitCircle * patrolRadius;
            actualPatrolPosition = (Vector2)lastLocation.transform.position + circlePosition;
            patrolLocation.transform.position = actualPatrolPosition;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, actualPatrolPosition, Vector2.Distance(transform.position, actualPatrolPosition), LayerMask.GetMask("Wall"));
            if (!hit.collider) positionFree = true;
        }

        patrolPosition = actualPatrolPosition;
    }
}