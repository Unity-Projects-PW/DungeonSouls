using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{

	public static PlayerController instance = null;

	private PlayerStats playerStats;

	public GameObject slideParticle;

	private Vector2 mousePosition;
	private Animator anim;
	[HideInInspector]
	public Transform hands;
	[HideInInspector]
	public Transform body;
	private SpriteRenderer rightHand, leftHand;

	public GameObject weapon;
	public float moveSpeed;
	public static Rigidbody2D myBody;
	public bool charging = false;

	public float dashForce = 100f;
	public float maxSpeed = 1f;

	public bool canSlide = true;
	public bool sliding = false;
	public bool drinking = false;

	private int invokesTimes = 0;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	// Use this for initialization
	void Start()
	{
		playerStats = GetComponent<PlayerStats>();

		myBody = GetComponent<Rigidbody2D> ();
		anim = GetComponentInChildren<Animator> ();

		body = transform.Find ("Body");
		hands =	transform.Find ("Hands");
		rightHand = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
		leftHand = transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		HandsOrderLayer();
	}

	void Update()
	{

		mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if (charging == false && sliding == false)
		{
			Move();
		}

		if (charging == false)
		{
			if (!drinking)
			{
				PlayerFacing();
				HandsMovement();
			}	
		}
	}

	void Move()
	{
		float moveX = Input.GetAxisRaw ("Horizontal");
		float moveY = Input.GetAxisRaw ("Vertical");

		if( moveX > 0 || moveX < 0 || moveY > 0 || moveY < 0 )
		{
			anim.SetBool ("Run", true);
			//transform.Translate (moveX * moveSpeed * Time.deltaTime, moveY * moveSpeed * Time.deltaTime, 0f);
			myBody.velocity = (new Vector2(moveX, moveY) * moveSpeed);
		} 

		if( moveX == 0 && moveY == 0 )
		{
			anim.SetBool ("Run", false);
		}

		if( Input.GetKeyDown (KeyCode.Space) )
		{
			if( canSlide && playerStats.currentStamina >= playerStats.staminaNeededForDash)
			{
				float n_dashForce = dashForce;

				if (moveX != 0 || moveY != 0)
				{
					if( moveX != 0 && moveY != 0 )
					{
						n_dashForce = dashForce / 1.5f;
					}

					sliding = true;
					myBody.velocity = (new Vector2(moveX, moveY) * n_dashForce);
					GameManager.instance.playSound("jump");
					playerStats.currentStamina -= 15f;
					playerStats.usingStamina = true;
					playerStats.Invoke("ResetStaminaState", 0.8f);
					anim.SetTrigger ("Dash");
					invokesTimes = 0;
					InvokeRepeating("SlideParticles", 0.0001f, 0.02f);

					canSlide = false;
					Invoke("CanSlide", 0.2f);
				}
			}
		}
			
	}

	private void CanSlide()
	{
		canSlide = true;
		sliding = false;
	}

	private void SlideParticles()
	{
		invokesTimes ++;
		Instantiate (slideParticle, transform.position, Quaternion.Euler(0f,-90f,90f));

		if (invokesTimes >= 13) CancelInvoke("SlideParticles");
	}

	void PlayerFacing()
	{
		Vector3 temp = body.localScale;

		if( mousePosition.x > transform.position.x )
		{
			temp.x = 1;
			body.localScale = temp;
		} else if( mousePosition.x < transform.position.x )
		{
			temp.x = -1;
			body.localScale = temp;
		}
	}

	void HandsMovement()
	{
		Vector3 difference = Camera.main.ScreenToWorldPoint (Input.mousePosition) - hands.position;
		difference.Normalize ();					// Normalize the vector. Meaning that all the sum of the vector will be equal to 1.

		float rotZ = Mathf.Atan2 (difference.y, difference.x) * Mathf.Rad2Deg;  //find the angle in degrees

		Quaternion newHandsRotation = Quaternion.Euler (0f, 0f, rotZ - 90); 

		hands.rotation = Quaternion.RotateTowards (hands.rotation, newHandsRotation, Time.deltaTime * 1200f); // better to use RotateTowards than assigning rotation value direct to transform,
		// its smooth and rotates instead of jumping between rotations;
	}

	void HandsOrderLayer()
	{
		if ( hands.eulerAngles.z > 35 & hands.eulerAngles.z < 180 )
		{
			rightHand.sortingOrder = 6;
			weapon.GetComponent<Renderer> ().sortingOrder = 5;
		} else
		{
			rightHand.sortingOrder = 9;
			weapon.GetComponent<Renderer> ().sortingOrder = 8;
		}

		if( hands.eulerAngles.z > 180 & hands.eulerAngles.z < 315 )
		{
			leftHand.sortingOrder = 6;
		} else
		{
			leftHand.sortingOrder = 9;
		}
	}
		
}
