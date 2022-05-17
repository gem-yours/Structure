using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpManager
{

    public int level { private set; get; } = 1;
    public int exp { private set; get; } = 0;

    private static int initialRequireExp = 1;
    public int requireExp { private set; get; } = initialRequireExp;
    public delegate void OnLevelUp(int level);
    public OnLevelUp onLevelUp { set; private get; } = (int level) => { };
    public delegate void OnExpGain(int level, int exp, int requireExp);
    public OnExpGain onExpGain { set; private get; } = (int level, int exp, int requireExp) => { };

    public void GainExp(int gainedExp)
    {
        if (gainedExp < 0)
        {
            return;
        }
        exp += gainedExp;

        CheckLevelUp();
        onExpGain(level, exp, requireExp);
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
        onLevelUp(level);

        CheckLevelUp();
    }

    private int CalcRequireExp(int level)
    {
        return (int)(Mathf.Ceil(level * level / 2)) + initialRequireExp;
    }
}
