using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

	public static GameManager instance;

	public bool PLAYING;

	public AudioClip[] fxSounds;
	public AudioSource audioSource;
	public AudioSource musicSource;
	[HideInInspector]
	public AudioClip lastPlayed;
	[HideInInspector]
	public AudioClip clipToPlay;

	[HideInInspector]
	public int currentLevel, upgradePoints, currentDamage;
	[HideInInspector]
	public float nextLevelExp, currentExp, maxHealth, maxStamina, staminaRegenPerSecond;
	[HideInInspector]
	public bool door = true;
	[HideInInspector]
	public bool isLoaded = false;

	public Slider musicSlider, soundSlider;
	public GameObject music, sounds;

	public Button continueBtn, newGameBtn, optionsBtn, exitBtn;

	public GameObject checkText;

	[HideInInspector]
	public Canvas canvas;

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	// called second
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if( scene.name == "Start" )
		{
			if (canvas != null)
			{
				Destroy(GameObject.Find("Canvas"));
				canvas.enabled = true;
				NoButton();

				CheckIfSaveFileExist();
			}

			newGameBtn.onClick.AddListener (StartGame);
			continueBtn.onClick.AddListener (ContinueGame);
			optionsBtn.onClick.AddListener (Options);
			exitBtn.onClick.AddListener (ExitGame);
				
		}
			
	}

	// Use this for initialization
	void Start()
	{
		if( instance != null && instance != this )
		{
			Destroy (this.gameObject);
		} else
		{
			instance = this;
		}

		DontDestroyOnLoad (instance);
			
		int i = 0;
		audioSource.clip = fxSounds[i];

		CheckIfSaveFileExist();

		canvas = FindObjectOfType<Canvas>();
		DontDestroyOnLoad(canvas);
		canvas.name = "OnlyCanvas";
		soundSlider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
		musicSlider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
	}

	void ValueChangeCheck()
	{
		audioSource.volume = soundSlider.value;
		musicSource.volume = musicSlider.value;
	}
	
	// Update is called once per frame
	void Update()
	{
		if( Input.GetKeyDown (KeyCode.F5) )
		{
			AssignVariablesAndSaveTheGame ();
		}

		if( Input.GetKeyDown (KeyCode.Escape) )
		{
			if( GameMaster.instance != null )
			{
				if( GameMaster.instance.escapePanel.activeInHierarchy )
				{
					GameMaster.instance.escapePanel.SetActive (false);
					Time.timeScale = 1f;
					GameMaster.instance.isPaused = false;
				} else
				{
					GameMaster.instance.escapePanel.SetActive (true);
					Time.timeScale = 0f;
					GameMaster.instance.isPaused = true;
				}
			}
		}
	}

	void CheckIfSaveFileExist()
	{
		if (continueBtn != null)
		{
			if( File.Exists (Application.persistentDataPath + "/playerInfo.dat") )
			{
				Color tempC = new Color(255, 255, 255);
				tempC.a = 1;
				continueBtn.image.color = tempC;
				continueBtn.interactable = true;
			} else
			{
				Color tempC = new Color(255, 255, 255);
				tempC.a = 0.45f;
				continueBtn.image.color = tempC;
				continueBtn.interactable = false;
			}
		}
	}

	public void AssignVariablesAndSaveTheGame()
	{
		currentLevel = GameMaster.instance.currentLevel;
		upgradePoints = GameMaster.instance.upgradePoints;
		nextLevelExp = GameMaster.instance.nextLevelExp;
		currentExp = GameMaster.instance.currentExp;

		maxHealth = PlayerStats.instance.maxHealth;
		maxStamina = PlayerStats.instance.maxStamina;
		staminaRegenPerSecond = PlayerStats.instance.staminaRegenPerSecond;
		currentDamage = PlayerStats.instance.currentDamage;

		Save ();
	}

	public void playSound(string name)
	{
		int i = 0;
		if( name == "hit" )
			i = 0;
		else if( name == "jump" )
			i = 1;
		else if( name == "enemydied" )
			i = 2;
		else if( name == "stronghit" )
			i = 3;
		else if( name == "boom" )
			i = 4;
		else if( name == "chest" )
			i = 5;
		else if( name == "lvlup" )
			i = 6;
		else if( name == "youdied" )
			i = 7;
		else if (name == "potion")
			i = 8;

		clipToPlay = fxSounds[i];
		//audioSource.clip = fxSounds[i];
		audioSource.PlayOneShot (clipToPlay);
		lastPlayed = fxSounds[i];
	}

	public void StartGame()
	{
		if( File.Exists (Application.persistentDataPath + "/playerInfo.dat") )
		{
			CheckTextActive();
		} else {
			StartCoroutine(Fading("Game"));
			isLoaded = false;
			PLAYING = true;
		}
	}

	public void ContinueGame()
	{
		StartCoroutine(Fading("Game"));
		Load ();
		isLoaded = true;
		PLAYING = true;
	}

	public void Options()
	{
		music.SetActive (true);
		sounds.SetActive (true);
	}

	public void ExitGame()
	{
		Application.Quit ();
	}

	public void ResumeGame()
	{
		GameMaster.instance.escapePanel.SetActive (false);
		Time.timeScale = 1f;
		GameMaster.instance.isPaused = false;
	}

	public void BackToMenu()
	{
		PLAYING = false;
		StartCoroutine(Fading("Start"));
	}

	public void CheckTextActive()
	{
		checkText.SetActive(true);
		newGameBtn.gameObject.SetActive(false);
		continueBtn.gameObject.SetActive(false);
		optionsBtn.gameObject.SetActive(false);
		exitBtn.gameObject.SetActive(false);
		music.SetActive (false);
		sounds.SetActive (false);
	}

	public void NoButton()
	{
		if (checkText != null)
		{
			checkText.SetActive(false);
			newGameBtn.gameObject.SetActive(true);
			continueBtn.gameObject.SetActive(true);
			optionsBtn.gameObject.SetActive(true);
			exitBtn.gameObject.SetActive(true);
			music.SetActive (false);
			sounds.SetActive (false);
		}
	}

	public void YesButton()
	{
		File.Delete (Application.persistentDataPath + "/playerInfo.dat");

		StartCoroutine(Fading("Game"));
		isLoaded = false;
		PLAYING = true;
	}

	public void Credits()
	{
		StartCoroutine(Fading("Credits"));
		Invoke("BackToMenu", 25f);
	}

	IEnumerator Fading(string whatScene)
	{
		float fadeTime = GetComponent<Fading>().BeginFade(1);
		yield return new WaitForSeconds(fadeTime);
		SceneManager.LoadScene(whatScene, LoadSceneMode.Single);
		Time.timeScale = 1f;
	}

	void Save()
	{
		StartCoroutine (GameMaster.instance.GetComponent<GameMaster> ().SavedText ());

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");

		PlayerData data = new PlayerData();

		data.currentLevel = currentLevel;
		data.currentExp = currentExp;
		data.nextLevelExp = nextLevelExp;
		data.upgradePoints = upgradePoints;

		data.maxStamina = maxStamina;
		data.maxHealth = maxHealth;
		data.staminaRegenPerSecond = staminaRegenPerSecond;
		data.currentDamage = currentDamage;

		data.door = door;

		bf.Serialize (file, data);
		file.Close ();
	}

	public void Load()
	{
		if( File.Exists (Application.persistentDataPath + "/playerInfo.dat") )
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

			PlayerData data = (PlayerData)bf.Deserialize (file);
			file.Close ();

			currentLevel = data.currentLevel;
			currentExp = data.currentExp;
			nextLevelExp = data.nextLevelExp;
			upgradePoints = data.upgradePoints;

			maxStamina = data.maxStamina;
			maxHealth = data.maxHealth;
			staminaRegenPerSecond = data.staminaRegenPerSecond;
			currentDamage = data.currentDamage;

			door = data.door;
		}
	}
}

[Serializable]
class PlayerData
{
	public int currentLevel;
	public int upgradePoints;
	public float nextLevelExp;
	public float currentExp;

	public float maxStamina;
	public float staminaRegenPerSecond;

	public float maxHealth;
	public int currentDamage;

	public bool door;
}
