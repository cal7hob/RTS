using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public static TeamManager instance;

    public User curUser;

    void Awake()
    {
        instance = this;
    }


}

[System.Serializable]
public class User
{
    public Team team;
    public string name;
    public Color teamColor;
}

[System.Serializable]
public class Team
{
    public enum TeamType
    {
        None,
        Romans,
        Goths
    }

    public TeamType teamType;
}