using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class ScoreBoard : MonoBehaviour
{

    public int TotalScore {get;set;}
   
   public void increaseScore(int increase)
    {
        TotalScore += increase;
    }
}

