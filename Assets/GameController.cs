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
	public static GameController instance;

    public List<GameObject> buildings;
    public List<GameObject> skys;
    void Awake () {
		instance = this;
		lanesSpeed = maxLanesSpeed;
	}
	void Start() {
        maxSunHeight = this.sun.GetComponent<SunSystemInfo>().sun.transform.eulerAngles.z;
        currentSunHeight = maxSunHeight;
        buildings = this.sun.GetComponent<SunSystemInfo>().buildings;
        skys = this.sun.GetComponent<SunSystemInfo>().skys;

    }
	void Update () {
		if (isSunAtMin()) {
			// Gameover
			Debug.Log("Gameover!");
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
        GameObject sun = this.sun.GetComponent<SunSystemInfo>().sun;
        Vector3 rot = sun.transform.eulerAngles;
        if (rot.z > 333)
        {
            rot.z -= sunsetSpeed * Time.deltaTime;
            sun.transform.eulerAngles = rot;
            currentSunHeight = rot.z;

            //Debug.Log(1 - ((maxSunHeight - currentSunHeight) / 20));
            for (int i = 0; i < buildings.Count; i++) {
                SpriteRenderer b = buildings[i].GetComponent<SpriteRenderer>();
                SpriteRenderer s = skys[i].GetComponent<SpriteRenderer>();
                Color color = b.color;
                color.a = 1 - ((maxSunHeight - currentSunHeight) / 20);
                b.color = color;
                s.color = color;
            }
            //SpriteRenderer building = this.sun.GetComponent<SunSystemInfo>().building.GetComponent<SpriteRenderer>();
            //Color colorB = building.color;
            //colorB.a = 1 - ((maxSunHeight - currentSunHeight) / 20);
            //building.color = colorB;

            //SpriteRenderer sky = this.sun.GetComponent<SunSystemInfo>().sky.GetComponent<SpriteRenderer>();
            //Color colorS = sky.color;
            //colorS.a = 1 - ((maxSunHeight - currentSunHeight) / 20);
            //sky.color = colorS;
        }
        else {
            sun.GetComponentInChildren<Light>().intensity = 0;
            for (int i = 0; i < buildings.Count; i++)
            {
                SpriteRenderer b = buildings[i].GetComponent<SpriteRenderer>();
                SpriteRenderer s = skys[i].GetComponent<SpriteRenderer>();
                Color color = b.color;
                color.a = 0;
                b.color = color;
                s.color = color;
            }
            //SpriteRenderer building = this.sun.GetComponent<SunSystemInfo>().building.GetComponent<SpriteRenderer>();
            //Color colorB = building.color;
            //colorB.a = 0;
            //building.color = colorB;

            //SpriteRenderer sky = this.sun.GetComponent<SunSystemInfo>().sky.GetComponent<SpriteRenderer>();
            //Color colorS = sky.color;
            //colorS.a = 0;
            //sky.color = colorS;
        }

        //      Vector3 pos = sun.transform.position;
        //pos.y -= sunsetSpeed * Time.deltaTime;
        //sun.transform.position = pos;
        //currentSunHeight = pos.y;
    }

	bool isSunAtMin() {
		return currentSunHeight <= minSunHeight;
	}

	void UpdateSunsetSpeed() {

	}
}
