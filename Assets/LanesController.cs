using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanesController : MonoBehaviour {
	public GameObject bgPrefab;
	public GameObject detectPoint;
	public Sprite[] terrains;
    public GameObject[] obstacles;
	TerrainLinkedList t1, t2;
	TerrainLinkedList currentTerrain;
	List<GameObject> bgs = new List<GameObject>();
	public float bgSpeed;
	public int numberOfBgs;
	public int numOfRemovedBgs;
	public int numChangeTerrain = 10;
	public float frameCounter = 0f;
	public float doubleSpawnChance = 0f;
	public float spawnPeriod = 6f;
	public float minSpawnPeriod = 0.5f;
	public float overlapOffset = 1f;
	public GameObject spawnPoint;
    private GameObject[] laneObjects;
	private bool isGameOver;
	public static LanesController instance;
    private GameObject player;
	void Awake () {
		instance = this;
	}

    void Start () {
        Vector2 bgSize = bgPrefab.GetComponent<SpriteRenderer>().bounds.size;
        player = GameObject.FindGameObjectWithTag("Player");
        laneObjects = GameController.instance.lanes;
        Debug.Log(bgSize.y);
		bgSpeed = GetLanesSpeed();
		// init terrain linked list
		t1 = new TerrainLinkedList(terrains[0]);
		t2 = new TerrainLinkedList(terrains[1]);
		t1.setNext(t2);
		t2.setNext(t1);
		currentTerrain = t1;
		// init all bgs
		GameObject bg;
		for (int i = 0; i < numberOfBgs; i++) {
			bg = Instantiate(bgPrefab);
			bg.transform.parent = this.transform;
			Vector3 newPos = this.transform.position;
			newPos.x += bg.GetComponent<SpriteRenderer>().bounds.size.x * i;
            newPos.y = -3;
			bg.transform.position = newPos;
			bg.GetComponent<SpriteRenderer>().sprite = currentTerrain.t;
            bgs.Add(bg);
		}
		currentTerrain = currentTerrain.next();
        Debug.Log("starttt    "+laneObjects.Length);
    }

	float GetLanesSpeed() {
		return GameController.instance.lanesSpeed; // get lanes speed from the controller
	}	
	bool GetIsGameOver() {
		return GameController.instance.isGameOver; // get lanes speed from the controller
	}	

	public void MoveBackground() {
		foreach (GameObject bg in bgs) {
			Vector3 pos = bg.transform.position;
			pos.x -= (bgSpeed * Time.deltaTime);
			bg.transform.position = pos;
		}
	}

	public void DetectBackground(){
		GameObject bg = bgs[0]; // 1st bg only collides with the detect point
		if (isCollidedWithDetectPoint(bg)) {
			// prepare a position of the last bg
			Vector3 lastBgPos = bgs[numberOfBgs - 1].transform.position;
			lastBgPos.x += bg.GetComponent<SpriteRenderer>().bounds.size.x;
			bg.transform.position = lastBgPos;
			numOfRemovedBgs += 1;
			if (numOfRemovedBgs > numChangeTerrain) {
				bg.GetComponent<SpriteRenderer>().sprite = currentTerrain.t;
			}
			if (numOfRemovedBgs > numChangeTerrain + numberOfBgs) {
				numOfRemovedBgs = 0;
				currentTerrain = currentTerrain.next();
			}
			SwapBackground();
		}
	}

	bool isCollidedWithDetectPoint(GameObject bg) {
		return bg.transform.position.x + bg.GetComponent<SpriteRenderer>().bounds.size.x < detectPoint.transform.position.x;
	}

	void SwapBackground() {
		GameObject firstBg = bgs[0];
        ClearRandomObstacle(firstBg);
        bgs.RemoveAt(0);
		bgs.Add(firstBg);
	}

	void Update () {
		frameCounter += Time.deltaTime;
		bgSpeed = GetLanesSpeed();
		isGameOver = GetIsGameOver();
		if (!isGameOver) {
			if (frameCounter >= spawnPeriod) {
				RandomInstantiateObstacle();
				if (spawnPeriod > minSpawnPeriod) {
					spawnPeriod -= 0.1f;
				}	
				frameCounter = 0;
			}
			MoveBackground();
			DetectBackground();
		}
		
	}

	void RandomInstantiateObstacle() {
		float randChance = Random.Range(0f, 1f);
		Debug.Log("randChance: " + randChance);
		// Double spawn
		if (randChance < doubleSpawnChance) {
			List<int> list = new List<int>();
			for (int i = 0; i < laneObjects.Length; i++) {
				list.Add(i);
			}
			int laneI = Random.Range(0, list.Count);
			int randLane1 = list[laneI];
			// remove first element
			list.RemoveAt(laneI);
			laneI = Random.Range(0, list.Count);
			int randLane2 = list[laneI];
			Spawn(randLane1);
			Spawn(randLane2, isDoubleSpawned: true);
		}
		// Single spawn 
		else {
			int randLane = Random.Range(0, laneObjects.Length);
			Spawn(randLane);
		}
		doubleSpawnChance += 0.01f;
    }

	float randHeight(float min, float max) {
		float r = Random.Range(0f, 1f);
		r = Mathf.Pow(r, 3f);
		return min + (max - min) * r;
	}

	void Spawn(int randLane, bool isDoubleSpawned = false) {
		GameObject obstacle = Instantiate(obstacles[0]);
		// set obstacle's height ranges from [0.5, 1.6]
		float rh = randHeight(0.5f, 1.6f);
		obstacle.GetComponent<Obstacle>().SetHeight(rh);
		// record lane index i
		obstacle.GetComponent<ObstacleInfo>().lane = randLane;
        if (randLane == 0)
        {
            obstacle.GetComponent<SpriteRenderer>().sortingOrder = -15;
            obstacle.GetComponent<ObstacleInfo>().pole.GetComponent<SpriteRenderer>().sortingOrder = -12;
            obstacle.GetComponent<ObstacleInfo>().extra.GetComponent<SpriteRenderer>().sortingOrder = -14;
        }
        else if (randLane == 1) {
            obstacle.GetComponent<SpriteRenderer>().sortingOrder = -11;
            obstacle.GetComponent<ObstacleInfo>().pole.GetComponent<SpriteRenderer>().sortingOrder = -8;
            obstacle.GetComponent<ObstacleInfo>().extra.GetComponent<SpriteRenderer>().sortingOrder = -10;
        }
        else if (randLane == 2)
        {
            obstacle.GetComponent<SpriteRenderer>().sortingOrder = -7;
            obstacle.GetComponent<ObstacleInfo>().pole.GetComponent<SpriteRenderer>().sortingOrder = -4;
            obstacle.GetComponent<ObstacleInfo>().extra.GetComponent<SpriteRenderer>().sortingOrder = -6;
        }
        float randPosX = spawnPoint.transform.position.x;
		Vector3 obsPosition = Vector3.zero;
		if (isDoubleSpawned) {
			obsPosition.x = randPosX + overlapOffset;
		} else {
			obsPosition.x = randPosX;
		}
		Debug.Log("laneY: " + laneObjects[randLane].transform.position.y);
		obsPosition.y = laneObjects[randLane].transform.position.y;
		obsPosition.y = obstacle.transform.position.y - (randLane * 0.5f);
		obsPosition.z = -1;
		obstacle.transform.localPosition = obsPosition;
	}

    void ClearRandomObstacle(GameObject terrain) {
        foreach (Transform t in terrain.transform) {
            Destroy(t.gameObject);
        }
    }
}
