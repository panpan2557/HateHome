using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSystem : MonoBehaviour {
	public SpriteRenderer mom;
	public SpriteRenderer  son;

	public SpriteRenderer momDialog;
	public SpriteRenderer sonDialog;

	public Sprite[] emojiSprite;
	
	public List<string> dialog;

	public float emojiDuration =0.5f;
	private float timeCounter;

	public List<string[]> dialogsList;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		ShowDialog(dialog);
	}

	void ShowDialog(List<string> dialog){
		if(timeCounter > emojiDuration && dialog.Count>0){
			string[] text = dialog[0].Split(' ');
			if(text[0] == "mom"){
				momDialog.sprite = emojiSprite[int.Parse(text[1])];
			}else{
				sonDialog.sprite = emojiSprite[int.Parse(text[1])];
			}
			dialog.RemoveAt(0);
			timeCounter = 0;
		}
		timeCounter += Time.deltaTime;
	}
}
