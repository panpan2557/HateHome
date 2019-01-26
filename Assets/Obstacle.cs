using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {
	public float lanesSpeed;
	// Use this for initialization
	void Start () {
		lanesSpeed = GameController.instance.lanesSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		lanesSpeed = GameController.instance.lanesSpeed;
		Vector3 pos = this.transform.position;
		pos.x -= lanesSpeed * Time.deltaTime;
		this.transform.position = pos;
	}

	public void SetHeight(float height) {
		Vector2 size = GetComponent<SpriteRenderer>().size;
		size.y = height;
		// set obstacle height
		this.GetComponent<SpriteRenderer>().size = size;
		// set pole height
		this.transform.GetChild(0).GetComponent<SpriteRenderer>().size = size;
		// set collider
		float initYOffset = 0.405f; // for height = 0.5
		float unitOffset = 0.99f; // for +1 height
		float diffPortion = height - 0.5f; 
		float newYOffset = diffPortion * unitOffset + initYOffset;
		this.GetComponent<Collider2D>().offset = new Vector2(this.GetComponent<Collider2D>().offset.x, newYOffset);
	}
}
