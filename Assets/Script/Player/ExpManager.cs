using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpManager
{
    public int level { private set; get; } = 1;
    public int exp { private set; get; } = 0;
    public int requireExp { private set; get; } = 5;

    public void GainExp(int gainedExp)
    {
        if (gainedExp < 0)
        {
            return;
        }
        exp += gainedExp;

        CheckLevelUp();
        Debug.Log(string.Concat("level", level));
        Debug.Log(string.Concat("exp", exp));
    }

    private void CheckLevelUp()
    {
        if (exp < requireExp)
        {
            return;
        }
        exp -= requireExp;
        level++;
        requireExp = CalcRequireExp(level);

        CheckLevelUp();
    }

    private int CalcRequireExp(int level)
    {
        return (int)(Mathf.Ceil(level * level / 2))  + 5;
    }
}
