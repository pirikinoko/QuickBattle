using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Button : MonoBehaviour
{

    public void ATK()
    {
        Battle.turnOver = false;
        Battle.act = "ATK";
        Battle.phase++;
    }
    public void DEF()
    {
        Battle.turnOver = false;
        Battle.act = "DEF";
        Battle.phase++;
    }
    public void SKL()
    {
        Battle.turnOver = false;
        Battle.act = "SKL";
        Battle.phase = 2;
    }
    public void Next()
    {
        if(Battle.phase == 6) { Battle.phase = 1; }
        else { Battle.phase++; }   
        Battle.turnOver = false;
    }
    public void Back()
    {
        Battle.act = "Back";
        Battle.turnOver = false;
    }
    public void Retry()
    {
        SceneManager.LoadScene("Main");
    }

    public void UseSkill1()
    {
        Battle.skillClicked = 0; 
    }
    public void UseSkill2()
    {
        Battle.skillClicked = 1;
    }
    public void UseSkill3()
    {
        Battle.skillClicked = 2;
    }
    public void UseSkill4()
    {
        Battle.skillClicked = 3;
    }
    public void UseSkill5()
    {
        Battle.skillClicked = 4;
    }
    public void UseSkill6()
    {
        Battle.skillClicked = 5;
    }

    public void Reward1()
    {
        Battle.rewardSelect = 0;
        Battle.phase++;
        SoundEffect.SETrigger[8] = true;
    }
    public void Reward2()
    {
        Battle.rewardSelect = 1;
        Battle.phase++;
        SoundEffect.SETrigger[8] = true;
    }
    public void Reward3()
    {
        Battle.rewardSelect = 2;
        Battle.phase++;
        SoundEffect.SETrigger[8] = true;
    }
}
