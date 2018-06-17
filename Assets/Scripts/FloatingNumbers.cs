using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingNumbers : MonoBehaviour {

	public float moveSpeed;
	public float DamageNumber;
	public Text displayNumber;


	// Use this for initialization
	void Start () {
		if (DamageNumber != 0) displayNumber.text = DamageNumber.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
