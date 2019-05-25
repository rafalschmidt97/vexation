/*
* Copyright (c) Rafał Schmidt
* http://rafalschmidt.com
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {
	[SerializeField] float destroyTime;
	[SerializeField] float collideTime;
	[SerializeField] float pickUpMaxVelocity;

	Rigidbody2D rb;
	IEnumerator coorutine;
	bool destroy;

	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}

	void Update () {
		if (rb.velocity.magnitude < pickUpMaxVelocity && coorutine == null) {
			coorutine = DestroyArrow(destroyTime);
			StartCoroutine(coorutine);
			destroy = true;
		}
	}
	
	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.tag == "Wall") {
			rb.velocity = Vector2.zero;
			coorutine = DestroyArrow(collideTime);
			StartCoroutine(coorutine);
    } else if (collider.tag == "Hero" && destroy == true && coorutine != null) {
			StopCoroutine(coorutine);
			collider.GetComponent<Hero>().Arrows += 1;
			Destroy(gameObject);
    }
	}

	IEnumerator DestroyArrow(float timeToDestroy) {
		yield return new WaitForSeconds(timeToDestroy);
		Destroy(gameObject);
	}
}
