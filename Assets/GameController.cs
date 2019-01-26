using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public GameObject[] lanes;
    public GameObject[] obstacles;
	public GameObject sun;
	public float maxLanesSpeed;
	public float lanesSpeed;
	public float rushLanesSpeed = 1f;
	public float currentSunHeight, maxSunHeight, minSunHeight, maxSunsetSpeed, sunsetSpeed;
	public float collideSpeedPenalty = 2f;
	public bool isGameOver;
	public static GameController instance;
	void Awake () {
		instance = this;
		lanesSpeed = maxLanesSpeed;
	}
	void Start() {
		currentSunHeight = maxSunHeight;
	}
	void Update () {
		if (isSunAtMin()) {
			// Gameover
			Debug.Log("Gameover!");
			isGameOver = true;
			// Stop lanes, obstacle generation
			lanesSpeed = 0f;
			// Destroy all obstacles
			GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
			foreach (GameObject o in obstacles) {
				Destroy(o);
			}
			// Prepare for End Scene
			// - Trigger something in animator
		} else {
			// Continue
			if (lanesSpeed < maxLanesSpeed) { // accelerate when do not collide
				if (lanesSpeed + rushLanesSpeed * Time.deltaTime >= maxLanesSpeed)
					lanesSpeed = maxLanesSpeed;
				else
					lanesSpeed += rushLanesSpeed * Time.deltaTime;
			}

			float calSpeed = this.sunsetSpeed;
			if (maxLanesSpeed - lanesSpeed != 0) { // slower than max lanes speed
				if (calSpeed + 0.3f * Time.deltaTime >= maxSunsetSpeed) 
					calSpeed = maxSunsetSpeed;
				else
					calSpeed += 0.3f * Time.deltaTime;
			} else if (maxLanesSpeed - lanesSpeed == 0) { // catch up the max speed
				if (currentSunHeight + calSpeed * Time.deltaTime >= maxSunHeight) {
					calSpeed = 0;
					currentSunHeight = maxSunHeight;
				} else
					calSpeed -= 0.3f * Time.deltaTime; 
			}
			this.sunsetSpeed = calSpeed;
			SunSetting();
		}
	}

	public void CollideWithObstacle() {
		this.lanesSpeed -= collideSpeedPenalty;
	}

	void SunSetting() {
		Vector3 pos = sun.transform.position;
		pos.y -= sunsetSpeed * Time.deltaTime;
		sun.transform.position = pos;
		currentSunHeight = pos.y;
	}

	bool isSunAtMin() {
		return currentSunHeight <= minSunHeight;
	}

	void UpdateSunsetSpeed() {

	}
}
