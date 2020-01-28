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
        CreateAchievement("Achievement", "Cow Grabber" ,"Collected 20 Cows");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>().GetScore() >= 2)
        {
            EarnAchievement("Cow Grabber");
        }
    }

    public void EarnAchievement(string title)
    {
        if (achievements[title].EarnAchievement())
        {
            GameObject achievement = (GameObject)Instantiate(visualAchievement);
            SetAchievementInfo("Achievement", title, achievement);
            StartCoroutine(HideAchievement(achievement));
        }
    }

    public IEnumerator HideAchievement(GameObject achievement)
    {
        yield return new WaitForSeconds(3);
        Destroy(achievement);
    }

    public void CreateAchievement(string parent, string title, string description)
    {
        GameObject achievement = (GameObject)Instantiate(achievementPrefab);
        SC_Achievement newAchievement = new SC_Achievement(name, description, achievement);
        achievements.Add(title, newAchievement);
        achievement.SetActive(true);

        SetAchievementInfo(parent, title, achievement);
    }

    public void SetAchievementInfo(string parent, string title, GameObject achievement)
    {
        achievement.transform.SetParent(GameObject.Find(parent).transform);
        achievement.transform.localPosition = new Vector3(0, 0, 0);
        achievement.transform.localScale = new Vector3(1, 1, 1);
        achievement.transform.GetComponent<Text>().text = "Achievement Unlocked: " + achievements[title].Description;
    }
}
