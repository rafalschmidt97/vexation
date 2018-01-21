/*
* Copyright (c) Rafał Schmidt
* http://rafalschmidt.com
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stat : MonoBehaviour {
	Image sprite;
	float value;
	float maxValue;

	public float Value { 
		get {
			return value;
		}
		
		set {
			if (value > maxValue) {
				this.value = maxValue;
			} else if (value < 0) {
				this.value = 0;
			} else {
				this.value = value;
			}
		}
	}

	public float MaxValue { 
		get {
			return maxValue;
		}

		set {
			maxValue = value;
		}
	}

	public void Initialize(float initialValue) {
		value = initialValue;
		maxValue = initialValue;
	}

	void Start () {
		sprite = GetComponent<Image>();
	}

	void Update () {
		sprite.fillAmount = Value / MaxValue;
	}
}
