  using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using TMPro;
using System;
using System.Linq;
using UnityEngine.UI;

public class DataBaseManager : MonoBehaviour
{
    private DatabaseReference dbReference;
    public GameObject loginPanel;
    public GameObject changeNickBackButton;
    [Header("ID")]
    string id;
    public TMP_InputField idtext;
    public GameObject idTextObject;
    [Header("Confirm")]
    public GameObject infoTextObject;
    public TextMeshProUGUI infoText;
    public GameObject confirmButtonObject;
    [Header("Nick")]
    public TMP_InputField nickText;
    public GameObject nickTextObject;
    public GameObject confirmNickObject;
    [Header("Get Info")]
    public GameObject getInfoPanel;
    public TextMeshProUGUI getIdText;
    public TextMeshProUGUI getNickText;
    [Header("Score")]
    public GameObject scorePanel;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI lastScoreText;
    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI earnedAvaxText;
    [Header("LeadBoardElements")]
    public GameObject content;
    public GameObject textElement;
    public GameObject leadBoardPanel;
    public GameObject[] leadBoardActiveElements;
    [Header("Internet Check")]
    public GameObject noInternetPanel;


    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }

    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        if (PlayerPrefs.HasKey("selfId"))
        {
            getInfoPanel.SetActive(true);
            getIdText.text = getId();
            StartCoroutine(getNick((string name) =>
            {
                getNickText.text = name;
            }));
        }
        else
        {
            loginPanel.SetActive(true);
        }
    }
    private void Update()
    {
        if (content.activeSelf == true)
        {
            content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, content.GetComponent<RectTransform>().anchoredPosition.y);
        }
    }
    public void closeLeadBoardMethod()
    {
        StopCoroutine(leadBoard());
        foreach (GameObject g in leadBoardActiveElements)
        {
            g.SetActive(true);
        }
        Transform[] contentChild = content.GetComponentsInChildren<Transform>();
        for (int i = 1; i < contentChild.Length; i++)
        {
            Destroy(contentChild[i].gameObject);
        }
        leadBoardPanel.SetActive(false);

    }
    public void leadBoardMethod()
    {
        foreach(GameObject g in leadBoardActiveElements)
        {
            g.SetActive(false);
        }
        scorePanel.SetActive(false);
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            leadBoardPanel.SetActive(true);
            StartCoroutine(leadBoard());
        }
        else
        {
            noInternetPanel.SetActive(true);
        }
        
    }
    public void confirmButton()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            confirmButtonObject.GetComponent<Button>().enabled = false;
            if (!string.IsNullOrEmpty(idtext.text))
            {
                id = idtext.text;
                StartCoroutine(idCheckMethod());

            }
            else
            {
                if (infoTextObject.activeSelf == false)
                {
                    infoTextObject.SetActive(true);
                }
                infoText.color = Color.red;
                infoText.text = "Empty ID";
                confirmButtonObject.GetComponent<Button>().enabled = true;
            }
        }
        else
        {
            noInternetPanel.SetActive(true);
        }
       
    }
    IEnumerator getNick(Action<string> onCallBacks)
    {
        var nick = dbReference.Child("Users").Child(PlayerPrefs.GetString("selfId")).Child("Nick").GetValueAsync();
        yield return new WaitUntil(predicate:() => nick.IsCompleted);
        if (nick != null)
        {
            DataSnapshot snapshot = nick.Result;
            onCallBacks.Invoke(snapshot.Value.ToString());
        }
    }
    IEnumerator idCheckMethod()
    {
        confirmButtonObject.GetComponent<Button>().enabled = false;
        var idCheckIds = dbReference.Child("IDs").Child(id).GetValueAsync();
        yield return new WaitUntil(predicate: () => idCheckIds.IsCompleted);
        DataSnapshot snapshot = idCheckIds.Result;

        var nickCheck = dbReference.Child("Users").Child(id).Child("Nick").GetValueAsync();
        yield return new WaitUntil(predicate: () => nickCheck.IsCompleted);
        DataSnapshot snapshotC = nickCheck.Result;

        if (snapshot.Value == null)
        {
            infoTextObject.SetActive(true);
            infoText.color = Color.red;
            infoText.text = "Invalid ID";
            confirmButtonObject.GetComponent<Button>().enabled = true;
        }
        else if (!snapshot.Value.ToString().Contains("Used"))
        {
            newUser(id);
            dbReference.Child("IDs").Child(id).SetValueAsync(snapshot.Value.ToString()+" Used");
            infoTextObject.SetActive(true);
            infoText.color = Color.green;
            infoText.text = "Valid ID";
            idTextObject.SetActive(false);
            nickTextObject.SetActive(true);
            confirmNickObject.SetActive(true);
            confirmButtonObject.SetActive(false);
        }
        else if (snapshotC.Value.ToString().Contains("Banned"))
        {
            infoTextObject.SetActive(true);
            infoText.color = Color.red;
            infoText.text = "Banned ID";
            confirmButtonObject.GetComponent<Button>().enabled = true;
        }
        else if (snapshot.Value.ToString().Contains("Used"))
        {
            if (snapshotC.Value.ToString().Equals(""))
            {
                infoTextObject.SetActive(true);
                infoText.color = Color.green;
                infoText.text = "Valid ID";
                idTextObject.SetActive(false);
                nickTextObject.SetActive(true);
                confirmNickObject.SetActive(true);
                confirmButtonObject.SetActive(false);
                PlayerPrefs.SetString("selfId", id);
                
            }
            else
            {
                PlayerPrefs.SetString("selfId", id);
                loginPanel.SetActive(false);
                getInfoPanel.SetActive(true);
                getIdText.text = getId();
                StartCoroutine(getNick((string name) =>
                {
                    getNickText.text = name;
                }));
            } 
        }
    }

    void newUser(string id)
    {
        dbReference.Child("Users").Child(id).Child("Nick").SetValueAsync("");
        dbReference.Child("Users").Child(id).Child("Game In Play Time").SetValueAsync((int)0);
        dbReference.Child("Users").Child(id).Child("High Score").SetValueAsync((int)0);
        dbReference.Child("Users").Child(id).Child("Last Score").SetValueAsync((int)0);
        dbReference.Child("Users").Child(id).Child("Total Score").SetValueAsync((int)0);
    }
    public void confirmNickButton()
    {
        if (nickText.text.ToString().Contains("Banned"))
        {
            infoTextObject.SetActive(true);
            infoText.color = Color.red;
            infoText.text = "Banned Should Not Exist";
            return;
        }
        if(nickText.text.Length <= 12)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                confirmNickObject.SetActive(false);
                StartCoroutine(getAllNick());
                PlayerPrefs.SetString("selfId", id);
                
            }
            else
            {
                noInternetPanel.SetActive(true);
            }
        }
        else
        {
            infoTextObject.SetActive(true);
            infoText.color = Color.red;
            infoText.text = "Max 12 Character";
        }
    }
    IEnumerator getAllNick()
    {
        string nick = nickText.text;

        if (!string.IsNullOrEmpty(nick))
        {
            var nicks = dbReference.Child("Users").OrderByChild("Nick").GetValueAsync();
            yield return new WaitUntil(predicate: () => nicks.IsCompleted);
            DataSnapshot nicksDataSnapshot = nicks.Result;
            foreach (DataSnapshot childSnapshot in nicksDataSnapshot.Children.Reverse<DataSnapshot>())
            {
                if (nick.Equals(childSnapshot.Child("Nick").Value.ToString()))
                {
                    infoTextObject.SetActive(true);
                    infoText.color = Color.red;
                    infoText.text = "Used Nick";
                    confirmNickObject.SetActive(true);
                    yield break;
                }
            }
            dbReference.Child("Users").Child(id).Child("Nick").SetValueAsync(nick);
            loginPanel.SetActive(false);
            getInfoPanel.SetActive(true);
            getIdText.text = getId();
            StartCoroutine(getNick((string name) =>
            {
                getNickText.text = name;
            }));
        }
        else
        {
            infoText.color = Color.red;
            infoText.text = "Empty Nick";
            confirmNickObject.SetActive(true);
        }
    }
    public string getId()
    {
        return PlayerPrefs.GetString("selfId");
    }
   public IEnumerator getHighScore(Action<int> onCallBacks)
    {
        var highScore = dbReference.Child("Users").Child(getId()).Child("High Score").GetValueAsync();
        yield return new WaitUntil(predicate: () => highScore.IsCompleted);
        if (highScore != null)
        {
            DataSnapshot snapshot = highScore.Result;
            onCallBacks.Invoke(int.Parse(snapshot.Value.ToString()));
        }
    }
    public IEnumerator getlastScore(Action<int> onCallBacks)
    {
        var lastScore = dbReference.Child("Users").Child(getId()).Child("Last Score").GetValueAsync();
        yield return new WaitUntil(predicate: () => lastScore.IsCompleted);
        if (lastScore != null)
        {
            DataSnapshot snapshot = lastScore.Result;
            onCallBacks.Invoke(int.Parse(snapshot.Value.ToString()));
        }
    }
    public IEnumerator gettotalScore(Action<int> onCallBacks)
    {
        var totalScore = dbReference.Child("Users").Child(getId()).Child("Total Score").GetValueAsync();
        yield return new WaitUntil(predicate: () => totalScore.IsCompleted);
        if (totalScore != null)
        {
            DataSnapshot snapshot = totalScore.Result;
            onCallBacks.Invoke(int.Parse(snapshot.Value.ToString()));
        }
    }

    public void scoreMethod()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (scorePanel.activeSelf == true)
            {
                scorePanel.SetActive(false);
            }
            else if (scorePanel.activeSelf == false)
            {
                scorePanel.SetActive(true);
                StartCoroutine(getHighScore((int s) =>
                {
                    highScoreText.text = s.ToString();
                }));
                StartCoroutine(getlastScore((int s) =>
                {
                    lastScoreText.text = s.ToString();
                }));
                StartCoroutine(gettotalScore((int s) =>
                {
                    totalScoreText.text = s.ToString();
                    if (s >= 65000)
                    {
                        earnedAvaxText.text = "0.34";
                    }else if (s >= 40000)
                    {
                        earnedAvaxText.text = "0.21";
                    }else if (s >= 20000)
                    {
                        earnedAvaxText.text = "0.1";
                    }
                    else
                    {
                        earnedAvaxText.text = "0.0";
                    }
                }));
            }
        }
        else
        {
            noInternetPanel.SetActive(true);
        }
       
    }
    public void setLastScore(int score)
    {
        dbReference.Child("Users").Child(getId()).Child("Last Score").SetValueAsync(score);
    }
    public void setHighScore(int score)
    {
        dbReference.Child("Users").Child(getId()).Child("High Score").SetValueAsync(score);
    }
    IEnumerator leadBoard()
    {
        Dictionary<string, int> array = new Dictionary<string, int>();
        var board = dbReference.Child("Users").OrderByChild("Nick").GetValueAsync();
        
        yield return new WaitUntil(predicate: () => board.IsCompleted);
        DataSnapshot nicksDataSnapshot = board.Result;
        foreach (DataSnapshot childSnapshot in nicksDataSnapshot.Children.Reverse<DataSnapshot>())
        {
            if (!childSnapshot.Child("Nick").Value.ToString().Contains("Banned"))
            {
                array.Add(childSnapshot.Child("Nick").Value.ToString(), int.Parse(childSnapshot.Child("Total Score").Value.ToString()));
            }
            
        }
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, array.Count * 120);
        int textElementY = 0;
        TextMeshProUGUI[] texts;
        for(int i = 0; i < array.Count; i++)
        {
            GameObject s =  Instantiate(textElement, Vector2.zero, Quaternion.identity);
            s.transform.SetParent(content.transform);
            s.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, textElementY);
            s.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            textElementY = textElementY - 120;
        }
        texts = content.GetComponentsInChildren<TextMeshProUGUI>();
        Debug.Log(texts.Length);
        int t = 0;
        int sira = 1;
        foreach (KeyValuePair<string, int> author in array.OrderByDescending(key => key.Value))
        {
            texts[t].text = sira + ".";
            t++;
            texts[t].text = author.Key.ToString();
            t++;
            texts[t].text = author.Value.ToString();
            t++;
            sira++;
        }
    }
    public void changeNickButton()
    {
        loginPanel.SetActive(true);
        idTextObject.SetActive(false);
        nickTextObject.SetActive(true);
        confirmNickObject.SetActive(true);
        confirmButtonObject.SetActive(false);
        changeNickBackButton.SetActive(true);
    }
}
