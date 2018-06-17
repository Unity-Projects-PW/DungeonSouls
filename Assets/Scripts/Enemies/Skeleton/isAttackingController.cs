using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isAttackingController : MonoBehaviour
{

	private Enemy enemy;

	private GameObject parent;
	private EnemyStats enemyStats;

	private SkeletonAI skeletonAI;
	private Collider2D weaponCollider;

	void Start()
	{
		weaponCollider = transform.GetChild(0).GetChild(0).GetComponent<BoxCollider2D>();
		enemy = GetComponentInParent<Enemy> ();
		skeletonAI = GetComponentInParent<SkeletonAI> ();
		enemyStats = transform.parent.transform.parent.GetChild(0).GetComponent<EnemyStats>();
	}

	void changeIsAttackingState()
	{
		if( enemy.isAttacking == false )
		{
			enemy.isAttacking = true;
		} else
		{
			enemy.isAttacking = false;
		}
	}

	void changeSkeletonAIIsAttackingState()
	{
		skeletonAI.isAttacking = false;
	}

	void Charge()
	{
		enemy.Charge();
	}

	void EnableOrDisableWeaponCollider(string attackType)
	{
		if (weaponCollider)
		{
			if (attackType == "attack1") enemyStats.damage = 12;
			else if (attackType == "attack2") enemyStats.damage = 20;

			weaponCollider.enabled = !weaponCollider.enabled;
		}
	}
}
