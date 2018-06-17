using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
	private Door door;

	static private bool isGCNear;

	//private GameObject goldenChest;
	public Sprite closedGCSprite;
	public Sprite openedGCSprite;

	public GameObject goldenKey;

	public Image keyImage;
	public Text UI_text;

	// Use this for initialization
	void Start()
	{
		door = GameObject.Find("Door").GetComponent<Door>();
	}
	
	// Update is called once per frame
	void Update()
	{
		if( isGCNear )
		{
			if( Input.GetKeyDown (KeyCode.E) )
			{
				Instantiate(goldenKey, transform.position, Quaternion.identity);
				GameManager.instance.playSound("chest");
				UI_text.text = "";
				UI_text.enabled = false;
				this.GetComponent<SpriteRenderer> ().sprite = openedGCSprite;
				keyImage.enabled = true;
				door.gotKey = true;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D collider)
	{
		if( collider.gameObject.tag == "Player" && this.gameObject.tag == "Golden Chest")
		{
			if( !door.gotKey )
			{
				UI_text.text = "Press E to open chest.";
				UI_text.enabled = true;
				isGCNear = true;
			}
		}
	}

	void OnCollisionExit2D(Collision2D collider)
	{
		if( collider.gameObject.tag == "Player" && this.gameObject.tag == "Golden Chest" )
		{
			isGCNear = false;
			UI_text.text = "";
			UI_text.enabled = false;
		}

	}


}
