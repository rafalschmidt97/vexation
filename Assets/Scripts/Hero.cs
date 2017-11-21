/*
* Copyright (c) Rafał Schmidt
* http://rafalschmidt.com
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {

	[SerializeField]
	float speed;
	
	Animator animator;
	Rigidbody2D rb;
    bool alive = true;
	Vector2 direction = Vector2.zero;

	void Start () {
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}
	
	void Update () {
		// if(alive != true) {
        //     rb.velocity = Vector2.zero;
        //     return;
        // }

		Vector3 newDirection = Vector3.zero;
		
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") < 0) {
			newDirection += Vector3Int.left;
		}
			
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("Horizontal") > 0) {
			newDirection += Vector3Int.right;
		}
			
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetAxis("Vertical") > 0) {
			newDirection += Vector3.up;
		}
			
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetAxis("Vertical") < 0) {
			newDirection += Vector3.down;
		}
		
		if (newDirection.magnitude > 0 && (newDirection.x != direction.x || newDirection.y != direction.y)) {
			animator.SetInteger("x", (int)newDirection.x);
			animator.SetInteger("y", (int)newDirection.y);
		}

		rb.velocity = Vector2.ClampMagnitude(newDirection, 1f) * speed;
		// transform.Translate(newDirection * speed * Time.deltaTime);


		if (newDirection.magnitude > 0) {
			animator.SetLayerWeight(1,1);
		} else {
			animator.SetLayerWeight(1,0);
		}

        // animator.SetBool("walking", newDirection.magnitude > 0);

		direction = newDirection;
	}
}
