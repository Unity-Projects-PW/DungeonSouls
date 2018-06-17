using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameMaster : MonoBehaviour {

	public static GameMaster instance;
	private PlayerStats playerStats;

	public Text UI_text;
	public Slider musicSlider;
	public Slider soundSlider;

	[HideInInspector]
	public bool isPaused = false;

	private int maxLevel = 60;

	[HideInInspector]
	public int currentLevel = 1;
	[HideInInspector]
	public int upgradePoints = 0;
	[HideInInspector]
	public float nextLevelExp = 50;
	[HideInInspector]
	public float currentExp = 0;

	public GameObject escapePanel;

	[Header("Level Up Panel")]
	public GameObject panel;
	public GameObject levelUPAnim;

	[Header("Panel Stats")]
	public Text level;
	public Text hp;
	public Text stamina;
	public Text staminaReg;
	public Text attackPower;
	public Text pointsLeft;

	public Text playerExpText;
	public Image expBar;

	[Header("Panel Tooltip Stats")]
	public Text[] plusStats;

	[Header("Game leveling system settings")]
	public int HPPerLevel;
	public int StaminaPerLevel;
	public int StaminaRegPerLevel;
	public int APPerLevel;

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	// called second
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "Game")
		{
			if (this != null)
			{
				GameManager.instance.canvas.enabled = false;

				if (GameManager.instance.isLoaded)
				{
					currentLevel = GameManager.instance.currentLevel;
					upgradePoints = GameManager.instance.upgradePoints;
					nextLevelExp = GameManager.instance.nextLevelExp;
					currentExp = GameManager.instance.currentExp;
				}

				soundSlider.value = GameManager.instance.soundSlider.value;
				musicSlider.value = GameManager.instance.musicSlider.value;

				Invoke("SetUp", 0.5f);
				Invoke("ToolTipSetUp", 0.5f);
				Invoke("TooltipUpdate", 0.5f);
			}
		}
	}
		
	// Use this for initialization
	void Start () 
	{

		if( instance == null )
			instance = this;

		if (GameManager.instance.isLoaded)
		{
			currentLevel = GameManager.instance.currentLevel;
			upgradePoints = GameManager.instance.upgradePoints;
			nextLevelExp = GameManager.instance.nextLevelExp;
			currentExp = GameManager.instance.currentExp;
		}

		soundSlider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
		musicSlider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });

		ToolTipSetUp();
		TooltipUpdate();
		SetUp();
	}

	void FixedUpdate()
	{
		CheckForLevel();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F) && !escapePanel.activeInHierarchy)
		{
			if (panel.activeInHierarchy)
			{
				panel.SetActive(false);
				Time.timeScale = 1f;
				isPaused = false;
			} else {
				panel.SetActive(true);
				Time.timeScale = 0f;
				isPaused = true;
			}
		}
	}

	void ValueChangeCheck()
	{
		GameManager.instance.soundSlider.value = soundSlider.value;
		GameManager.instance.musicSlider.value = musicSlider.value;	
	}

	void ToolTipSetUp()
	{
		plusStats[0].text = HPPerLevel.ToString();
		plusStats[1].text = APPerLevel.ToString();
		plusStats[2].text = StaminaPerLevel.ToString();
		plusStats[3].text = StaminaRegPerLevel.ToString();
	}

	void SetUp()
	{
		playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();

		level.text = "Lvl " + currentLevel.ToString();
		expBar.fillAmount = currentExp / nextLevelExp;
		playerExpText.text = currentExp.ToString() + " / " + nextLevelExp.ToString();
		hp.text = playerStats.maxHealth.ToString();
		stamina.text = playerStats.maxStamina.ToString();
		staminaReg.text = playerStats.staminaRegenPerSecond.ToString();
		attackPower.text = playerStats.currentDamage.ToString();
		pointsLeft.text = upgradePoints.ToString();

	}

	void CheckForLevel()
	{
			
		if(currentExp >= nextLevelExp && currentLevel <= maxLevel)
		{
			GameManager.instance.playSound("lvlup");
			GameObject levelup = Instantiate(levelUPAnim, playerStats.transform.position, Quaternion.Euler(Vector3.zero)) as GameObject;
			Destroy(levelup, 1.5f);
			currentExp = Mathf.Round(currentExp - nextLevelExp);
			upgradePoints++;
			nextLevelExp = Mathf.Round(nextLevelExp * 1.6f);
			currentLevel++;
			TooltipUpdate();
			SetUp();
		}

	}

	void TooltipUpdate()
	{
		if (upgradePoints > 0)
		{
			foreach(Text tooltip in plusStats)
			{
				tooltip.enabled = true;
			}
		} else {
			foreach(Text tooltip in plusStats)
			{
				tooltip.enabled = false;
			}
		}
	}

	public void UpdateExpBar(float exp)
	{
		currentExp += exp;
		playerExpText.text = currentExp.ToString() + " / " + nextLevelExp.ToString();
		expBar.fillAmount = currentExp / nextLevelExp;
	}

	public void UpgradeStats(string stat)
	{
		if (upgradePoints > 0)
		{
			switch (stat)
			{
			case "hp":
				playerStats.maxHealth += HPPerLevel;
				playerStats.currentHealth += HPPerLevel;
				break;
			case "stamina":
				playerStats.maxStamina += StaminaPerLevel;
				break;
			case "staminareg":
				playerStats.staminaRegenPerSecond += StaminaRegPerLevel;
				break;
			case "ap":
				playerStats.currentDamage += APPerLevel;
				break;
			}

			upgradePoints--;
			SetUp();
			TooltipUpdate();
		}
	}

	public IEnumerator SavedText()
	{
		UI_text = GameObject.Find("UI_Text").GetComponent<Text>();
		UI_text.text = "Game has been Saved!";
		UI_text.enabled = true;
		yield return new WaitForSeconds(2f);
		if (UI_text != null)
		{
			UI_text.enabled = false;
			UI_text.text = "";
		}
	}

	public void BackToMenu()
	{
		isPaused = false;
		GameManager.instance.PLAYING = false;
		Time.timeScale = 1f;
		GameManager.instance.BackToMenu();
	}

	public void ResumeGame()
	{
		Time.timeScale = 1f;
		GameManager.instance.ResumeGame();
	}

	public void SaveGame()
	{
		GameManager.instance.AssignVariablesAndSaveTheGame();
	}

}
