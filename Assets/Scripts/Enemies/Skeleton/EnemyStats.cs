using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyStats : MonoBehaviour {

	[HideInInspector]
	public AudioClip hit, died;

	public GameObject damageNumber;

	private WeaponController weaponController;
	private PlayerStats playerStats;
	private SkeletonAI skeletonAI;
	private Animator enemyHandsAnim;
	private Animator enemyBodyAnim;

	public int health = 100;
	public int damage = 20;

	private GameObject parent;
	private GameObject handsgameObject;

	public GameObject skeletonParticle;

	public bool canHit = true;


	void Start()
	{
		skeletonAI = GetComponentInParent<SkeletonAI>();
		weaponController = FindObjectOfType<WeaponController>();
		playerStats = FindObjectOfType<PlayerStats>();
		enemyBodyAnim = GetComponent<Animator>();

		parent = transform.parent.gameObject;
		handsgameObject = parent.transform.GetChild(1).GetChild(0).gameObject;
		enemyHandsAnim = handsgameObject.GetComponent<Animator>();

	}

	void DamageEnemy(int damage)
	{
		canHit = false;
		health = health - damage;
		CameraShake.instance.Shake();
		InstantiateDamageNumber(damage);
		Instantiate(skeletonParticle, transform.position, transform.rotation);
		enemyBodyAnim.SetTrigger("Damaged");
		Invoke("CanHit", 0.3f);

		if (weaponController.isStrongAttack == false) GameManager.instance.playSound("hit");
		else { GameManager.instance.playSound("stronghit"); }

		if (health <= 0)
		{
			enemyBodyAnim.SetTrigger("Died");
			enemyHandsAnim.SetTrigger("Died");
			StartCoroutine("deadSound");
			Destroy(GetComponentInParent<SkeletonAI>());
			Destroy(GetComponentInParent<Enemy>());
			Destroy(transform.parent.GetComponent<CapsuleCollider2D>());
			Destroy(GetComponent<CapsuleCollider2D>());
			Destroy(GetComponent<Rigidbody2D>());
			Destroy(parent.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject);
			GameMaster.instance.UpdateExpBar(50f);
		}
	}

	void OnTriggerEnter2D (Collider2D collider)
	{
		if (collider.tag == "PlayerWeapon")
		{
			if(canHit) DamageEnemy(playerStats.weaponDamage);
			if(skeletonAI.target == null) skeletonAI.target = collider.transform.parent;
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
