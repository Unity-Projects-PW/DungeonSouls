using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class BossStats : MonoBehaviour {

	[HideInInspector]
	public AudioClip hit, died;

	public GameObject damageNumber;

	private WeaponController weaponController;
	private PlayerStats playerStats;
	private BossBehaviour bossBehaviour;
	private Animator anim;

	public float currentHealth;
	public float maxHealth = 2000f;

	public Image healthBar;

	private GameObject parent;
	private GameObject handsgameObject;

	public GameObject bossDeathParticle;

	public bool canHit = true;


	void Start()
	{
		bossBehaviour = GetComponent<BossBehaviour>();
		weaponController = FindObjectOfType<WeaponController>();
		playerStats = FindObjectOfType<PlayerStats>();
		anim = GetComponent<Animator>();

		currentHealth = maxHealth;
	}

	void FixedUpdate()
	{		
		if (bossBehaviour.target) 
		{
			healthBar.fillAmount = currentHealth / maxHealth;
		}
	}

	void DamageEnemy(int damage)
	{
		canHit = false;
		currentHealth = currentHealth - damage;
		CameraShake.instance.Shake();
		InstantiateDamageNumber(damage);
		//Instantiate(bossParticle, transform.position, transform.rotation);
		anim.SetTrigger("Damaged");
		Invoke("CanHit", 0.3f);

		if (weaponController.isStrongAttack == false) GameManager.instance.playSound("hit");
		else { GameManager.instance.playSound("stronghit"); }

		if (currentHealth <= 0)
		{
			BossDeath();
		}
	}

	void BossDeath()
	{
		Destroy(transform.Find("Body").gameObject);
		Instantiate(bossDeathParticle, transform.position, transform.rotation);
		GameManager.instance.playSound ("boom");
		StartCoroutine("deadSound");
		Destroy(GetComponent<BoxCollider2D>());
		bossBehaviour.target = null;
		Destroy(bossBehaviour.skills.gameObject);
		Destroy(bossBehaviour);
		healthBar.fillAmount = 0;
		bossBehaviour.bossBlockade.SetActive(false);
		Time.timeScale = 0.5f;
		Invoke("RollCredits", 2.5f);
	}

	void RollCredits()
	{
		GameManager.instance.Credits();
	}

	void OnTriggerEnter2D (Collider2D collider)
	{
		if (collider.tag == "PlayerWeapon")
		{
			if(canHit) DamageEnemy(playerStats.weaponDamage);
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
