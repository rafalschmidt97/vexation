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
    Vector2 direction = Vector2.zero;
    float lastRollTime = 0;

    void Start () {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health.Initialize(initialHealth);
        stamina.Initialize(initialStamina);
		InvokeRepeating("UpdateStamina", 0, initialStaminaUpdateSpeed);
    }
    
    void Update () {
        if (health.Value > 0) {
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

                if (Input.GetButtonDown("Roll") && newDirection.magnitude > 0 && lastRollTime + 1 < Time.unscaledTime && stamina.Value > 0) {
					stamina.Value -= 1;
                    animator.SetTrigger("roll");
                    lastRollTime = Time.unscaledTime;
                    rb.velocity = Vector2.ClampMagnitude(newDirection, 1f) * speed * 1.75f;
                } else {
                    rb.velocity = Vector2.ClampMagnitude(newDirection, 1f) * speed;
                }

                if (Input.GetButtonDown("Fire")) {
                    StartCoroutine(AnimateRotation());
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
                    animator.SetLayerWeight(1,1);
                } else {
                    animator.SetLayerWeight(1,0);
                }

                direction = newDirection;
            }
        } else {
			rb.velocity = Vector2.zero; 
			animator.SetBool("alive", false); 
         	animator.SetLayerWeight(2,1);
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

	void UpdateStamina() {
		if (health.Value > 0)
			stamina.Value += 1;
	}
}