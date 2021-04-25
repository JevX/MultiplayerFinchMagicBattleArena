using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Display_StageInfo : MonoBehaviour
{
    public Text stageText;
    public Text totalScore;
    public Text totalTime;
    
    public void SetStageInfoText(string stageText, int totalScore, int lastScore)
    {
        this.stageText.text = stageText;
        this.totalScore.text = totalScore.ToString();
        this.totalTime.text = '+'+lastScore.ToString();
    }
}
