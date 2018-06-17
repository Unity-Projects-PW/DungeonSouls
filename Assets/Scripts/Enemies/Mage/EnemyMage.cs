using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMage : MonoBehaviour
{

	private Animator anim;

	private Transform body;
	private Transform hands;

	public Transform target;

	public GameObject fireBall;
	public GameObject fireBallParticles;
	public GameObject castingFireBallParticles;
	public float fireBallSpeed;

	public float currentDistanceToPlayer = 10f;
	//current distance from enemy to player
	public float maxDistanceToPlayer = 5f;
	// How close to player can enemy walk.

	public bool isAttacking = false;

	private bool isRunning = false;
	// stops coroutine to cast multiple times, just one fireball at once, every 2.2s

	// Use this for initialization
	void Start()
	{
		anim = GetComponent<Animator> ();
		body = transform.Find ("Body");
		hands = transform.Find ("Hands");
	}
	
	// Update is called once per frame
	void Update()
	{
		if( target )
		{
			currentDistanceToPlayer = Vector3.Distance (transform.position, target.position);
			if( currentDistanceToPlayer > 12f )
			{
				anim.SetBool ("Attacking", false);
				target = null;
			}

			if( target )
			{
				HandsMovement ();
				EnemyFacing ();
				Attack ();
			}
		} 

	}

	void Attack()
	{
		if( target )
		{
			anim.SetBool ("Attacking", true);
			if( !isRunning )
				StartCoroutine (StartCastingFireBall ());
		} else
		{
			anim.SetBool ("Attacking", false);
		}
	}

	void ThrowFireBall()
	{
		if( target )
		{
			
			var dir = target.position - transform.position;
			dir.z = 0.0f;
			dir.Normalize ();

			GameObject castedFireBall = Instantiate (fireBall, transform.position - new Vector3(0f, 0.2f, 0f), fireBall.transform.rotation) as GameObject;
			castedFireBall.GetComponent<Rigidbody2D> ().AddForce (dir * fireBallSpeed);

			var dir2 = transform.position - target.position;
			Quaternion rotation = Quaternion.LookRotation (dir2);

			GameObject castedFireBallParticle = Instantiate (fireBallParticles, castedFireBall.transform.position, rotation) as GameObject;
			castedFireBallParticle.transform.SetParent (castedFireBall.transform);

			StartCoroutine (DestroyFireBall (castedFireBall));

		}

	}

	IEnumerator StartCastingFireBall()
	{
		isRunning = true;
		while (target)
		{
			Instantiate (castingFireBallParticles, transform.position - new Vector3(0f, 0.2f, 0f), transform.rotation);
			Invoke ("ThrowFireBall", 2.2f);
			yield return new WaitForSeconds(2.2f);
		} 

		isRunning = false;
	}

	IEnumerator DestroyFireBall(GameObject fb)
	{
		while (fb)
		{
			yield return new WaitForSeconds(5f);
			Destroy (fb);
		}
	}

	void EnemyFacing()
	{
		Vector3 temp = body.localScale;

		if( target.position.x > transform.position.x )
		{
			temp.x = -1;
			body.localScale = temp;
		} else if( target.position.x < transform.position.x )
		{
			temp.x = 1;
			body.localScale = temp;
		}

	}

	void HandsMovement()
	{
		
		Vector3 difference = target.position - hands.position;
		difference.Normalize ();					// Normalize the vector. Meaning that all the sum of the vector will be equal to 1.

		float rotZ = Mathf.Atan2 (difference.y, difference.x) * Mathf.Rad2Deg;  //find the angle in degrees

		Quaternion newHandsRotation = Quaternion.Euler (0f, 0f, rotZ - 90); 

		hands.rotation = Quaternion.RotateTowards (hands.rotation, newHandsRotation, Time.deltaTime * 1200f); // better to use RotateTowards than assigning rotation value direct to transform,
		// its smooth and rotates instead of jumping between rotations;
	}


}
