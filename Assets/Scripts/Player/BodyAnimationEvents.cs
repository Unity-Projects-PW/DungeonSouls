using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAnimationEvents : MonoBehaviour {



	private PlayerController playerController;

	// Use this for initialization
	void Start () {
		playerController = GetComponentInParent<PlayerController>();
	}
	
	void SlidingBoolean()
	{
		if (playerController.canSlide == false) playerController.canSlide = true;
		else { playerController.canSlide = false; }

		if (playerController.sliding == false) playerController.sliding = true;
		else { playerController.sliding = false; }
	}
}
