/*
* Copyright (c) Rafał Schmidt
* http://rafalschmidt.com
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {
	Rigidbody2D rb;

	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}

	void Update () {
		if (rb.velocity == Vector2.zero) {
			StartCoroutine(DestroyArrow());
		}
	}
	
	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.tag == "Wall") {
			StartCoroutine(DestroyArrow());
    }
	}

	IEnumerator DestroyArrow() {
		rb.velocity = Vector2.zero;
		yield return new WaitForSeconds(5);
		Destroy(gameObject);
	}
}
