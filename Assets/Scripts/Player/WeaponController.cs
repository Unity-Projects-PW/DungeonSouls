using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{

	private PlayerStats playerStats;

	private Animator anim;
	public float force = 200f;
	public GameObject weapon;
	private Collider2D weaponCollider;

	private float staminaReq;

	[HideInInspector]
	public bool isStrongAttack = false;

	// Use this for initialization
	void Start()
	{

		playerStats = GetComponentInParent<PlayerStats> ();

		weaponCollider = weapon.GetComponent<Collider2D> ();
		anim = GetComponent<Animator> ();

	}
	
	// Update is called once per frame
	void Update()
	{
		SwordAttack ();
	}

	void SwordAttack()
	{
		if( Input.GetMouseButtonDown (0) )
		{
			if( playerStats.currentStamina >= playerStats.Attack1StaminaReq && !GameMaster.instance.isPaused 
																			&& !PlayerController.instance.drinking)
			{
				playerStats.weaponDamage = playerStats.currentDamage;
				isStrongAttack = false;
				staminaReq = playerStats.Attack1StaminaReq;
				anim.SetTrigger ("SwordAttack1");
			}
		}	

		if( Input.GetMouseButtonDown (1) )
		{
			if( playerStats.currentStamina >= playerStats.Attack2StaminaReq && !GameMaster.instance.isPaused 
																			&& !PlayerController.instance.drinking)
			{
				playerStats.weaponDamage = playerStats.currentDamage * 2;
				isStrongAttack = true;
				staminaReq = playerStats.Attack2StaminaReq;
				anim.SetTrigger ("SwordAttack2");
			}
		}

		if (Input.GetKeyDown(KeyCode.Q))
		{
			if (!GameMaster.instance.isPaused && !PlayerController.instance.drinking && playerStats.potions > 0)
			{
				playerStats.potions--;
				StartCoroutine(playerStats.RegenHealth());
				PlayerController.instance.body.transform.localScale = new Vector3(1f, 1f, 1f);
				PlayerController.instance.hands.transform.rotation = Quaternion.Euler(Vector3.zero);
				PlayerController.instance.drinking = true;
				PlayerController.instance.moveSpeed = 1.5f;
				anim.SetTrigger("potion");
			}
		}
	}

	void Drinking()
	{

	}

	void ChangeDrinkingState()
	{
		PlayerController.instance.moveSpeed = 4.5f;
		PlayerController.instance.drinking = false;
		PlayerController.instance.charging = false;
	}

	void StaminaUpdate()
	{
		playerStats.currentStamina -= staminaReq;
		playerStats.usingStamina = true;
		playerStats.Invoke ("ResetStaminaState", 0.8f);
	}

	void Charge()
	{
		var mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		var mouseDirection = mousePosition - transform.root.position;
		mouseDirection.z = 0.0f;
		mouseDirection.Normalize ();

		PlayerController.myBody.AddForce (mouseDirection * force);
	}

	void changeChargingState()
	{
		if( PlayerController.instance.charging == false )
		{
			PlayerController.instance.charging = true;
		} else
		{
			PlayerController.instance.charging = false;
		}
	}

	void EnableOrDisableWeaponCollider()
	{
		weaponCollider.enabled = !weaponCollider.enabled;
	}



}
