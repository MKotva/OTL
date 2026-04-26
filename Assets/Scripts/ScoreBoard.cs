using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class ScoreBoard : MonoBehaviour
{

    public TMP_Text scoreBoard;
    public int TotalScore {get;set;}
   
   public void increaseScore(int increase)
    {
        TotalScore += increase;
        scoreBoard.text = TotalScore.ToString();
    }
}

