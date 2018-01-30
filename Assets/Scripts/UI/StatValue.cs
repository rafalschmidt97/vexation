/*
* Copyright (c) Rafał Schmidt
* http://rafalschmidt.com
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatValue : MonoBehaviour {
	Text text;
	int value;

	public int Value { 
		get {
			return value;
		}
		
		set {
			if (value < 0) {
				this.value = 0;
			} else {
				this.value = value;
			}
		}
	}

	public void Initialize(int initialValue) {
		value = initialValue;
	}

	void Start () {
		text = GetComponent<Text>();
	}

	void Update () {
		text.text = Value.ToString();
	}
}
