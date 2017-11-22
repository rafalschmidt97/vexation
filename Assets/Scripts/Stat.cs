/*
* Copyright (c) Rafał Schmidt
* http://rafalschmidt.com
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stat : MonoBehaviour {

	private Image sprite;
	private float value;
	private float maxValue;

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

	void Start () {
		sprite = GetComponent<Image>();
	}

	void Update () {
		sprite.fillAmount = value / maxValue;
	}

	public void Initialize(float initialValue) {
		value = initialValue;
		maxValue = initialValue;
	}
}
