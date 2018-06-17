using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireElementalStats : MonoBehaviour {

	//private WeaponController weaponController;
	//private FireElementalAI fireEleAI;
	//private Animator enemyBodyAnim;

	public GameObject damageNumber;
	private string missText = "miss";

	public int health = 75;
	public int damage = 50;

	public GameObject bloodParticle;

	public bool canHit = true;


	void Start()
	{
		//weaponController = FindObjectOfType<WeaponController>();
		//fireEleAI = GetComponent<FireElementalAI>();
		//enemyBodyAnim = GetComponent<Animator>();
	}

	void DamageEnemy(int damage)
	{
		InstantiateDamageNumber(0);
	}

	void OnTriggerEnter2D (Collider2D collider)
	{
		if (collider.tag == "PlayerWeapon")
		{
			if(canHit) DamageEnemy(0);
			//if(fireEleAI.target == null) fireEleAI.target = collider.transform.parent;
		}
	}

	void CanHit()
	{
		canHit = true;
	}

	void InstantiateDamageNumber(int damage)
	{
		GameObject dmg = Instantiate (damageNumber, transform.position + new Vector3(0f,1.5f,0f), Quaternion.Euler(Vector3.zero)) as GameObject;
		dmg.GetComponent<FloatingNumbers>().displayNumber.text = missText;
		Destroy(dmg, 1.5f);
	}

}
