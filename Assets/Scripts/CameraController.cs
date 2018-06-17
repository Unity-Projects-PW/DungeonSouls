using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private GameObject target;
	public float cameraSpeed;
	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = new Vector3 (0f,0f,-1f);
		target = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.position = Vector3.Lerp(transform.position, target.transform.position + offset, cameraSpeed * Time.deltaTime);	
	//	transform.position = target.transform.position + offset;
	}
}
