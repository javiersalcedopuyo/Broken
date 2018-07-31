using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private const int RUN_AWAY = 5;

	public float speed = 0.1f;
	public bool shining = false;

	private ParticleSystem emission;
	private AudioSource steps;
	private GameObject NPCContainer;
	private Transform NPC, target;

	private Vector3 targetPos, direction;
	private float dist, min_dist;
	private int targetID = 0;
	private bool playerControl = true;

	// Use this for initialization
	void Start () {

		NPCContainer = GameObject.Find("NPCs");
		
		steps = GetComponent<AudioSource>();
		emission = transform.transform.GetChild(1).GetComponent<ParticleSystem>();

		steps.volume = 0.5f;
		steps.Stop();
		emission.Play();
		emission.Stop();

		//shining = true;
	}
	
	// Update is called once per frame
	void Update () {


		if (playerControl) {

			transform.Translate(Vector3.down * Input.GetAxis("Vertical") * speed);
			transform.Rotate(Vector3.forward, Input.GetAxis("Horizontal"));

			if (Input.GetAxis("Vertical") != 0.0f && !steps.isPlaying) {
				steps.Play();
			}

			if (Input.GetKeyDown("space") ){

				if (emission.isStopped) {
					emission.Play();
					shining = true;
				}else if (emission.isPlaying) {
					emission.Stop();
					shining = false;
				}
			} 
		} else {
			
			shining = true;
			if (emission.isStopped) {
				emission.Play();
			}
			min_dist = float.MaxValue;
			for (int i=0; i< NPCContainer.transform.childCount; i++) {

				NPC = NPCContainer.transform.GetChild(i);

				dist = Vector3.Distance(transform.position, NPC.position);

				if (dist < min_dist && NPC.gameObject.GetComponent<NPCController>().status != RUN_AWAY) {
					min_dist = dist;
					targetID = i;
				}
			}

			target = NPCContainer.transform.GetChild(targetID);

			direction = target.position - transform.position;
			direction.Normalize();

			transform.position += direction*speed;

			dist = Vector3.Distance(target.position, transform.position);
		}

		if (Input.GetKeyDown("escape")) {
			Application.Quit();
			Debug.LogWarning("EXIT");
		} else if (Input.GetKeyDown("return")) {
			playerControl = !playerControl;
		}
	}
}
