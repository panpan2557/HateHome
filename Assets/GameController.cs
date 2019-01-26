using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public GameObject[] lanes;
    public GameObject[] obstacles;
	public GameObject sun;
	public float maxLanesSpeed;
	public float lanesSpeed;
	public float currentSunHeight, maxSunHeight, minSunHeight, sunsetSpeed;
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
		} else {
			// Continue
			// alter the sunset speed
			float calSpeed = sunsetSpeed;
			// if (lane)
			SunSetting();
		}
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
