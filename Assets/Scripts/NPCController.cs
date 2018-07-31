using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

	public float speed = 0.1f;

	private const int ALONE = 0;
	private const int FRIEND_SEEN = 1;
	private const int WITH_FRIEND = 2;
	private const int PLAYER_NEAR = 3;
	private const int PLAYER_SPOKE = 4;
	private const int RUN_AWAY = 5;
	private const int ATTACK = 6;

	public int status;

	private int playerColCount = 0;
	private int NPCColCount = 0;

	private ParticleSystem emision;
	private Light innerLight;
	private Vector3 targetPosition = new Vector3(0.0f, 0.0f, 0.0f);
	private float defaultIntensity;
	private float rangoX, rangoZ;

	private PlayerController player;
	private AudioSource wind;
	private Light sun;

	private Vector3 direction, current_dir;

	// Use this for initialization
	void Start () {

		wind = GameObject.Find("Wind").GetComponent<AudioSource>();
		sun = GameObject.Find("Sun").GetComponent<Light>();

		emision = transform.GetChild(1).GetComponent<ParticleSystem>();
		emision.Stop();
		innerLight = transform.GetChild(0).GetComponent<Light>();

		innerLight.intensity = 1.0f;
		defaultIntensity = 1.0f;;

		direction = targetPosition - transform.position;
		direction.Normalize();
		current_dir = new Vector3(0.0f, 0.0f, 0.0f);

		targetPosition = transform.position;

		status = ALONE;
	}
	
	// Update is called once per frame
	void Update () {

		switch (status) {

			case PLAYER_NEAR:
				emision.Stop();
				if(player.shining) { 
					fightOrFlight();
				}
			break;

			case ATTACK:
				attack();
			break;

			case RUN_AWAY:
				runAway();
			break;

			case ALONE:
				patroll();
			break;

			case FRIEND_SEEN:
				walkToTarget();
			break;

			case WITH_FRIEND:
				if (!emision.isEmitting) { emision.Play(); }
			break;

			default:
				if (innerLight.intensity != defaultIntensity) {
					innerLight.intensity = defaultIntensity;
				}
				if(emision.isStopped){
					emision.Play();		
				} 
			break;
		}
	}

	void patroll(){

		if (current_dir == direction){
			transform.position += direction * speed;
			current_dir = targetPosition - transform.position;
			current_dir.Normalize();
		} else {

			rangoX = Random.Range(-10.0f, 10.0f);
			StartCoroutine("littleDelay");
			rangoZ = Random.Range(-10.0f, 10.0f);

			targetPosition = new Vector3(targetPosition.x + rangoX, targetPosition.y, targetPosition.z + rangoZ);

			//Debug.LogWarning(targetPosition);

			direction = targetPosition - transform.position;
			direction.Normalize();

			current_dir = direction;
		}
		return;
	}

	void walkToTarget() {

		direction = -(transform.position - targetPosition);
		float distance = Vector3.Distance(transform.position, targetPosition);
		direction.Normalize();
		transform.position += direction * speed;
		if ( distance <= 2.0f) {
			status = WITH_FRIEND;
		}
		return;
	}

	void runAway() {

		transform.position -= direction*speed*5.0f;

		if ( Vector3.Distance(transform.position, player.transform.position) > 100.0f ){
			Destroy(gameObject);
		}
	}

	void attack() {

		transform.position += direction*speed*5.0f;
	}

	void fightOrFlight() {

		emision.Stop();

		direction = player.transform.position - transform.position;
		direction.Normalize();

		float coin = Random.value;
		if (coin < 0.5f) {
			status = ATTACK;
		} else {
			direction = direction;
			status = RUN_AWAY;
			increaseOpression();
		}

		//status = ATTACK;
	}

	void increaseOpression() {

		innerLight.intensity = 0.0f;

		wind.volume = Mathf.Lerp(wind.volume, wind.volume*2f, 0.5f);
		RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, RenderSettings.fogDensity*1.25f, 0.5f);
		sun.intensity = Mathf.Lerp(sun.intensity, sun.intensity*0.9f, 0.5f);
	}

	void OnTriggerEnter(Collider c) {

		if (status == RUN_AWAY) {
			return;
		}

		if (c.gameObject.name == "Player") {

			playerColCount++;
			player = c.GetComponent<PlayerController>();

			status = PLAYER_NEAR;

			Object.FindObjectOfType<Canvas>().enabled = true;

		} else if (c.gameObject.tag == "NPC" && (status == ALONE || status == FRIEND_SEEN) ) {

			if ( c.gameObject.GetComponent<NPCController>().status == RUN_AWAY ) {
				return;
			}

			NPCColCount++; 

			status = FRIEND_SEEN;
			targetPosition = c.transform.position;
			/*
			if (NPCColCount == 1) {

				status = FRIEND_SEEN;
				direction = targetPosition - transform.position;
				direction.Normalize();
				//Debug.LogWarning("NPC detected!");

			} else if (NPCColCount == 2) {

				status = WITH_FRIEND;
				//Debug.LogWarning("NPC is close!!");
			}*/
		}
	}

	void OnTriggerExit(Collider c) {

		if (status == RUN_AWAY) {
			return;
		}

		if (c.gameObject.name == "Player") {

			playerColCount--;
			status = ALONE;

		}
	}

	void OnCollisionEnter(Collision c) {

		if (c.rigidbody != null && c.gameObject.name == "Player") {

			//Debug.LogWarning("Crash with player!");

			status = RUN_AWAY;
			increaseOpression();
		
		} else if (c.gameObject.tag == "NPC" && c.gameObject.GetComponent<NPCController>().status != RUN_AWAY) {
			status = WITH_FRIEND;
		}
	}

	IEnumerator littleDelay() {

		yield return new WaitForSeconds(0.01f);
	}
}
