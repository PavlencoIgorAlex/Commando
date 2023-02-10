using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;

public class GameManager : MonoBehaviour
{
    public class ResultsData
    {
        public bool[] initialized = new bool[10] { false, false, false, false, false, false, false, false, false, false };
        public bool[] isGameWon = new bool[10] { false, false, false, false, false, false, false, false, false, false };
        public int[] numberOfFrags = new int[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public int[] score = new int[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    }
    
    void SaveResults(ResultsData resData)
    {
        ResultsData data = new ResultsData();
        data.initialized = resData.initialized;
        data.isGameWon = resData.isGameWon;
        data.numberOfFrags = resData.numberOfFrags;
        data.score = resData.score;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    void LoadResults(ResultsData resData)
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ResultsData data = JsonUtility.FromJson<ResultsData>(json);

            resData.initialized = data.initialized;
            resData.isGameWon = data.isGameWon;
            resData.numberOfFrags = data.numberOfFrags;
            resData.score = data.score;
        }
        else
       {
            SaveResults(resData);
        }
    }

    void UpdateResultsData(ResultsData resData, bool tmp_initialized, bool tmp_isGameWon, int tmp_numberOfFrags, int tmp_score)
    {
        Array.Copy(resData.initialized, 0, resData.initialized, 1, 9);
        Array.Copy(resData.isGameWon, 0, resData.isGameWon, 1, 9);
        Array.Copy(resData.numberOfFrags, 0, resData.numberOfFrags, 1, 9);
        Array.Copy(resData.score, 0, resData.score, 1, 9);
        resData.initialized[0] = tmp_initialized;
        resData.isGameWon[0] = tmp_isGameWon;
        resData.numberOfFrags[0] = tmp_numberOfFrags;
        resData.score[0] = tmp_score;
    }

    //******************************************************

    //Constants
    const int numberOfGuns = 3;

    //Variables
    public bool isGameActive = false;
    private bool isGameStarted = false;
    private bool isGameWon = false;
    private bool isGameLosed = false;
    private bool initialization = true;

    private int score;
    private int health;

    enum Weapons
    {
        Pistol = 0,
        Rifle,
        MachineGun,
        Undefined
    }

    private Shootable player;
    public GunProfile[] gunProfiles = new GunProfile[numberOfGuns];
    public int activeGun = 0;
    public int[] ammoRemain = new int[numberOfGuns];
    public TextMeshProUGUI ammoRemain1Text;
    public TextMeshProUGUI ammoRemain2Text;
    public TextMeshProUGUI ammoRemain3Text;

    public GameObject Item1ActivePanel;
    public GameObject Item2ActivePanel;
    public GameObject Item3ActivePanel;
    public GameObject ResultsPanel;

    private AudioSource GetItSound;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI resultsText;

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI winnerText;

    public Button startButton;
    public Button restartButton;
    public Button exitButton;

    private int frags = 0;

    private ResultsData resData;


    // Start is called before the first frame update
    void Start()
    {
        resData = new ResultsData();
        LoadResults(resData);
        UpdateResults();

        player = GameObject.Find("Player").GetComponent<Shootable>();
        health = player.currentHealth;
        UpdateHealth(0);

        SetActiveGun(0);
        gunProfiles[0] = GameObject.Find("Item 1").GetComponent<GunProfile>();
        ammoRemain[0]  = gunProfiles[0].gunCount;
        UpdateAmmo(0);

        SetActiveGun(1);
        gunProfiles[1] = GameObject.Find("Item 2").GetComponent<GunProfile>();
        ammoRemain[1]  = gunProfiles[1].gunCount;
        UpdateAmmo(0);

        SetActiveGun(2);
        gunProfiles[2] = GameObject.Find("Item 3").GetComponent<GunProfile>();
        ammoRemain[2]  = gunProfiles[2].gunCount;
        UpdateAmmo(0);

        SetActiveGun(0);
        UpdateScore(0);
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealth(0);
        if (health <= 0 && !isGameLosed)
        {
            GameOver();
        }
        if (frags >= 30 && !isGameWon)
        {
            WinningGame();
            gameOverText.gameObject.SetActive(false);
        }
    }

    public void WinningGame()
    {
        winnerText.gameObject.SetActive(true);
        isGameWon = true;
        UpdateResults();
        PauseGame();
    }

    public void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
        isGameLosed = true;
        UpdateResults();
        PauseGame();
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd * gunProfiles[activeGun].scoreMultiplier;
        scoreText.text = "Score:" + score;
    }

    public void UpdateFrags(int fragsToAdd)
    {
        frags += fragsToAdd;
    }

    public void UpdateHealth(int healthToReduce)
    {
        health = player.currentHealth;
        healthText.text = "Health:" + health;
    }

    public void UpdateResults()
    {
        if (!initialization)
        {
            UpdateResultsData(resData, true, isGameWon, frags, score);
        }
        else
        {
            initialization = false;
        }
        resultsText.text = "#\tW/L\tKills\tScore\n";
        string state;  
        for (int i = 1; i < 11 && resData.initialized[i-1]; i++)
        {
            state = resData.isGameWon[i - 1] == true ? "Win\t" : "Lose";
            resultsText.text = resultsText.text + i + ":\t" + state + "\t" + resData.numberOfFrags[i - 1] + "\t" + resData.score[i - 1] + "\n";
        }
    }

    public void UpdateAmmo(int ammoToReduce)
    {
        if (!gunProfiles[activeGun].isInfiniteAmmo)
        {
            ammoRemain[activeGun] -= ammoToReduce;
            switch (activeGun)
            {
                case 0:
                    ammoRemain1Text.text = "" + ammoRemain[activeGun];
                    break;
                case 1:
                    ammoRemain2Text.text = "" + ammoRemain[activeGun];
                    break;
                case 2:
                    ammoRemain3Text.text = "" + ammoRemain[activeGun];
                    break;
            }
        }
    }

    public void StartGame()
    {
        isGameActive = true;
        startButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        ResultsPanel.SetActive(false);
        Cursor.visible = false;
        isGameStarted = true;
    }

    public void RestartGame()
    {
        SaveResults(resData);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); isGameActive = true;
    }

    public void ExitGame()
    {
        SaveResults(resData);
        Application.Quit();
    }

    public void PauseGame()
    {
        if (isGameStarted)
        {
            if (isGameActive)
            {
                isGameActive = false;
                restartButton.gameObject.SetActive(true);
                exitButton.gameObject.SetActive(true);
                ResultsPanel.SetActive(true);
                Cursor.visible = true;
            }
            else if (!isGameWon && !isGameLosed)
            {
                isGameActive = true;
                restartButton.gameObject.SetActive(false);
                exitButton.gameObject.SetActive(false);
                ResultsPanel.SetActive(false);
                Cursor.visible = false;
            }
        }
    }

    public ref GunProfile GetGunProfile()
    {
        return ref gunProfiles[activeGun];
    }

    public int GetCurAmmoRemain()
    {
        return ammoRemain[activeGun];
    }

    public void SetActiveGun(int number)
    {
        if (activeGun != number)
        { 
            activeGun = number;
            switch (activeGun)
            {
                case 0:
                    Item1ActivePanel.gameObject.SetActive(true);
                    Item2ActivePanel.gameObject.SetActive(false);
                    Item3ActivePanel.gameObject.SetActive(false);
                    GetItSound = GameObject.Find("Get It Sound 1").GetComponent<AudioSource>();
                    break;
                case 1:
                    Item1ActivePanel.gameObject.SetActive(false);
                    Item2ActivePanel.gameObject.SetActive(true);
                    Item3ActivePanel.gameObject.SetActive(false);
                    GetItSound = GameObject.Find("Get It Sound 2").GetComponent<AudioSource>();
                    break;
                case 2:
                    Item1ActivePanel.gameObject.SetActive(false);
                    Item2ActivePanel.gameObject.SetActive(false);
                    Item3ActivePanel.gameObject.SetActive(true);
                    GetItSound = GameObject.Find("Get It Sound 3").GetComponent<AudioSource>();
                    break;
            }
            if (isGameActive)
            {
                GetItSound.Play();
            }
        }
    }

    public bool IsMachineGun()
    {
        return activeGun == (int)Weapons.MachineGun;
    }
}
