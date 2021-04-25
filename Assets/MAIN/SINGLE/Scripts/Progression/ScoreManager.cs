using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Finch;

public class ScoreManager : MMSingleton<ScoreManager>
{
    public UI_Display_StageInfo stageInfo;
    public UI_Display_StageTimer stageTimer;
    public Text StageText;
    public Text TimeText;
    public Text TargetsText;
    public Text ScoreText;
    public Text LastScore;
    public Text NewRecord;

    public Animator animator;

    public float AverageStageTime = 30f;
    public int MaxPointsForAverageTime = 300;
    public int PointsForTarget = 40;

    public ParticleSystem[] confetti;
    public Transform confettiContainer;

    public GameObject panelScoreManager;

    [FMODUnity.EventRef]
    public string FMODEvent;


    float TotalTimeInGameSession;
    int TotalScoreInGameSession;
    int TotalTargetsInGameSession;

    public bool GameEnded;

    int lastStageScore;

    NodeType Node;

    private void Start()
    {
        DOTween.Init();
        TotalScoreInGameSession = 0;
        TotalTimeInGameSession = 0;
        TotalTargetsInGameSession = 0;
        GameEnded = false;
    }

    private void HideAllPanels()
    {
        panelScoreManager.SetActive(false);
    }

    /// <summary>
    /// Получаем статистику текущей стадии. Вообще это черновой скрипт и его надо переделать на ивент.
    /// И по хорошему по результату ивента запрашивать у менеджера прогрессии результаты самостоятельно.
    /// </summary>
    /// <param name="stage"></param>
    /// <param name="time"></param>
    /// <param name="targets"></param>
    /// <param name="showTimer"></param>
    public void ShowStageResults(string stage, float time, int targets)
    {
        HideAllPanels();
        stageTimer.StopTimer();
        panelScoreManager.SetActive(true);
        transform.position = new Vector3(PlacementController.Instance.transform.localPosition.x, Camera.main.transform.position.y, PlacementController.Instance.transform.localPosition.z);
        transform.LookAt(Camera.main.transform);
        transform.localRotation *= Quaternion.Euler(0, 180, 0);

        confettiContainer.transform.position = transform.position + Camera.main.transform.forward * 2f;
        confettiContainer.transform.LookAt(Camera.main.transform);
        confettiContainer.transform.localScale = Vector3.one;//transform.localScale;

        FMODUnity.RuntimeManager.PlayOneShot(FMODEvent, confettiContainer.transform.position);

        foreach (ParticleSystem system in confetti)
        {
            system.Play();
        }

        if (stage != "Tutorial")
        {
            int stageScore = Mathf.FloorToInt((1 / (time / AverageStageTime)) * MaxPointsForAverageTime) + PointsForTarget * targets;

            TotalScoreInGameSession += stageScore;
            TotalTimeInGameSession += time;
            TotalTargetsInGameSession += targets;

            if (stage == "Final Stage")
            {
                int lastScore = PlayerPrefs.GetInt("TotalScore", -99);

                if (TotalScoreInGameSession > lastScore)
                {
                    NewRecord.gameObject.SetActive(true);
                    PlayerPrefs.SetInt("TotalScore", stageScore);
                }
                else
                {
                    NewRecord.gameObject.SetActive(false);
                }

                LastScore.text = lastScore > 0 ? lastScore.ToString() : "N/A";

                ScoreText.text = TotalScoreInGameSession.ToString();
                TimeText.text = MMTime.FloatToTimeString(TotalTimeInGameSession);
                TargetsText.text = TotalTargetsInGameSession.ToString();

                animator.Play("OpenScore");

                GameEnded = true;
            }

            lastStageScore = stageScore;
            UpdateStageInfo(stage);
        }
    }

    public void UpdateStageInfo(string stage)
    {
        stageInfo.SetStageInfoText(stage, TotalScoreInGameSession, lastStageScore);
    }

    public void Update()
    {
        if (GameEnded)
        {
            if (Finch.FinchInput.GetTouchpadEvent(Node, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeLeft))
            {
                ResetGame();
            }


            if (Finch.FinchInput.GetTouchpadEvent(Node, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeRight))
            {

            }
        }
    }


    public void EndStage()
    {
         stageTimer.ResetTimer();
        stageTimer.StartTimer();
    }

    public void CloseStageResults()
    {
        animator.Play("CloseScore");        
    }

    public void ResetGame()
    {
        EndStage();
        GameEnded = false;
        CloseStageResults();
        TotalScoreInGameSession = 0;
        TotalTimeInGameSession = 0;
        TotalTargetsInGameSession = 0;
        lastStageScore = 0;
        ProgressionManager.Instance.ResetGame();
    }
}
