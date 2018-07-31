using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class startScreen : MonoBehaviour {

	CanvasGroup img;
	GameObject NPCContainer;

	// Use this for initialization
	void Start () {

		NPCContainer = GameObject.Find("NPCs");
		
		img = GameObject.Find("start_screen").GetComponent<CanvasGroup>();
		StartCoroutine("fadeOut");
	}

	void Update() {

		if (NPCContainer.transform.childCount <= 0) {

			Debug.LogWarning("FadeIn black screen.");
			fadeIn();
			Debug.LogWarning("EXIT");
			Application.Quit();
		}
	}

	void fadeIn() {

		Image i = GameObject.Find("start_screen").GetComponent<Image>();
		i.CrossFadeAlpha(1.0f, 5.0f, false);
	}

	IEnumerator fadeOut() {

		while (img.alpha > 0.0f) {
			img.alpha -= 0.025f;
			yield return new WaitForSeconds(0.1f);
		}
	}
}
