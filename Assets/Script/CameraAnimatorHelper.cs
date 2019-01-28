using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimatorHelper : MonoBehaviour {
	public void DoneColliding() {
		this.GetComponent<Animator>().SetBool("isColliding", false);
	}
}
