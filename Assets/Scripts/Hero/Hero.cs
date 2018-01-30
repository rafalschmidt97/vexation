/*
* Copyright (c) Rafał Schmidt
* http://rafalschmidt.com
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Hero : MonoBehaviour {
	[SerializeField] float walkSpeed;
	[SerializeField] float runSpeed;
	[SerializeField] float rollSpeed;
	[SerializeField] float rollTime;
	[SerializeField] float arrowSpeed;
	[SerializeField] float arrowDragMax;
	[SerializeField] float arrowDragSpeed;
	[SerializeField] float zoomSize;
	[SerializeField] float zoomInSize;
	[SerializeField] float zoomInSpeed;
	[SerializeField] float zoomOutSpeed;
	[SerializeField] float initialHealth;
	[SerializeField] float initialStamina;
	[SerializeField] float initialStaminaUpdateSpeed;
	[SerializeField] int initialArrows;
	[SerializeField] int initialItem;
	[SerializeField] StatFill health;
	[SerializeField] StatFill stamina;
	[SerializeField] StatValue arrows;
	[SerializeField] StatValue item;
	[SerializeField] CinemachineVirtualCamera virtualCamera;
	[SerializeField] Transform[] arrowPoints;
	[SerializeField] GameObject arrowPrefab;

	Animator animator;
	Rigidbody2D rb;
	Vector3 direction;
	Vector3 directionLook;
	int directionArrow;
	bool roll;
	float lastRollTime;
	bool fire;
	float fireButtonTime;
	bool run;

	public bool Roll {
		set {
			roll = value;
		}
	}

	public int Arrows {
		set {
			arrows.Value = value;
		}

		get {
			return arrows.Value;
		}
	}

	bool Alive {
		get {
			return health.Value > 0;
		}
	}

	bool CanRoll {
		get {
			return stamina.Value > 0 && !roll;
		}
	}

	void Start () {
		directionLook = Vector3Int.down;
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		health.Initialize(initialHealth);
		stamina.Initialize(initialStamina);
		arrows.Initialize(initialArrows);
		item.Initialize(initialItem);
		virtualCamera.m_Lens.OrthographicSize = zoomSize;
		InvokeRepeating("UpdateStamina", 0, initialStaminaUpdateSpeed);
	}

	void FixedUpdate() {
		rb.velocity = Vector2.ClampMagnitude(direction, 1f) * (roll ? rollSpeed : (run ? runSpeed : walkSpeed)) * (Alive ? 1 : 0) * (fire ? 0 : 1);
	}

	void Update () {
		if (Alive) {
			if (Input.GetButtonDown("Run"))
				run = true;

			if (Input.GetButtonUp("Run"))
				run = false;

			if (Input.GetButtonDown("Item")  && item.Value > 0) {
				item.Value -= 1;
				health.Value += 3;
			}
				

			if (!roll) {
				Vector3 newDirection = Vector3.zero;

				if (Input.GetAxis("Horizontal") < 0)
					newDirection += Vector3Int.left;

				if (Input.GetAxis("Horizontal") > 0)
					newDirection += Vector3Int.right;

				if (Input.GetAxis("Vertical") > 0)
					newDirection += Vector3.up;

				if (Input.GetAxis("Vertical") < 0)
					newDirection += Vector3.down;

				if (newDirection.magnitude > 0)
					directionLook = newDirection;

				UpdateDirectionArrow(directionLook);

				if (Input.GetButtonDown("Roll") && newDirection.magnitude > 0  && CanRoll) {
					stamina.Value -= 1;
					lastRollTime = Time.unscaledTime;
					animator.SetTrigger("roll");
				}

				if (Input.GetButtonDown("Fire") && arrows.Value > 0) {
					fire = true;
					fireButtonTime = Time.unscaledTime;
				}

				if (Input.GetButtonUp("Fire")) {
					if (fire) {
						fire = false;
						arrows.Value -= 1;
						GameObject obj = Instantiate(arrowPrefab, arrowPoints[directionArrow].position, arrowPoints[directionArrow].rotation);
						Rigidbody2D objRb = obj.GetComponent<Rigidbody2D>();
						objRb.velocity = Vector2.ClampMagnitude(directionLook, 1f) * arrowSpeed;

						if (fireButtonTime + 1 > Time.unscaledTime) {
							objRb.drag = arrowDragMax - (Mathf.Pow(arrowDragMax, Mathf.Pow(Time.unscaledTime - fireButtonTime, (1/arrowDragSpeed)))); 
						}
					}
				}

				if (Input.GetButtonDown("R1"))
					health.Value += 1;

				if (Input.GetButtonDown("L1"))
					health.Value -= 1;
			
				animator.SetInteger("x", (int)directionLook.x);
				animator.SetInteger("y", (int)directionLook.y);

				virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, fire ? zoomInSize : zoomSize, fire ? Time.deltaTime * zoomInSpeed : Time.deltaTime * zoomOutSpeed);
				direction = newDirection;

				if (fire) {
					ActivateLayer("Fire");
				} else {
					if (newDirection.magnitude > 0) {
						if (run) {
							ActivateLayer("Run");
						} else {
							ActivateLayer("Walk");
						}
					} else {
						ActivateLayer("Idle");
					}
				}
			}
		} else {
			animator.SetBool("alive", false); 
			ActivateLayer("State");
		}
	}

	void UpdateStamina() {
		if (Alive && (lastRollTime + rollTime + initialStaminaUpdateSpeed < Time.unscaledTime))
			stamina.Value += 1;
	}

	void UpdateDirectionArrow(Vector3 newDirection) {
		if (newDirection.x == 0 && newDirection.y == 1) {
			directionArrow = 0;
		} else if (newDirection.x == 1 && newDirection.y == 1) {
			directionArrow = 1;
		} else if (newDirection.x == 1 && newDirection.y == 0) {
			directionArrow = 2;
		} else if (newDirection.x == 1 && newDirection.y == -1) {
			directionArrow = 3;
		} else if (newDirection.x == -1 && newDirection.y == -1) {
			directionArrow = 5;
		} else if (newDirection.x == -1 && newDirection.y == 0) {
			directionArrow = 6;
		} else if (newDirection.x == -1 && newDirection.y == 1) {
			directionArrow = 7;
		} else {
			directionArrow = 4;
		}
  }

	void ActivateLayer(string layerName) {
		for (int i = 0; i < animator.layerCount; i++)
			animator.SetLayerWeight(i, 0);

		animator.SetLayerWeight(animator.GetLayerIndex(layerName), 1);
	}
}