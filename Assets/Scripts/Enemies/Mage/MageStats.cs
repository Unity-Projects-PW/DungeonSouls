using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MageStats : MonoBehaviour {

	public GameObject damageNumber;

	private WeaponController weaponController;
	private PlayerStats playerStats;
	private EnemyMage enemyMage;
	private Animator enemyBodyAnim;

	public int health = 150;
	public int damage = 30;

	public GameObject bloodParticle;

	public bool canHit = true;


	void Start()
	{
		weaponController = FindObjectOfType<WeaponController>();
		enemyBodyAnim = GetComponent<Animator>();
		playerStats = FindObjectOfType<PlayerStats>();
		enemyMage = GetComponent<EnemyMage>();
	}

	void DamageEnemy(int damage)
	{
		canHit = false;
		health = health - damage;
		CameraShake.instance.Shake();
		InstantiateDamageNumber(damage);
		Instantiate(bloodParticle, transform.position, transform.rotation);
		enemyBodyAnim.SetTrigger("Damaged");
		Invoke("CanHit", 0.3f);

		if (weaponController.isStrongAttack == false) GameManager.instance.playSound("hit");
		else { GameManager.instance.playSound("stronghit"); }

		if (health <= 0)
		{
			enemyBodyAnim.SetTrigger("Died");
			StartCoroutine("deadSound");
			Destroy(GetComponent<Collider2D>());
			Destroy(GetComponent<Rigidbody2D>());
			Destroy(GetComponent<EnemyMage>());
			Destroy(GetComponent<MageStats>());
			GameMaster.instance.UpdateExpBar(100f);

		}
	}

	void OnTriggerEnter2D (Collider2D collider)
	{
		if (collider.tag == "PlayerWeapon")
		{
			if(canHit) DamageEnemy(playerStats.weaponDamage);
			if(enemyMage.target == null) enemyMage.target = collider.transform.parent;
		}
	}

	void CanHit()
	{
		canHit = true;
	}

	void InstantiateDamageNumber(int damage)
	{
		GameObject dmg = Instantiate (damageNumber, transform.position + new Vector3(0f,1.5f,0f), Quaternion.Euler(Vector3.zero)) as GameObject;
		dmg.GetComponent<FloatingNumbers>().DamageNumber = damage;
		Destroy(dmg, 1.5f);
	}

	IEnumerator deadSound()
	{
		yield return new WaitForSeconds(GameManager.instance.lastPlayed.length);
		GameManager.instance.playSound("enemydied");
	}
}
