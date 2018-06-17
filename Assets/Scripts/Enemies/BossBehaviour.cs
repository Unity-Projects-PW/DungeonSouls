using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
	enum Faze
	{
		Idle,
		Bombs,
		FireBalls
	};

	private Animator anim;

	private Faze currentFaze;
	private Faze lastFaze;

	public Transform target;

	private Transform body;

	public GameObject fireBall;
	public GameObject fireBallParticles;
	public GameObject castingFireBallParticles;
	public GameObject Boom;

	private bool isThrowingFireBalls = false;
	// stops coroutine to cast multiple times, just one fireball at once, every 2.2s
	private bool isCastingBombs = false;
	private bool isResting = false;

	private int randomFaze;

	[Header ("Boss difficulty settings")]

	public float fireBallSpeed;
	public int amountOfFireBalls = 30;
	public float timeBetweenFireBallSpawn = 0.02f;

	public float timeToNextFaze = 3f;

	public int amountOfBombs = 10;
	public float timeBetweenBombSpawn = 0.5f;

	[HideInInspector]
	public Transform skills;

	[Header("Blockade")]
	public GameObject bossBlockade;
	private bool isBlocked;

	void Start()
	{ 
		skills = transform.Find("Skills");
		anim = GetComponent<Animator>();

		currentFaze = Faze.Idle;
		randomFaze = Random.Range (0, 2);

		if( randomFaze == 0 )
			currentFaze = Faze.FireBalls;
		else
		{
			currentFaze = Faze.Bombs;
		}
	
		body = transform.Find ("Body");
	}

	void Update()
	{
		if(target) BossFacing ();

		// FireBlockade so player cant leave boss fight... 
		if (target && !isBlocked)
		{
			isBlocked = true;
			bossBlockade.SetActive(true);
		}
		else if (isBlocked == true && target == null)
		{
			isBlocked = false;
			bossBlockade.SetActive(false);
		}
	}

	void FixedUpdate()
	{
		if( target )
		{
			if( !isResting && currentFaze == Faze.Idle )
				StartCoroutine (Rest ());
			else if( !isCastingBombs && currentFaze == Faze.Bombs )
				StartCoroutine (StartCastingBombs ());
			else if( !isThrowingFireBalls && currentFaze == Faze.FireBalls )
				StartCoroutine (StartCastingFireBalls ());
		}
	}

	IEnumerator Rest()
	{
		isResting = true;

		yield return new WaitForSeconds(timeToNextFaze);
		if( lastFaze == Faze.FireBalls )
		{
			currentFaze = Faze.Bombs;
		} else if( lastFaze == Faze.Bombs )
		{
			currentFaze = Faze.FireBalls;
		}
			
		isResting = false;
	}

	IEnumerator ThrowFireBalls(Vector3 randomFireBallPos)
	{
		yield return new WaitForSeconds(2.2f);

		//var dir = target.position - transform.position;
		var dir = new Vector3(Random.Range (-500, 500), Random.Range (-500, 500), 0f) - transform.position;
		dir.z = 0.0f;
		dir.Normalize ();

		GameObject castedFireBall = Instantiate (fireBall, randomFireBallPos, fireBall.transform.rotation) as GameObject;
		castedFireBall.GetComponent<Rigidbody2D> ().AddForce (dir * fireBallSpeed);
		castedFireBall.transform.SetParent (skills);

		var dir2 = transform.position - target.position;
		Quaternion rotation = Quaternion.LookRotation (dir2);

		GameObject castedFireBallParticle = Instantiate (fireBallParticles, castedFireBall.transform.position, rotation) as GameObject;
		castedFireBallParticle.transform.SetParent (castedFireBall.transform);

		StartCoroutine (DestroyFireBall (castedFireBall));
	}

	IEnumerator StartCastingFireBalls()
	{
		isThrowingFireBalls = true;

		anim.SetBool("CastBombs", true);

		while (target && isThrowingFireBalls)
		{
			for (int i = 0; i < amountOfFireBalls; i++)
			{
				float randomX = Random.Range (transform.position.x - 3f, transform.position.x + 3f);
				float randomY = Random.Range (transform.position.y - 3f, transform.position.y + 3f);

				Vector3 randomFireBallPos = new Vector3(randomX, randomY, 0f);
					
				GameObject fbp = Instantiate (castingFireBallParticles, randomFireBallPos, transform.rotation) as GameObject;
				fbp.transform.SetParent (skills);

				StartCoroutine (ThrowFireBalls (fbp.transform.position));
				yield return new WaitForSeconds(timeBetweenFireBallSpawn);
			}
				
			isThrowingFireBalls = false;
		} 
			
		lastFaze = currentFaze;
		anim.SetBool("CastBombs", false);
		currentFaze = Faze.Idle;
	}

	IEnumerator DestroyFireBall(GameObject fb)
	{
		while (fb)
		{
			yield return new WaitForSeconds(2.5f);
			Destroy (fb);
		}
	}

	IEnumerator StartCastingBombs()
	{
		isCastingBombs = true;

		anim.SetBool("CastBombs", true);

		while (target && isCastingBombs)
		{
			for (int i = 0; i < amountOfBombs; i++)
			{
				float randomX = Random.Range (target.position.x - 3f, target.position.x + 3f);
				float randomY = Random.Range (target.position.y - 3f, target.position.y + 3f);

				Vector3 randomBombsPosition = new Vector3(randomX, randomY, 0f);

				GameObject castedBomb = Instantiate (Boom, randomBombsPosition, new Quaternion(0, 0, 0, 0)) as GameObject;
				castedBomb.transform.SetParent (skills);
				StartCoroutine (PlayBoomSound (castedBomb));
				yield return new WaitForSeconds(timeBetweenBombSpawn);
			}
				
			isCastingBombs = false;
		} 

		lastFaze = currentFaze;
		anim.SetBool("CastBombs", false);
		currentFaze = Faze.Idle;

	}

	IEnumerator PlayBoomSound(GameObject bomb)
	{
		yield return new WaitForSeconds(1.9f);
		bomb.GetComponent<CircleCollider2D> ().enabled = true;
		bomb.GetComponent<SpriteRenderer> ().enabled = false;
		yield return new WaitForSeconds(0.1f);
		GameManager.instance.playSound ("boom");
		bomb.GetComponent<CircleCollider2D> ().enabled = false;
		StartCoroutine (DestroyBomb (bomb));
	}

	IEnumerator DestroyBomb(GameObject b)
	{
		while (b)
		{
			yield return new WaitForSeconds(1f);
			Destroy (b);
		}
	}

	void BossFacing()
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
}