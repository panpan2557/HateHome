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
}
