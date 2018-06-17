using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour {

	public static PlayerStats instance;

	private MageStats mageStats;
	private FireElementalStats fireEleStats;
	private Animator playerAnim;

	public GameObject bloodParticle;

	public int currentDamage = 25;
	[HideInInspector]
	public int weaponDamage;

	public float currentHealth;
	public float maxHealth = 100f;
	public bool canHit = true;

	public float currentStamina;
	public float maxStamina = 60f;
	public float staminaRegenPerSecond = 12f;
	public float Attack1StaminaReq = 15f;
	public float Attack2StaminaReq = 25f;

	public int potions = 3;
	private Text potionText;
	[HideInInspector]
	public GameObject potText;
	[HideInInspector]
	public GameObject potImage;

	public float staminaNeededForDash;

	private Image healthBar;
	private Text healthText;
	private Image staminaBar;
	private Text staminaText;
	private Text staminaRegenText;
	public bool usingStamina = false;

	public GameObject YouDiedPanel;

	public GameObject damageNumber;

	void Start()
	{
		if( instance == null )
			instance = this;

		if (GameManager.instance.isLoaded)
		{
			maxHealth = GameManager.instance.maxHealth;
			maxStamina = GameManager.instance.maxStamina;
			staminaRegenPerSecond = GameManager.instance.staminaRegenPerSecond;
			currentDamage = GameManager.instance.currentDamage;
		}

		playerAnim = transform.GetChild(0).GetComponent<Animator>();
		mageStats = FindObjectOfType<MageStats>();
		fireEleStats = FindObjectOfType<FireElementalStats>();


		healthBar = GameObject.Find("PlayerHealthBar").GetComponent<Image>();
		healthText = GameObject.Find("PlayerHealthText").GetComponent<Text>();
		staminaBar = GameObject.Find("PlayerStaminaBar").GetComponent<Image>();
		staminaText = GameObject.Find("PlayerStaminaText").GetComponent<Text>();
		staminaRegenText = GameObject.Find("StaminaRegenText").GetComponent<Text>();

		potionText = GameObject.Find("PotionText").GetComponent<Text>();
		potText = GameObject.Find("PotionText");
		potImage = GameObject.Find("PotionImage");
		potionText.text = potions.ToString();

		currentHealth = maxHealth;
		healthText.text = currentHealth.ToString()+ "/" + maxHealth.ToString();
		currentStamina = maxStamina;
		staminaText.text = currentStamina.ToString() + "/" + maxStamina.ToString();
	}

	void Update()
	{
		PlayerBarsUpdate();
		RegenStamina();
	}

	void DamagePlayer(int damage)
	{
		canHit = false;
		currentHealth -= damage;
		InstantiateDamageNumber(damage);
		GameManager.instance.playSound("hit");
		CameraShake.instance.Shake();
		playerAnim.SetTrigger("Damaged");
		Instantiate(bloodParticle, transform.position, transform.rotation);
		if (currentHealth <= 0)
		{
			playerAnim.SetBool("Dead", true);
			Time.timeScale = 0.1f;
			YouDiedPanel.SetActive(true);
			GameManager.instance.playSound("youdied");
			GameManager.instance.musicSource.Stop();
			Destroy(GetComponent<CapsuleCollider2D>());
			Destroy(GetComponent<PlayerController>());
			Destroy(transform.GetChild(1).gameObject);
			GameManager.instance.AssignVariablesAndSaveTheGame();
			Invoke("ReloadCurrentLevel", 1f);
		}
		Invoke("CanHit", 0.3f);
	}

	public void ReloadCurrentLevel()
	{
		GameManager.instance.Load();
		GameManager.instance.isLoaded = true;
		Time.timeScale = 1f;
		GameManager.instance.musicSource.Play();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	void OnTriggerEnter2D (Collider2D collider)
	{
		if (collider.tag == "EnemyWeapon")
		{
			if (canHit) DamagePlayer(collider.transform.parent.transform.parent.transform.parent.transform.parent
																	.GetChild(0).GetComponent<EnemyStats>().damage);
		}
		if (collider.tag == "FireBall")
		{
			if (canHit) DamagePlayer(mageStats.damage);
			Destroy(collider.gameObject);
		}
		if (collider.tag == "BlowZone")
		{
			if (canHit) DamagePlayer(fireEleStats.damage);
		}
	}

	void PlayerBarsUpdate()
	{
		bool playing = GameManager.instance.PLAYING;

		if (playing)
		{
			healthBar.fillAmount = currentHealth / maxHealth;
			healthText.text = Mathf.Round(currentHealth) + "/" + maxHealth;
			staminaBar.fillAmount = currentStamina / maxStamina;
			staminaText.text = Mathf.Round(currentStamina) + "/" + maxStamina;
			staminaRegenText.text = "+" + staminaRegenPerSecond;
		}
	}

	public void ResetStaminaState()
	{
		usingStamina = false;
	}

//	void isUsingStamina()
//	{
//		if(usingStamina == false)
//		{
//			if (currentStamina <= maxStamina)
//			InvokeRepeating("RegenStamina", 1f, 0.05f);
//		}
//	}
		
	void RegenStamina()
	{
		if(usingStamina == false && currentStamina < maxStamina)
		{
			currentStamina += staminaRegenPerSecond * Time.deltaTime;
			Mathf.Clamp(currentStamina, 0, maxStamina);
		}
	}

	public IEnumerator RegenHealth()
	{
		GameManager.instance.playSound("potion");
		UpdatePotionStatus();

		if (currentHealth < maxHealth)
		{
			int i = 0;

			while (i < 50 && currentHealth < maxHealth)
			{
				i++;
				currentHealth += 1;
				Mathf.Clamp(currentHealth, 0, maxHealth);
				yield return new WaitForSeconds(0.1f);
			}
		}
	}

	public void UpdatePotionStatus()
	{
		potionText.text = potions.ToString();

		if (potions == 0)
		{
			potText.SetActive(false);
			potImage.SetActive(false);
		} else
		{
			potText.SetActive(true);
			potImage.SetActive(true);
		}
	}

	void CanHit()
	{
		canHit = true;
	}

	void InstantiateDamageNumber(int damage)
	{
		GameObject dmg = Instantiate(damageNumber, transform.position + new Vector3(-0.5f, 1f, 0f), Quaternion.Euler(Vector3.zero)) as GameObject;
		dmg.GetComponent<FloatingNumbers>().DamageNumber = damage;
		Destroy(dmg, 1.5f);
	}
}
