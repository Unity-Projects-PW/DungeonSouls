using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour {

	private SkeletonAI skeletonAI;
	private EnemyMage enemyMage;
	private FireElementalAI fireEleAI;
	private BossBehaviour bossBehaviour;
	private BossStats bossStats;

	public RectTransform bossUI;

	void Start()
	{
		fireEleAI = GetComponentInParent<FireElementalAI>();
		skeletonAI = GetComponentInParent<SkeletonAI>();
		enemyMage = GetComponentInParent<EnemyMage>();
		bossBehaviour = GetComponentInParent<BossBehaviour>();
	}

	void OnTriggerEnter2D (Collider2D trigger)
	{
		if (trigger.tag == "Player")
		{
			if(skeletonAI != null) 
			{
				skeletonAI.target = trigger.transform;
			}
			else if (enemyMage != null) 
			{
				enemyMage.target = trigger.transform;
			}
			else if(fireEleAI != null)
			{
				fireEleAI.target = trigger.transform;
			}
			else if(bossBehaviour != null)
			{
				bossBehaviour.target = trigger.transform;
				bossUI.gameObject.SetActive(true);
			}
		}
	}
}
