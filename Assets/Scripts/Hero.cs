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
	[SerializeField] Stat health;
	[SerializeField] Stat stamina;
	[SerializeField] CinemachineVirtualCamera virtualCamera;
	[SerializeField] Transform[] arrowPoints;
	[SerializeField] GameObject arrowPrefab;

	Animator animator;
	Rigidbody2D rb;
	Vector3 direction;
	Vector3 directionLook;
	float lastRollTime;
	bool fire;
	float fireButtonTime;
	bool run;
	bool roll;
	int arrowDirection;

	bool Alive {
		get {
			return health.Value > 0;
		}
	}

	public bool Roll {
		set {
			roll = value;
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
		virtualCamera.m_Lens.OrthographicSize = zoomSize;
		InvokeRepeating("UpdateStamina", 0, initialStaminaUpdateSpeed);
	}

	void FixedUpdate() {
		rb.velocity = Vector2.ClampMagnitude(direction, 1f) * (roll ? rollSpeed : (run ? runSpeed : walkSpeed)) * (Alive ? 1 : 0) * (fire ? 0 : 1);
	}

	void Update () {
		if (Alive) {
			if (Input.GetButtonDown("Cancel"))
				run = true;

			if (Input.GetButtonUp("Cancel"))
				run = false;

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

				UpdateArrowDirection(directionLook);

				if (Input.GetButtonDown("Roll") && newDirection.magnitude > 0  && CanRoll) {
					stamina.Value -= 1;
					lastRollTime = Time.unscaledTime;
					animator.SetTrigger("roll");
				}

				if (Input.GetButtonDown("Fire")) {
					fire = true;
					fireButtonTime = Time.unscaledTime;
				}

				if (Input.GetButtonUp("Fire")) {
					if (fire) {
						fire = false;
						Rigidbody2D obj = Instantiate(arrowPrefab, arrowPoints[arrowDirection].position, arrowPoints[arrowDirection].rotation).GetComponent<Rigidbody2D>();
						obj.velocity = Vector2.ClampMagnitude(directionLook, 1f) * arrowSpeed;

						if (fireButtonTime + 1 > Time.unscaledTime) {
							obj.drag = arrowDragMax - (Mathf.Pow(arrowDragMax, Mathf.Pow(Time.unscaledTime - fireButtonTime, (1/arrowDragSpeed)))); 
						}
					}
				}

				if (Input.GetButtonDown("R1"))
					health.Value += 1;

				if (Input.GetButtonDown("L1"))
					health.Value -= 1;
			
				animator.SetInteger("x", (int)newDirection.x);
				animator.SetInteger("y", (int)newDirection.y);

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

	void UpdateArrowDirection(Vector3 newDirection) {
		if (newDirection.x == 0 && newDirection.y == 1) {
			arrowDirection = 0;
		} else if (newDirection.x == 1 && newDirection.y == 1) {
			arrowDirection = 1;
		} else if (newDirection.x == 1 && newDirection.y == 0) {
			arrowDirection = 2;
		} else if (newDirection.x == 1 && newDirection.y == -1) {
			arrowDirection = 3;
		} else if (newDirection.x == -1 && newDirection.y == -1) {
			arrowDirection = 5;
		} else if (newDirection.x == -1 && newDirection.y == 0) {
			arrowDirection = 6;
		} else if (newDirection.x == -1 && newDirection.y == 1) {
			arrowDirection = 7;
		} else {
			arrowDirection = 4;
		}
  }

	void ActivateLayer(string layerName) {
		for (int i = 0; i < animator.layerCount; i++)
			animator.SetLayerWeight(i, 0);

		animator.SetLayerWeight(animator.GetLayerIndex(layerName), 1);
	}
}