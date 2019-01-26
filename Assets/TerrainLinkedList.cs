using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainLinkedList {
	public TerrainLinkedList(Sprite t) {
		this.t = t;
	}
	public Sprite t;
	private TerrainLinkedList nextBg;
	public void setNext(TerrainLinkedList n) {
		this.nextBg = n;
	}
	public TerrainLinkedList next() {
		return nextBg;
	} 
}
