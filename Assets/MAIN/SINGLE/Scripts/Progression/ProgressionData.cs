using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Progression Data", menuName = "Progression/Progression Data", order = 1)]
public class ProgressionData : ScriptableObject
{
    [SerializeField] private List<StageData> stageData;

    [Serializable]
    public struct StageData
    {
        [SerializeField]
        public string stageName;
        [SerializeField]
        public int standartTargets;
        [SerializeField]
        public int minimumHealth;
        [SerializeField]
        public int maximumHealth;
        [SerializeField]
        [Range(0,1)]
        public float chanceToHaveAShield;
        [SerializeField]
        public int specialTargets;
        [SerializeField]
        public int specialTargetMinimumHealth;
        [SerializeField]
        public int specialTargetMaximumHealth;
    }

    public StageData GetStageSettings(int stage)
    {
        stage = Mathf.Clamp(stage - 1, 0, stageData.Count - 1);
        return stageData[stage];
    }
}
