using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Seeker))]

public class SkeletonAI : MonoBehaviour
{
	private Enemy enemy;
	public Transform target;

	//How many times each second we will update our path
	public float updateRate = 2f;

	private Seeker seeker;
	private Rigidbody2D rb;

	//The calculated path
	public Path path;

	//The AI's speed per second;
	public float speed = 300f;
	public ForceMode2D fmode;

	[HideInInspector]
	public bool pathIsEnded = false;

	//The max distance from the AI to a waypoing to it to continue to next waypoint
	public float nextWaypointDistance = 3f;

	//The waypoint we are currently moving towards
	private int currentWaypoint = 0;

	private bool coroutineAlreadyStarted = false;

	public float currentDistanceToPlayer = 10f;  //current distance from enemy to player
	public float maxDistanceToPlayer = 1.2f; // How close to player can enemy walk.

	public bool isAttacking = false;

	// Use this for initialization
	void Start()
	{
		enemy = GetComponent<Enemy>();

		seeker = GetComponent<Seeker> ();
		rb = GetComponent<Rigidbody2D> ();

		if( target == null )
		{
			//Debug.Log ("No player found? Lets wait for one");
			return;
		} 
		
	}
		
	// Update is called once per frame
	void FixedUpdate()
	{

		if( target && !coroutineAlreadyStarted )
		{
			StartCoroutine (UpdatePath ());
			coroutineAlreadyStarted = true;
		}

		if( path == null )
		{
			return;
		}

		if( currentWaypoint >= path.vectorPath.Count )
		{
			if( pathIsEnded )
			{
				return;
			}

			Debug.Log ("End of path reached.");
			pathIsEnded = true;
			return;
		}
		pathIsEnded = false;
	
		currentDistanceToPlayer = Vector3.Distance (transform.position, target.position);
		if( currentDistanceToPlayer > maxDistanceToPlayer && !enemy.isAttacking && isAttacking == false)
		{
			ChasePlayer ();
		}
			

		if ((currentDistanceToPlayer < maxDistanceToPlayer) && (isAttacking == false))
		{
			isAttacking = true;
			enemy.AttackPlayer();
		} else if (currentDistanceToPlayer < 5f && currentDistanceToPlayer > maxDistanceToPlayer && isAttacking == false && enemy.attack2Viable == true)
		{
			isAttacking = true;
			enemy.Attack2Player();
		}

	}

	IEnumerator UpdatePath()
	{
		this.seeker.StartPath (transform.position, target.position, OnPathComplete);

		yield return new WaitForSeconds(1f / updateRate);
		StartCoroutine (UpdatePath ());
	}

	void ChasePlayer()
	{
		//Direction to the next waypoint
		Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
		dir *= speed * Time.fixedDeltaTime;

		//Move the AI
		rb.AddForce (dir, fmode);

		float distance = Vector3.Distance (transform.position, path.vectorPath[currentWaypoint]);
		if( distance < nextWaypointDistance )
		{ 
			currentWaypoint++;
		}
	}

	public void OnPathComplete(Path p)
	{
		// Debug.Log ("We found a path. Any error? " + p.error);
		if( !p.error )
		{
			path = p;
			currentWaypoint = 0;
		}
	}


		
}
