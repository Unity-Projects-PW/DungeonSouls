using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

	public static CameraShake instance;

	public Camera mainCam;
	private float shakeAmount = 0;
	public float shakeAmt = 0.1f;
	public float shakeLength = 0.1f;

	void Awake()
	{
		if (instance == null)
			instance = this;

		if( mainCam == null )
			mainCam = Camera.main;
	}

	void Update()
	{
		if( Input.GetKeyDown(KeyCode.T) )
		{
			Shake ();
		}
	}

	public void Shake()
	{
		shakeAmount = shakeAmt;
		InvokeRepeating ("BeginShake", 0f, 0.01f);
		Invoke ("StopShake", shakeLength);
	}
		
	void BeginShake()
	{
		if( shakeAmount > 0 )
		{
			Vector3 camPos = mainCam.transform.position;

			float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
			float offsetY = Random.value * shakeAmount * 2 - shakeAmount;

			camPos.x += offsetX;
			camPos.y += offsetY;

			mainCam.transform.position = camPos;
		}
	}

	void StopShake()
	{
		CancelInvoke ("BeginShake");
		mainCam.transform.localPosition = Vector3.zero;
	}
}
