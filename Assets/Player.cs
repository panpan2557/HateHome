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
	void Start () {
		rigid = this.GetComponent<Rigidbody2D>();
		laneObjects = GameController.instance.lanes;
		SwitchLanes(currentLane); // init and trigger lanes
		triggerColliders(currentLane);
	}

	public void SwitchLanes(lanes l) {
		Vector3 lanePos = laneObjects[(int)l].transform.position;
		lanePos.z = -1; // fixed z-axis: always front
		lanePos.y += this.GetComponent<SpriteRenderer>().bounds.size.y / 2;
		this.transform.position = lanePos;
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
		if (Input.GetKeyDown(KeyCode.Space) && !isJumping) {
            jumpTimeCount += Time.deltaTime;
            Debug.Log("Jump");
            rigid.AddForce(Vector2.up * jumpForce);
            isJumping = true;

        }
        if (Input.GetKey(KeyCode.Space) && isJumping) {
            jumpTimeCount += Time.deltaTime;
            if (jumpTimeCount < jumpTimeLimit)
            {
                Debug.Log("height jump");
                rigid.AddForce(Vector2.up * jumpForce / 20);
            }
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            jumpTimeCount = 0;
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
		}
    }
	
	void Update () {
		CheckSwitchLane();
		CheckJump();
	}
}
