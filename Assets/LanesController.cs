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
    private GameObject[] laneObjects;

    void Start () {
        Vector2 bgSize = bgPrefab.GetComponent<SpriteRenderer>().bounds.size;
        
        laneObjects = GameController.instance.lanes;
        Debug.Log(bgSize);
        //for (int i =0; i < laneObjects.Length; i++) {
        //    Vector3 v = laneObjects[i].transform.position;
        //    v.y = bgSize.y / 3 * i;
        //    laneObjects[i].transform.position = v;
        //}
        // init terrain linked list
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
            //newPos.z = 1;
			bg.transform.position = newPos;

            bg.GetComponent<SpriteRenderer>().sprite = currentTerrain.t;
            RandomInstantiateObstacle(bg);

            bgs.Add(bg);
		}
		currentTerrain = currentTerrain.next();
        //laneObjects = GameController.instance.lanes;
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
        RandomInstantiateObstacle(firstBg);
        bgs.RemoveAt(0);
		bgs.Add(firstBg);
	}

	// Update is called once per frame
	void Update () {
		MoveBackground();
		DetectBackground();
	}

    void RandomInstantiateObstacle(GameObject terrain) {
        int randAmount = Random.Range(spawnRate - 5, spawnRate + 5);
        for (int i = 0; i < randAmount; i++) {
            //GameObject obstacle = Instantiate(obstacles[Random.Range(0, obstacles.Length)]);
            GameObject obstacle = Instantiate(obstacles[0]);
            int randPosX = Random.Range(-128, 128);
            int randLane = Random.Range(0, laneObjects.Length);
            obstacle.transform.parent = terrain.transform;
            Vector3 obsPosition = Vector3.zero;
            obsPosition.x = randPosX/10f;
            Debug.LogError(obstacle.transform.position.y + "/"+ (randLane + 1));
            obsPosition.y = obstacle.transform.position.y * (randLane + 1);
            obsPosition.z = -1;
            obstacle.transform.position = obsPosition;
        }
        spawnRate++;
    }

    void ClearRandomObstacle(GameObject terrain) {
        foreach (Transform t in terrain.transform) {
            Destroy(t.gameObject);
        }
    }
}
