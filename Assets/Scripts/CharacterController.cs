using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

	float speed = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		//transform.Translate( 0.0f, 0.0f, Input.GetAxis("Vertical") );
		//transform.Translate( Input.GetAxis("Horizontal"), 0.0f, 0.0f );

		transform.position += Vector3 (0.0f, 0.0f, Input.GetAxis ("Vertical"));
		transform.position += Vector3 (Input.GetAxis ("Horizontal"), 0.0f, 0.0f);
		
	}
}
