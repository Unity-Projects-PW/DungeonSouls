using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
	[HideInInspector]
	public bool gotKey;
	private bool isDoorNear;

	private Animator anim;

	public Image keyImage;
	public Text UI_text;

	private bool opened;

	// Use this for initialization
	void Start()
	{
		if (GameManager.instance.isLoaded)
		{
			if (GameManager.instance.door == false)
			{
				Destroy(this.gameObject);
			}
		}

		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		if( isDoorNear && gotKey )
		{
			if( Input.GetKeyDown (KeyCode.E) )
			{
				opened = true;
				//				GameMaster.instance.playSound("chest");
				//				UI_text.text = "";
				//				UI_text.enabled = false;
				//				this.GetComponent<SpriteRenderer> ().sprite = openedGCSprite;

				UI_text.text = "The Door remain open after death...";
				UI_text.enabled = true;

				keyImage.enabled = false;
				GameManager.instance.playSound("chest");
				GameManager.instance.door = false;
				anim.Play("OpenDoor");
				GetComponent<BoxCollider2D>().enabled = false;
				Invoke("DisableTip", 3f);
			}
		}
			
	}

	void DisableTip()
	{
		UI_text.text = "";
		UI_text.enabled = false;
	}

	void OnCollisionEnter2D(Collision2D collider)
	{
		if( collider.gameObject.tag == "Player" && this.gameObject.tag == "Door" && opened == false)
		{
			if( !gotKey )
			{
				isDoorNear = true;
				UI_text.text = "Door locked. I need to find a key.";
				UI_text.enabled = true;
			}

			if( gotKey )
			{
				isDoorNear = true;
				UI_text.text = "Press E to unlock the door.";
				UI_text.enabled = true;
			}
		}
	}

	void OnCollisionExit2D(Collision2D collider)
	{
		if( collider.gameObject.tag == "Player" && this.gameObject.tag == "Door" && opened == false)
		{
			isDoorNear = false;
			UI_text.text = "";
			UI_text.enabled = false;
		}
	}

	public void Destroy()
	{
		Destroy(this.gameObject);
	}

}
