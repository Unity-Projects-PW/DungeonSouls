using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	private Animator handsAnim;
	private Rigidbody2D enemyBody;

	private SkeletonAI skeletonAI;

	private Vector2 facingDirection;
	public Transform hands;
	private Transform body;
	private GameObject rightHand, leftHand;

	public GameObject weapon;

	private Transform target;

	public bool isAttacking;

	public float attack2CoolDown = 3f;
	public bool attack2Viable = false;

	public float chargeForce = 3000f;

	// Use this for initialization
	void Start()
	{
		enemyBody = GetComponent<Rigidbody2D>();

		skeletonAI = GetComponentInParent<SkeletonAI> ();

		handsAnim = transform.GetChild(1).GetChild(0).GetComponent<Animator>();

		body = transform.Find ("Body S");
		//rightHand = GameObject.Find ("Right Hand S");
		rightHand = transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
		//leftHand = GameObject.Find ("Left Hand S");
		leftHand = transform.GetChild(1).GetChild(0).GetChild(1).gameObject;
		//weapon = GameObject.Find ("Skeleton Dagger");
		weapon = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject;

	}

	void Update()
	{
		attack2CoolDown -= Time.deltaTime;
		if (attack2CoolDown <= 0f) attack2Viable = true;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if( skeletonAI.target == true && isAttacking == false)
		{
			HandsMovement ();
			EnemyFacing();
		}

		HandsOrderLayer ();
	}

	public void AttackPlayer()
	{
		handsAnim.SetTrigger("SAttack1");
	}

	public void Attack2Player()
	{
		handsAnim.SetTrigger("SAttack2");
		attack2Viable = false;
		attack2CoolDown = 4f;
	}

	public void Charge()
	{
		if( skeletonAI.target == true)
		{
			if (this != null)
			{
				var playerPosition = PlayerController.instance.transform.position;
				var playerDirection = playerPosition - transform.root.position;
				playerDirection.z = 0.0f;
				playerDirection.Normalize();

				enemyBody.AddForce(playerDirection * chargeForce);
			}
		}
	}
		
	void EnemyFacing()
	{
		Vector3 temp = body.localScale;

		if (skeletonAI.target.position.x > transform.position.x)
		{
			temp.x = -1;
			body.localScale = temp;
		} else if (skeletonAI.target.position.x < transform.position.x)
		{
			temp.x = 1;
			body.localScale = temp;
		}
	}

	void HandsMovement()
	{
		Vector3 difference = skeletonAI.target.position - hands.position;
		difference.Normalize ();					// Normalize the vector. Meaning that all the sum of the vector will be equal to 1.
	
		float rotZ = Mathf.Atan2 (difference.y, difference.x) * Mathf.Rad2Deg;  //find the angle in degrees

		Quaternion newHandsRotation = Quaternion.Euler (0f, 0f, rotZ - 90); 

		hands.rotation = Quaternion.RotateTowards (hands.rotation, newHandsRotation, Time.deltaTime * 1200f); // better to use RotateTowards than assigning rotation value direct to transform, 																										// its smooth and rotates instead of jumping between rotations;
	}

	void HandsOrderLayer()
	{
		if( hands.eulerAngles.z > 35 & hands.eulerAngles.z < 180 )
		{
			rightHand.GetComponent<Renderer> ().sortingOrder = 2;
			weapon.GetComponent<Renderer> ().sortingOrder = 1;
		} else
		{
			rightHand.GetComponent<Renderer> ().sortingOrder = 5;
			weapon.GetComponent<Renderer> ().sortingOrder = 4;
		}
	
		if( hands.eulerAngles.z > 180 & hands.eulerAngles.z < 315 )
		{
			leftHand.GetComponent<Renderer> ().sortingOrder = 5;
		} else
		{
			leftHand.GetComponent<Renderer> ().sortingOrder = 4;
		}
	}

}
