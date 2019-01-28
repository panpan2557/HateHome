using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObstacle : MonoBehaviour {
	private void OnTriggerEnter2D(Collider2D col) {
		Debug.Log("collide with: " + col.gameObject.name);
		if (col.gameObject.tag == "Obstacle") {
			Destroy(col.gameObject);
        }
	}
}
