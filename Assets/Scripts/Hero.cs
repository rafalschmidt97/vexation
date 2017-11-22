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
	float lastRollTime = 0;

	void Start () {
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}
	
	void Update () {
		if (alive) {
			if (lastRollTime + 0.5 < Time.unscaledTime) {
				Vector3 newDirection = Vector3.zero;

				if (Input.GetAxis("Horizontal") < 0) {
				newDirection += Vector3Int.left;
				}
					
				if (Input.GetAxis("Horizontal") > 0) {
					newDirection += Vector3Int.right;
				}
					
				if (Input.GetAxis("Vertical") > 0) {
					newDirection += Vector3.up;
				}
					
				if (Input.GetAxis("Vertical") < 0) {
					newDirection += Vector3.down;
				}

				if (Input.GetButtonDown("Jump") && newDirection.magnitude > 0 && lastRollTime + 1 < Time.unscaledTime) {
					animator.SetTrigger("roll");
					lastRollTime = Time.unscaledTime;
					rb.velocity = Vector2.ClampMagnitude(newDirection, 1f) * speed * 1.75f;
				} else {
					rb.velocity = Vector2.ClampMagnitude(newDirection, 1f) * speed;
				}

				if (Input.GetButtonDown("Fire1")) {
					StartCoroutine(AnimateRotation());
				}

				if (Input.GetButtonDown("Fire3")) {
					alive = false;

					rb.velocity = Vector2.zero;
					animator.SetBool("alive", false);
					animator.SetLayerWeight(2,1);
				}

				if (newDirection.magnitude > 0 && (newDirection.x != direction.x || newDirection.y != direction.y)) {
					animator.SetInteger("x", (int)newDirection.x);
					animator.SetInteger("y", (int)newDirection.y);
				}		

				if (newDirection.magnitude > 0) {
					animator.SetLayerWeight(1,1);
				} else {
					animator.SetLayerWeight(1,0);
				}

				direction = newDirection;
			}

		}
	}

	IEnumerator AnimateRotation() {
		animator.SetTrigger("rotate");
		animator.SetLayerWeight(2,1);
		yield return new WaitForSeconds(0.9f);
		animator.SetInteger("x", 0);
		animator.SetInteger("y", -1);
		animator.SetLayerWeight(2,0);
	}
}
