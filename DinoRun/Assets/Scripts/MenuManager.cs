using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using System;

public class MenuManager : MonoBehaviour
{
    public GameObject bannedPanel;

    public GameObject noInternetPanel;

    private DatabaseReference dbReference;
    public Slider slider;
    public float speedChangeDuration;
    Dino dino;
    public ObstacleSpawnerSc obstacleSpawner;
    bool laserInstantReady = false;

    public GameObject gameEndUI;
    public TextMeshProUGUI scoreText;
    public TMP_InputField field;
    public Container container;
    public GameObject text;

    public AudioSource startMusic;
    public AudioSource gameMusic;

    public LevelGenerator generator;

    private float speedModifier = 1;
    private int score = 0;
    bool gameEnd = false;
    float speedC = 1;

    int gameInPlayTime = 0;

    public GameObject music;

    public GameObject pausePanel;
    public TextMeshProUGUI dieScoreText;
    public GameObject gameInScoreText;
    public GameObject[] accountBannedUI;
    bool loadMenu;
    
    int threadInt = 0;

    private readonly object _locker = new object();
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }
    private void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        StartCoroutine(setGameInPlayTime());

        dino = GameObject.Find("Player").GetComponent<Dino>();
        slider.value = PlayerPrefs.GetFloat("volume");
        startMusic.volume = PlayerPrefs.GetFloat("volume");
        gameMusic.volume = PlayerPrefs.GetFloat("volume");
        if (dino != null)
        {
            dino.dinoSoundVolumeChange();
        }

    }
    private void OnEnable()
    {
        Time.timeScale = 1;
        Dino.GameEnd += GameEnd;
    }
    private void Update()
    {
        Debug.Log(threadInt);
        if (threadInt == 4 && loadMenu)
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            SceneManager.LoadScene(0);

        }
        if (threadInt == 4 && loadMenu==false)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            

        }

        if (Time.timeScale == 0 && Input.GetKeyDown(KeyCode.Escape) && gameEnd == false)
        {
            Time.timeScale = 1;
            if (pausePanel.activeSelf == true)
            {
                music.SetActive(false);
                pausePanel.SetActive(false);
            }

        }
        else if (Input.GetKeyDown(KeyCode.Escape) && gameEnd == false)
        {
            Time.timeScale = 0;

            if (pausePanel.activeSelf == false)
            {
                pausePanel.SetActive(true);
                music.SetActive(true);
            }

        }
        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !container.start)
        {
            startMusic.Stop();
            gameMusic.Play();
            container.start = true;
            text.SetActive(false);
            StartCoroutine(UpdateScore());
        }
    }
    private IEnumerator UpdateScore()
    {
        while (true)
        {
            score++;
            if (laserInstantReady)
            {
                if (score % 15 == 0)
                {
                    obstacleSpawner.laserInstantMethod();
                }
                if (score % 15 == 0)
                {
                    obstacleSpawner.laserPercentMethod();
                }
            }
            if (score % 50 == 0)
            {
                speedModifier += 0.035f;
                speedModifier = Mathf.Clamp(speedModifier, 1, 2f);

                //container.speed = 6 * x; // 6 * 1.5;
            }
            if (score <= 730)
            {
                if (score % 40 == 0)
                {
                    speedC += 0.035f;
                    double x = speedC / 1.2f;
                    StartCoroutine(speedChnageMethod(x));
                }
            }


            if (score % 230 == 0)
            {
                laserInstantReady = true;
                generator.ChangeMap();
            }

            scoreText.text = score.ToString();

            yield return new WaitForSeconds(.35f * speedModifier); // 0.35 * 1.05;
        }
    }
    public void AdjustVolume(float vol)
    {
        startMusic.volume = vol;
        gameMusic.volume = vol;
        if (dino != null)
        {
            dino.dinoSoundVolumeChange();
        }
        PlayerPrefs.SetFloat("volume", vol);

    }
    public void SubmitScore()
    {
        //manager.PostJSON("Scores", field.text, score.ToString());
    }
    public void UpdateLeaderboard()
    {
        //manager.Leaderboard();
    }

    private void GameEnd()
    {
        gameEnd = true;
        Invoke("StopGame", 0.01f);
        gameMusic.Stop();
        gameEndUI.SetActive(true);
        music.SetActive(true);
        dieScoreText.text = gameInScoreText.GetComponent<TextMeshProUGUI>().text;
        gameInScoreText.SetActive(false);
    }
    private void StopGame()
    {
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        Dino.GameEnd -= GameEnd;
    }
    public void Restart()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            StartCoroutine(getNick((string name) =>
            {
                if (name.Contains("Banned"))
                {
                    foreach (GameObject g in accountBannedUI)
                    {
                        g.SetActive(false);
                    }
                    bannedPanel.SetActive(true);
                }
                else
                {
                    loadMenu = false;
                    setLastScore(score);
                    StartCoroutine(setHighScore(score));
                    StartCoroutine(setTotalScore(score));
                    StartCoroutine(setTotalPlayTime());

                }
            }));

        }
        else
        {
            noInternetPanel.SetActive(true);
        }

    }
    public void Menu()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {

            StartCoroutine(getNick((string name) =>
            {
                if (name.Contains("Banned"))
                {
                    foreach (GameObject g in accountBannedUI)
                    {
                        g.SetActive(false);
                    }
                    bannedPanel.SetActive(true);
                }
                else
                {
                    loadMenu = true;
                    setLastScore(score);
                    StartCoroutine(setHighScore(score));
                    StartCoroutine(setTotalScore(score));
                    StartCoroutine(setTotalPlayTime());

                }
            }));

        }
        else
        {
            noInternetPanel.SetActive(true);
        }

    }
    IEnumerator speedChnageMethod(double x)
    {
        double speed = container.speed;

        x = x / 20;
        for (int i = 0; i < 20; i++)
        {
            speed = x + speed;
            container.speed = (float)speed;

            yield return new WaitForSeconds(speedChangeDuration);
        }
        Debug.Log(speed);
    }
    public void setLastScore(int score)
    {
        dbReference.Child("Users").Child(PlayerPrefs.GetString("selfId")).Child("Last Score").SetValueAsync(score);
        intThearedMethod();
    }
    public IEnumerator setTotalPlayTime()
    {
        var totalPlayTime = dbReference.Child("Users").Child(PlayerPrefs.GetString("selfId")).Child("Game In Play Time").GetValueAsync();
        yield return new WaitUntil(predicate: () => totalPlayTime.IsCompleted);
        if (totalPlayTime != null)
        {
            DataSnapshot snapshot = totalPlayTime.Result;
            int s = int.Parse(snapshot.Value.ToString());
            s = s + gameInPlayTime;
            gameInPlayTime = 0;
            dbReference.Child("Users").Child(PlayerPrefs.GetString("selfId")).Child("Game In Play Time").SetValueAsync(s);

        }
        intThearedMethod();
    }
    public IEnumerator setTotalScore(int score)
    {
        var totalScore = dbReference.Child("Users").Child(PlayerPrefs.GetString("selfId")).Child("Total Score").GetValueAsync();
        yield return new WaitUntil(predicate: () => totalScore.IsCompleted);
        if (totalScore != null)
        {
            DataSnapshot snapshot = totalScore.Result;
            int s = int.Parse(snapshot.Value.ToString());
            s = s + score;
            dbReference.Child("Users").Child(PlayerPrefs.GetString("selfId")).Child("Total Score").SetValueAsync(s);

        }
        intThearedMethod();
    }
    IEnumerator setGameInPlayTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            gameInPlayTime++;
        }

    }
    public IEnumerator setHighScore(int lastScore)
    {
        var highScore = dbReference.Child("Users").Child(PlayerPrefs.GetString("selfId")).Child("High Score").GetValueAsync();
        yield return new WaitUntil(predicate: () => highScore.IsCompleted);
        if (highScore != null)
        {
            DataSnapshot snapshot = highScore.Result;
            int s = int.Parse(snapshot.Value.ToString());
            Debug.Log(s);
            Debug.Log(lastScore);
            if (lastScore > s)
            {
                dbReference.Child("Users").Child(PlayerPrefs.GetString("selfId")).Child("High Score").SetValueAsync(lastScore);

            }
        }
        intThearedMethod();
    }
    public void bannedButton()
    {
        Application.Quit();
    }
    IEnumerator getNick(Action<string> onCallBacks)
    {
        var nick = dbReference.Child("Users").Child(PlayerPrefs.GetString("selfId")).Child("Nick").GetValueAsync();
        yield return new WaitUntil(predicate: () => nick.IsCompleted);
        if (nick != null)
        {
            DataSnapshot snapshot = nick.Result;
            onCallBacks.Invoke(snapshot.Value.ToString());
        }
    }
     void intThearedMethod()
    {
        lock (_locker)
        {
            threadInt++;
        }
        
    }
}

