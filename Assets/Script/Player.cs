using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public enum lanes {
		lane1, lane2, lane3
	}
	public lanes currentLane;
	public float jumpTimeCount;
	public float jumpTimeLimit;
	public float jumpForce;
	public bool isJumping;
	private GameObject[] laneObjects;
	private Rigidbody2D rigid;

    private bool isSwitchPlane;
    private float time;
    public float timeToReachTarget;
    private Vector2 startPosition;
    private Vector2 target;
    private Vector2 startPos;
    private Vector2 direction;
    public bool directionChosen;
    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private Vector3 mouseEnd;
    private bool isPanning;
    private SpriteRenderer spriteRenderer;

    public Animator animator;
    private bool isMomExist;
    public GameObject momPrefab;
    public GameObject endingWord;
    public GameObject replayButton;
    private GameObject mom;
    private Vector3 mousePos;

    public GameObject soundEffect;
	public Animator cameraAnimator;

    void Start () {
		rigid = this.GetComponent<Rigidbody2D>();
        isMomExist = false;
        //timeToReachTarget = 0.5f;
        //isOnPlane = false;
        laneObjects = GameController.instance.lanes;
		SwitchLanes(currentLane); // init and trigger lanes
		triggerColliders(currentLane);
	}

	public void SwitchLanes(lanes l) {
		Vector3 lanePos = laneObjects[(int)l].transform.position;
		lanePos.z = -1; // fixed z-axis: always front
                        //lanePos.y += this.GetComponent<SpriteRenderer>().bounds.size.y / 2;
                        //this.transform.position = lanePos;
        if ((int)l == 0)
        {
            this.GetComponent<SpriteRenderer>().sortingOrder = -13;
        }
        else if ((int)l == 1)
        {
            this.GetComponent<SpriteRenderer>().sortingOrder = -9;
        }
        else if ((int)l == 2)
        {
            this.GetComponent<SpriteRenderer>().sortingOrder = -5;
        }
        
        isSwitchPlane = true;
        time = 0;
        startPosition = this.transform.position;
        target = lanePos;
		// turn off others lane's collider
		triggerColliders(l);
	}

	void triggerColliders(lanes l) {
		for (int i = 0; i < laneObjects.Length; i++) {
			if ((int)l == i) {
				laneObjects[i].GetComponent<BoxCollider2D>().enabled = true;
			} else {
				laneObjects[i].GetComponent<BoxCollider2D>().enabled = false;
			}
		} 
	}

	void CheckSwitchLane() {
		if (!isJumping) { // cannot switch lane while jumping
			if (Input.GetKeyDown(KeyCode.UpArrow)) {
            // change lanes UP
			this.currentLane -= 1;
			if (this.currentLane < lanes.lane1) {
				this.currentLane = lanes.lane1;
			}
			SwitchLanes(currentLane);

			} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                // change lanes DOWN
                this.currentLane += 1;
				if (this.currentLane > lanes.lane3) {
					this.currentLane = lanes.lane3;
				}
				SwitchLanes(currentLane);
			}
		}
	}

    void CheckJumpTab() {
        // Hold button to jump higher
        if (!isSwitchPlane)
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    mousePos = Input.mousePosition;
            //    jumpTimeCount += Time.deltaTime;
            //    Debug.Log("Jump");
            //}
            //if (Input.GetMouseButton(0))
            //{
            //    jumpTimeCount += Time.deltaTime;
            //}
            if (Input.GetMouseButtonUp(0) &&!isJumping)
            {
                isJumping = true;

                Debug.Log("height jump");
                //jumpTimeCount = Mathf.Clamp(jumpTimeCount, 0, 0.5f);
                //rigid.AddForce(Vector2.up * jumpForce * (1 + jumpTimeCount));
                rigid.AddForce(Vector2.up * jumpForce);

                animator.SetInteger("status", 1);
                jumpTimeCount = 0;
                //animator.SetInteger("status", 1);
            }
        }
    }

	void CheckJump() {
        // Hold button to jump higher
        if (!isSwitchPlane) {
            if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                jumpTimeCount += Time.deltaTime;
                Debug.Log("Jump");
                rigid.AddForce(Vector2.up * jumpForce);
                isJumping = true;
                animator.SetInteger("status", 1);
            }
            if (Input.GetKey(KeyCode.Space) && isJumping)
            {
                jumpTimeCount += Time.deltaTime;
                if (jumpTimeCount < jumpTimeLimit)
                {
                    Debug.Log("height jump");
                    rigid.AddForce(Vector2.up * jumpForce / 20);
                }
                animator.SetInteger("status", 1);
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                jumpTimeCount = 0;
                //animator.SetInteger("status", 1);
            }
        }
	}

	void OnCollisionEnter2D (Collision2D col) {
		// will be used to detect obstacle
		// steps: 
		// 1. collide with obs
		// 2. change anim state + has effect with camera + slow the speed
		Debug.Log(col.gameObject.tag);
		if (col.gameObject.tag == "Lane") {
			isJumping = false;
			jumpTimeCount = 0;
            animator.SetInteger("status", 0);
        }
    }

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "Obstacle") {
            Debug.Log("Player: Collide with Obstacle");
			if (col.gameObject.GetComponent<ObstacleInfo>().lane == (int)currentLane) {
                soundEffect.GetComponent<SoundInfo>().replay.Play();
                col.gameObject.GetComponent<BoxCollider2D>().enabled = false;
				cameraAnimator.SetBool("isColliding", true);
                GameController.instance.CollideWithObstacle();
			}
		}
	}

    public void PrintDirection(string direction) {
        Debug.LogError(direction);
        if (!GameController.instance.isGameOver)
        {
            if (!isJumping)
            {
                if (direction.Contains("Up"))
                {
                    this.currentLane -= 1;
                    if (this.currentLane < lanes.lane1)
                    {
                        this.currentLane = lanes.lane1;
                    }
                    SwitchLanes(currentLane);
                }
                else if (direction.Contains("Down"))
                {
                    this.currentLane += 1;
                    if (this.currentLane > lanes.lane3)
                    {
                        this.currentLane = lanes.lane3;
                    }
                    SwitchLanes(currentLane);
                }
            }
        } 
    }

    public void PrintTab(string type) {
        Debug.LogError(type);
        if (!GameController.instance.isGameOver)
        {
            if (!isSwitchPlane)
            {
                if (!isJumping)
                {
                    Debug.Log("Jump");
                    rigid.AddForce(Vector2.up * jumpForce);
                    isJumping = true;
                    animator.SetInteger("status", 1);
                }
            }
        }
    }

    public void SpawnMom() {

        mom = Instantiate(momPrefab);
        Vector3 newPos = this.transform.position;
        newPos.y = this.laneObjects[(int)currentLane].transform.position.y;
        newPos.x += 15f;
        mom.transform.position = newPos;
    }

    public void DestroyMom() {
        Destroy(mom);
        soundEffect.GetComponent<SoundInfo>().mom.Play();
        endingWord.SetActive(true);
        replayButton.SetActive(true);
    }
	
	void Update () {
        if (!GameController.instance.isGameOver)
        {
            if (isSwitchPlane && !isJumping)
            {
                time += Time.deltaTime / timeToReachTarget;
                transform.position = Vector3.Lerp(startPosition, target, time);
                if (Vector3.Distance(transform.position, target) < 0.01)
                {
                    isSwitchPlane = false;
                }
            }
            CheckJump();
            CheckSwitchLane();
            //CheckJumpTab();
        }
        //// Track a single touch as a direction control.
        //if (Input.touchCount > 0)
        //{
        //    Touch touch = Input.GetTouch(0);

        //    // Handle finger movements based on touch phase.
        //    switch (touch.phase)
        //    {
        //        // Record initial touch position.
        //        case TouchPhase.Began:
        //            startPos = touch.position;
        //            directionChosen = false;
        //            break;

        //        // Determine direction by comparing the current touch position with the initial one.
        //        case TouchPhase.Moved:
        //            direction = touch.position - startPos;
        //            break;

        //        // Report that a direction has been chosen when the finger is lifted.
        //        case TouchPhase.Ended:
        //            directionChosen = true;
        //            break;
        //    }
        //}
        //if (directionChosen)
        //{
        //    // Something that uses the chosen direction...
        //}

        


    }
}
