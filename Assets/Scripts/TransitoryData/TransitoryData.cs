using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "TransitoryData",
    menuName = "QuizGame/TransitoryData"
)]
public class TransitoryData : ScriptableObject
{
    public LevelPack currentLevelPack;
    public int currentQuestionIndex;

    public bool onLosingFromGame;

    public void SetOnLosingFromGame(bool value)
    {
        onLosingFromGame = value;
    }

    public void Reset()
    {
        currentLevelPack = null;
        currentQuestionIndex = 0;
        onLosingFromGame = false;
    }
}
