using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SC_AchievementManager : MonoBehaviour
{

    public GameObject achievementPrefab;

    public GameObject visualAchievement;

    public Dictionary<string, SC_Achievement> achievements = new Dictionary<string, SC_Achievement>();

    // Start is called before the first frame update
    void Start()
    {
        CreateAchievement("Achievement", "The First of Many" ,"You've grabbed your first cow!");
        CreateAchievement("Achievement", "An Experienced Alien", "You've reached a score of 3!");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>().GetScore() >= 1)
        {
            EarnAchievement("The First of Many");
        }
        if (GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>().GetScore() >= 3)
        {
            EarnAchievement("An Experienced Alien");
        }
    }

    public void EarnAchievement(string title)
    {
        if (achievements[title].EarnAchievement())
        {
            //GameObject achievement = (GameObject)Instantiate(visualAchievement);
            SetAchievementInfo("Achievement", title);
            visualAchievement.SetActive(true);
            StartCoroutine(HideAchievement());
            
        }
    }

    public IEnumerator HideAchievement()
    {
        yield return new WaitForSeconds(3);
        visualAchievement.SetActive(false);
    }

    public void CreateAchievement(string parent, string title, string description)
    {
        GameObject achievement = (GameObject)Instantiate(visualAchievement);
        SC_Achievement newAchievement = new SC_Achievement(name, description, achievement);
        achievements.Add(title, newAchievement);
        achievement.SetActive(true);

        //SetAchievementInfo(parent, title, achievement);
    }

    public void SetAchievementInfo(string parent, string title)
    {
        Debug.Log("Achievement Unlocked: " + achievements[title].Description);
        //achievement.transform.SetParent(GameObject.Find(parent).transform);
        //achievement.transform.localPosition = new Vector3(0, 0, 0);
        //achievement.transform.localScale = new Vector3(1, 1, 1);
        visualAchievement.SetActive(true);
        visualAchievement.GetComponent<Text>().text = "Achievement Unlocked: " + achievements[title].Description;  
    }
}
