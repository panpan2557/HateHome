using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public GameObject[] lanes;
    public GameObject[] obstacles;
	public GameObject sun;
	public float maxLanesSpeed;
	public float lanesSpeed;
	public float rushLanesSpeed = 1f;
    public float revertSunsetSpeed = 2f;
	public float currentSunHeight, maxSunHeight, minSunHeight, maxSunsetSpeed, sunsetSpeed;
	public float collideSpeedPenalty = 2f;
    public float endingSpeed = 10f;
	public bool isGameOver;
	public static GameController instance;
    private float time;
    private float distanceCounter;
    private bool canPlayEnding;

    public List<GameObject> buildings;
    public List<GameObject> skys;
    public Text score;
    void Awake () {
		instance = this;
		lanesSpeed = maxLanesSpeed;
	}
	void Start() {
        maxSunHeight = this.sun.GetComponent<SunSystemInfo>().sun.transform.eulerAngles.z;
        currentSunHeight = maxSunHeight;
        //currentSunHeight = minSunHeight;
        buildings = this.sun.GetComponent<SunSystemInfo>().buildings;
        skys = this.sun.GetComponent<SunSystemInfo>().skys;
        canPlayEnding = false;
        distanceCounter = 0;
    }
	void Update () {



		if (isSunAtMin()) {
			// Gameover
			Debug.Log("Gameover!");
            isGameOver = true;
            
            // Destroy all obstacles
            GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
			foreach (GameObject o in obstacles) {
                //Destroy(o);
                o.GetComponent<BoxCollider2D>().enabled = false;
			}
            if (canPlayEnding)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().SetInteger("status", 3);
                //GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().SetBool("isGameOver", true);
                // Stop lanes, obstacle generation
                if (lanesSpeed > 0) {
                    //Debug.LogError(lanesSpeed + "/" + endingSpeed * Time.deltaTime);
                    lanesSpeed -= endingSpeed * Time.deltaTime;
                }
                else
                {
                    lanesSpeed = 0;
                    //GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().DestroyMom();
                }

                if (this.sun.GetComponent<SunSystemInfo>().skysSpeed > 0)
                {
                    //Debug.LogError(this.sun.GetComponent<SunSystemInfo>().skysSpeed);
                    this.sun.GetComponent<SunSystemInfo>().skysSpeed -= endingSpeed * Time.deltaTime;
                }
                else
                {
                    this.sun.GetComponent<SunSystemInfo>().skysSpeed = 0;
                }

                if (this.sun.GetComponent<SunSystemInfo>().buildingsSpeed > 0)
                {
                    //Debug.LogError(this.sun.GetComponent<SunSystemInfo>().buildingsSpeed);
                    this.sun.GetComponent<SunSystemInfo>().buildingsSpeed -= endingSpeed * Time.deltaTime;
                }
                else
                {
                    this.sun.GetComponent<SunSystemInfo>().buildingsSpeed = 0;
                }
            }
            else {
                if (lanesSpeed <= maxLanesSpeed)
                { // accelerate when do not collide
                    if (lanesSpeed + rushLanesSpeed * Time.deltaTime >= maxLanesSpeed)
                    {
                        Debug.Log("Can play Ending");
                        lanesSpeed = maxLanesSpeed;
                        canPlayEnding = true;
                    }
                    else
                    {
                        lanesSpeed += rushLanesSpeed * Time.deltaTime;
                    }
                }
                else {
                    Debug.Log("Can play Ending");
                    lanesSpeed = maxLanesSpeed;
                    canPlayEnding = true;
                }
            }
            

            // Prepare for End Scene
            // - Trigger something in animator
        } else{
            //distanceCounter = lanesSpeed * Time.deltaTime;
			// Continue
			if (lanesSpeed < maxLanesSpeed) { // accelerate when do not collide
				if (lanesSpeed + rushLanesSpeed * Time.deltaTime >= maxLanesSpeed)
					lanesSpeed = maxLanesSpeed;
				else
					lanesSpeed += rushLanesSpeed * Time.deltaTime;
			}



			float calSpeed = this.sunsetSpeed;
			if (maxLanesSpeed - lanesSpeed != 0) { // slower than max lanes speed
				if (calSpeed + 0.5f * Time.deltaTime >= maxSunsetSpeed) 
					calSpeed = maxSunsetSpeed;
				else
					calSpeed -= 0.5f * Time.deltaTime;
			} else if (maxLanesSpeed - lanesSpeed == 0) { // catch up the max speed
				if (currentSunHeight + calSpeed * Time.deltaTime >= maxSunHeight) {
					calSpeed = 0;
					currentSunHeight = maxSunHeight;
				} else
					calSpeed += 0.3f * Time.deltaTime; 
			}
			this.sunsetSpeed = calSpeed;
			SunSetting();
		}
	}

	public void CollideWithObstacle() {
        if (this.lanesSpeed - collideSpeedPenalty <= 0)
        {
            this.lanesSpeed = 0;
        }
        else {
            this.lanesSpeed -= collideSpeedPenalty;
        }
	}

	void SunSetting() {
        GameObject sun = this.sun.GetComponent<SunSystemInfo>().sun;
        Vector3 rot = sun.transform.eulerAngles;
        Debug.Log("rot.z: " + rot.z + ", maxSunHeight: " + maxSunHeight);
        if (rot.z > minSunHeight) {
            rot.z += sunsetSpeed * Time.deltaTime;
            sun.transform.eulerAngles = rot;
        } else
        {
            rot.z = minSunHeight;
        }
        currentSunHeight = rot.z;

        for (int i = 0; i < buildings.Count; i++)
        {
            SpriteRenderer b = buildings[i].GetComponent<SpriteRenderer>();
            SpriteRenderer s = skys[i].GetComponent<SpriteRenderer>();
            Color color = b.color;
            color.a = 1 - ((maxSunHeight - currentSunHeight) / 20);
            b.color = color;
            s.color = color;
        }

        //sun.GetComponentInChildren<Light>().intensity = 0;
        //for (int i = 0; i < buildings.Count; i++)
        //{
        //    SpriteRenderer b = buildings[i].GetComponent<SpriteRenderer>();
        //    SpriteRenderer s = skys[i].GetComponent<SpriteRenderer>();
        //    Color color = b.color;
        //    color.a = 0;
        //    b.color = color;
        //    s.color = color;
        //}

        //SpriteRenderer building = this.sun.GetComponent<SunSystemInfo>().building.GetComponent<SpriteRenderer>();
        //Color colorB = building.color;
        //colorB.a = 1 - ((maxSunHeight - currentSunHeight) / 20);
        //building.color = colorB;

        //SpriteRenderer sky = this.sun.GetComponent<SunSystemInfo>().sky.GetComponent<SpriteRenderer>();
        //Color colorS = sky.color;
        //colorS.a = 1 - ((maxSunHeight - currentSunHeight) / 20);
        //sky.color = colorS;

        //SpriteRenderer building = this.sun.GetComponent<SunSystemInfo>().building.GetComponent<SpriteRenderer>();
        //Color colorB = building.color;
        //colorB.a = 0;
        //building.color = colorB;

        //SpriteRenderer sky = this.sun.GetComponent<SunSystemInfo>().sky.GetComponent<SpriteRenderer>();
        //Color colorS = sky.color;
        //colorS.a = 0;
        //sky.color = colorS;

        //      Vector3 pos = sun.transform.position;
        //pos.y -= sunsetSpeed * Time.deltaTime;
        //sun.transform.position = pos;
        //currentSunHeight = pos.y;
    }

    public void RerunAgain() {
        SceneManager.LoadScene("Intro");
    }

	bool isSunAtMin() {

		return currentSunHeight <= minSunHeight;
	}

	void UpdateSunsetSpeed() {

	}
}
