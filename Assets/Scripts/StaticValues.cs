using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticValues
{
    public static float pacMoveTime;
    public static int levelsCompleted;
    public static int level;


    public static void ResetValues()
    {
        level = 1;
        pacMoveTime = 0.2f;
        levelsCompleted = 0;
    }

    public static string GetNextLevel()
    {
        pacMoveTime -= 0.01f;
        level++;
        levelsCompleted++;
        if (level > 3)
        {
            level = 1;
        }
        return "Level " + level + " Scene";
    }
}
