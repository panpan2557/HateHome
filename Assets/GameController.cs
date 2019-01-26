using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public GameObject[] lanes;
	
	public static GameController instance;
	void Start () {
		instance = this;
	}
	void Update () {
		
	}
}
