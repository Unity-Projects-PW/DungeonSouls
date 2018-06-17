using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireElemental : MonoBehaviour {

	private FireElementalAI fireElementalAI;

	public GameObject Boom;

	private Animator anim;

	private Transform body;
	private Transform hands;

	// Use this for initialization
	void Start () {
		fireElementalAI = GetComponent<FireElementalAI>();
		anim = GetComponent<Animator> ();
		body = transform.Find ("Body");
		hands = transform.Find ("Hands");
	}
	
	// Update is called once per frame
	void Update () {
		
		if( fireElementalAI.target == true && fireElementalAI.isAttacking == false)
		{
			HandsMovement ();
			EnemyFacing();
		}
	}

	public void BlowUp()
	{
		anim.SetTrigger("BlowingUp");
		Instantiate(Boom, transform.position, new Quaternion(0,0,0,0));
	}

	public void isAttackingToFalse()
	{
		GameManager.instance.playSound("boom");
		GameMaster.instance.UpdateExpBar(20f);
		Destroy(gameObject);
		//gameObject.SetActive(false);
		fireElementalAI.isAttacking = false;
	}

	void EnemyFacing()
	{
		Vector3 temp = body.localScale;

		if( fireElementalAI.target.position.x > transform.position.x )
		{
			temp.x = -1;
			body.localScale = temp;
		} else if( fireElementalAI.target.position.x < transform.position.x )
		{
			temp.x = 1;
			body.localScale = temp;
		}

	}

	void HandsMovement()
	{

		Vector3 difference = fireElementalAI.target.position - hands.position;
		difference.Normalize ();					// Normalize the vector. Meaning that all the sum of the vector will be equal to 1.

		float rotZ = Mathf.Atan2 (difference.y, difference.x) * Mathf.Rad2Deg;  //find the angle in degrees

		Quaternion newHandsRotation = Quaternion.Euler (0f, 0f, rotZ - 90); 

		hands.rotation = Quaternion.RotateTowards (hands.rotation, newHandsRotation, Time.deltaTime * 1200f); // better to use RotateTowards than assigning rotation value direct to transform,
		// its smooth and rotates instead of jumping between rotations;
	}


}
