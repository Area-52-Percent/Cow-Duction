using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<HighscoreEntry> highscoreEntryList;
    private List<Transform> highscoreEntryTransformList;
    public GameObject playerInput;
    public List<Text> player1NameInput;
    public List<Text> player2NameInput;
    private int curScore;

    

    private void Awake()
    {
        //Find highscore list
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");
        curScore = GameObject.Find("UI").GetComponent<SC_AlienUIManager>().GetScore();
        if (NewHighScore(curScore))
        {
            playerInput.SetActive(true);
        }
        entryTemplate.gameObject.SetActive(false);
        Highscores highscores = new Highscores();
        //create new default highscore list
        if (!PlayerPrefs.HasKey("highscoreTable"))
        {
            highscoreEntryList = new List<HighscoreEntry>()
            {
                new HighscoreEntry{ score = 8, player1Name="AAA", player2Name="AAA"},
                new HighscoreEntry{ score = 7, player1Name="CAT", player2Name="REQ"},
                new HighscoreEntry{ score = 6, player1Name="JOE", player2Name="ANN"},
                new HighscoreEntry{ score = 5, player1Name="MIK", player2Name="DAV"},
                new HighscoreEntry{ score = 4, player1Name="TOM", player2Name="JON"},
                new HighscoreEntry{ score = 3, player1Name="MAX", player2Name="WEN"},
                new HighscoreEntry{ score = 2, player1Name="LOU", player2Name="TAN"},
                new HighscoreEntry{ score = 1, player1Name="ORN", player2Name="REN"},
            };
            highscores.highscoreEntryList = highscoreEntryList;
            string json = JsonUtility.ToJson(highscores);
            PlayerPrefs.SetString("highscoreTable", json);
            PlayerPrefs.Save();
        }
        else
        {
            //Load highscore list
            string jsonString = PlayerPrefs.GetString("highscoreTable");
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
        }

        //build onscreen list
        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
    }
    //creates the onscreen highscore list
    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> tranformList)
    {
        float templateHeight = 50f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * tranformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = tranformList.Count + 1;
        string rankString;
        switch (rank)
        {
            case 1:
                rankString = "1ST";
                break;
            case 2:
                rankString = "2ND";
                break;
            case 3:
                rankString = "3RD";
                break;
            default:
                rankString = rank + "TH";
                break;
        }
        entryTransform.Find("positionText").GetComponent<Text>().text = rankString;
        string player1Name = highscoreEntry.player1Name;
        entryTransform.Find("player1Text").GetComponent<Text>().text = player1Name;
        string player2Name = highscoreEntry.player2Name;
        entryTransform.Find("player2Text").GetComponent<Text>().text = player2Name;
        int score = highscoreEntry.score; ;
        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        tranformList.Add(entryTransform);
    }
    //adds a new highscore to the list
    public void AddHighscoreEntry(int score, string player1Name, string player2Name)
    {
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, player1Name = player1Name, player2Name = player2Name };
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        if(highscores.highscoreEntryList.Count == 10)
            highscores.highscoreEntryList[9] = highscoreEntry;
        else
            highscores.highscoreEntryList.Add(highscoreEntry);
        highscores.Sort();
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();

    }

    public void SaveScore()
    {
        AddHighscoreEntry(curScore,(player1NameInput[0].text + player1NameInput[1].text + player1NameInput[2].text ), (player2NameInput[0].text + player2NameInput[1].text + player2NameInput[2].text));
    }

    //Check for a new highscore
    public bool NewHighScore(int i)
    {
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        return highscores.NewHighScore(i);
    }

    //used to store highscore list and manipulate it
    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
        public void Sort()
        {
            for (int i = 0; i < highscoreEntryList.Count; i++)
            {
                for (int j = i + 1; j < highscoreEntryList.Count; j++)
                {
                    if (highscoreEntryList[j].score > highscoreEntryList[i].score)
                    {
                        HighscoreEntry tmp = highscoreEntryList[i];
                        highscoreEntryList[i] = highscoreEntryList[j];
                        highscoreEntryList[j] = tmp;
                    }
                }
            }
        }
        public bool NewHighScore(int i)
        {
            return ((i > highscoreEntryList[highscoreEntryList.Count - 1].score) || (highscoreEntryList.Count < 10));
        }
    }
    //stores highscore entries
    [System.Serializable]
    private class HighscoreEntry
    {
        public int score;
        public string player1Name;
        public string player2Name;

    }
}
