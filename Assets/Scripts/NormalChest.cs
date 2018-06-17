using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalChest : MonoBehaviour
{

	private bool isPlayerNear;
	private bool isOpened;

	//private GameObject goldenChest;
	public Sprite closedChestSprite;
	public Sprite openedChestSprite;

	public GameObject item;

	public Text UI_text;

	private Animator anim;

	// Use this for initialization
	void Start()
	{
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update()
	{
		if( isPlayerNear )
		{
			if( Input.GetKeyDown (KeyCode.E) && !isOpened)
			{
				anim.SetTrigger("Open");
				GameManager.instance.playSound("chest");
				UI_text.text = "";
				UI_text.enabled = false;
				GetComponent<SpriteRenderer> ().sprite = openedChestSprite;
				isOpened = true;
				PlayerStats.instance.potions++;
				PlayerStats.instance.UpdatePotionStatus();
			}
		}
	}

	void OnCollisionEnter2D(Collision2D collider)
	{
		if( collider.gameObject.tag == "Player" && this.gameObject.tag == "Normal Chest" && !isOpened)
		{
			isPlayerNear = true;
			UI_text.text = "Press E to open chest.";
			UI_text.enabled = true;
		}
	}

	void OnCollisionExit2D(Collision2D collider)
	{
		if( collider.gameObject.tag == "Player" && this.gameObject.tag == "Normal Chest" )
		{
			isPlayerNear = false;
			UI_text.text = "";
			UI_text.enabled = false;
		}

	}


}
