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
    private bool isPanning;

    public Animator animator;

	void Start () {
		rigid = this.GetComponent<Rigidbody2D>();
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
				GameController.instance.CollideWithObstacle();
			}
		}
	}
	
	void Update () {
        if (isSwitchPlane && !isJumping) {
            time += Time.deltaTime / timeToReachTarget;
            transform.position = Vector3.Lerp(startPosition, target, time);
            if (Vector3.Distance(transform.position, target) < 0.01) {
                isSwitchPlane = false;
            }
        }

        CheckJump();
        CheckSwitchLane();

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

        //if (Input.GetMouseButtonDown(0))
        //{
        //    mouseOrigin = Input.mousePosition;
        //    isPanning = true;
        //}

        //if (!Input.GetMouseButton(0))
        //    isPanning = false;

        //if (isPanning)
        //{
        //    Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);


        //    Vector3 move = new Vector3(pos.x, 0, 0);
        //    transform.Translate(move * panSpeed * Time.deltaTime, Space.Self);

        //    Vector3 clampedPosition = transform.position;
        //    clampedPosition.x = Mathf.Clamp(transform.position.x, leftValue, rightValue);
        //    clampedPosition.z = Mathf.Clamp(transform.position.z, zStabilizer, zStabilizer);
        //    transform.position = clampedPosition;

        //}


    }
}
