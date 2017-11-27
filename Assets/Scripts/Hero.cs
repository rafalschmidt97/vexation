﻿/*
* Copyright (c) Rafał Schmidt
* http://rafalschmidt.com
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {

	[SerializeField]
	float walkSpeed;

	[SerializeField]
	float rollSpeed;

	[SerializeField]
	float initialHealth;

	[SerializeField]
	float initialStamina;

	[SerializeField]
	float initialStaminaUpdateSpeed;

	[SerializeField]
	Stat health;

	[SerializeField]
	Stat stamina;

	Animator animator;
	Rigidbody2D rb;
	Vector2 direction;
	float lastRollTime;

	bool Alive {
		get {
			return health.Value > 0;
		}
	}

	bool Roll {
		get {
			return lastRollTime + 0.5 > Time.unscaledTime;
		}
	}

	bool CanRoll {
		get {
			return stamina.Value > 0;
		}
	}

	void Start () {
		direction = Vector2.zero;
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		health.Initialize(initialHealth);
		stamina.Initialize(initialStamina);
		InvokeRepeating("UpdateStamina", 0, initialStaminaUpdateSpeed);
	}

	void FixedUpdate() {
		rb.velocity = Vector2.ClampMagnitude(direction, 1f) * (Roll ? rollSpeed : walkSpeed) * (Alive ? 1 : 0);
	}

	void Update () {
		if (Alive) {
			if (!Roll) {
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

				if (Input.GetButtonDown("Roll") && newDirection.magnitude > 0  && CanRoll) {
					stamina.Value -= 1;
					lastRollTime = Time.unscaledTime;
					animator.SetTrigger("roll");
				} 

				if (Input.GetButtonDown("R1")) {
					health.Value += 1;
				}

				if (Input.GetButtonDown("L1")) {
					health.Value -= 1;
				}

				if (newDirection.magnitude > 0 && (newDirection.x != direction.x || newDirection.y != direction.y)) {
					animator.SetInteger("x", (int)newDirection.x);
					animator.SetInteger("y", (int)newDirection.y);
				}  

				if (newDirection.magnitude > 0) {
					ActivateLayer("Walk");
				} else {
					ActivateLayer("Idle");
				}

				direction = newDirection;
			}
		} else {
			animator.SetBool("alive", false); 
			ActivateLayer("State");
		}
	}

	void UpdateStamina() {
		if (Alive && (lastRollTime + 0.5 + initialStaminaUpdateSpeed < Time.unscaledTime))
			stamina.Value += 1;
	}

	void ActivateLayer(string layerName) {
		for (int i = 0; i < animator.layerCount; i++) {
			animator.SetLayerWeight(i, 0);
		}

		animator.SetLayerWeight(animator.GetLayerIndex(layerName), 1);
	}
}