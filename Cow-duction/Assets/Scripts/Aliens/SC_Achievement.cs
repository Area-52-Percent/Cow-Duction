using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Achievement : MonoBehaviour
{
    private string achname;
    private string description;
    private bool unlocked;
    private GameObject achievementRef;

    public SC_Achievement(string name, string description, GameObject achievementRef)
    {
        this.Name = name;
        this.Description = description;
        this.AchievementRef = achievementRef;
    }

    public bool Unlocked { get => unlocked; set => unlocked = value; }
    public string Description { get => description; set => description = value; }
    public string Name { get => achname; set => achname = value; }
    public GameObject AchievementRef { get => achievementRef; set => achievementRef = value; }

    public bool EarnAchievement()
    {
        if (!Unlocked)
        {
            Unlocked = true;
            return true;
        }
        return false;
    }

    /*
    void Start()
    {
        SC_Achievement achievement = new SC_Achievement(achname, achdescription, achunlocked, achRef);
    }
    */

    // Update is called once per frame
    void Update()
    {
        
    }
}
