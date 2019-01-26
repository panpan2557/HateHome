using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimatorHelper : MonoBehaviour {
	public GameObject boyEmoji;
	public Sprite[] boyEmojiSprites;
	public void RandomEmoSprite() {
		int i = Random.Range(0, boyEmojiSprites.Length);
		boyEmoji.GetComponent<SpriteRenderer>().sprite = boyEmojiSprites[i];
	}

	public void MoveToMainScene() {
		SceneManager.LoadScene("Main");
	}
}
