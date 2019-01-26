using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanesController : MonoBehaviour {
	public GameObject bgPrefab;
	public GameObject detectPoint;
	public Sprite[] terrains;
	TerrainLinkedList t1, t2;
	TerrainLinkedList currentTerrain;
	List<GameObject> bgs = new List<GameObject>();
	public float bgSpeed;
	public int numberOfBgs;
	public int numOfRemovedBgs;
	public int numChangeTerrain = 10;
	void Start () {
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
		bgs.RemoveAt(0);
		bgs.Add(firstBg);
	}

	// Update is called once per frame
	void Update () {
		MoveBackground();
		DetectBackground();
	}
}
