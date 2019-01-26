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
    public int spawnRate = 6;
	public int frameCounter = 0;
	public float doubleSpawnChance = 0f;
	public int spawnAtFrame = 120;
	public float overlapOffset = 1f;
	public GameObject spawnPoint;
    private GameObject[] laneObjects;

    void Start () {
        Vector2 bgSize = bgPrefab.GetComponent<SpriteRenderer>().bounds.size;
        
        laneObjects = GameController.instance.lanes;
        Debug.Log(bgSize.y);
		bgSpeed = GameController.instance.lanesSpeed; // get lanes speed from the controller
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
			bg.transform.position = newPos;
			bg.GetComponent<SpriteRenderer>().sprite = currentTerrain.t;
            bgs.Add(bg);
		}
		currentTerrain = currentTerrain.next();
        Debug.Log("starttt    "+laneObjects.Length);
    }
	
	public void MoveBackground() {
		foreach (GameObject bg in bgs) {
			Vector3 pos = bg.transform.position;
			pos.x -= bgSpeed * Time.deltaTime;
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

	void FixedUpdate () {
		frameCounter++;
		if (frameCounter % spawnAtFrame == 0) {
			RandomInstantiateObstacle();
			if (spawnAtFrame > 30) {
				spawnAtFrame -= 1;
			}	
			frameCounter = 0;
		}
		MoveBackground();
		DetectBackground();
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
		doubleSpawnChance += 0.003f;
    }

	void Spawn(int randLane, bool isDoubleSpawned = false) {
		GameObject obstacle = Instantiate(obstacles[0]);
		float randPosX = spawnPoint.transform.position.x;
		Vector3 obsPosition = Vector3.zero;
		if (isDoubleSpawned) {
			obsPosition.x = randPosX + overlapOffset;
		} else {
			obsPosition.x = randPosX;
		}
		Debug.Log("laneY: " + laneObjects[randLane].transform.position.y);
		obsPosition.y = laneObjects[randLane].transform.position.y;
		obsPosition.y = obstacle.transform.position.y * (randLane + 1);
		obsPosition.z = -1;
		obstacle.transform.localPosition = obsPosition;
	}

    void ClearRandomObstacle(GameObject terrain) {
        foreach (Transform t in terrain.transform) {
            Destroy(t.gameObject);
        }
    }
}
