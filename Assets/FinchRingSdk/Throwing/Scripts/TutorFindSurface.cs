using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorFindSurface : MonoBehaviour 
{
    /// <summary>
    /// The raw image where the video will be played.
    /// </summary>
    public RawImage RawImage;

    /// <summary>
    /// The video player component to be played.
    /// </summary>
    public VideoPlayer VideoPlayer;

    public bool IsStoped = false;

    public bool ignoreArmsCount;

    private Texture m_RawImageTexture;
    public static System.Action searchFinish;
    public Text text;
    private float timer = 0;

    public void Start()
    {
        text.gameObject.SetActive(false);
        VideoPlayer.enabled = false;
        m_RawImageTexture = RawImage.texture;
        VideoPlayer.prepareCompleted += _PrepareCompleted;

        
        Finch.FinchCalibration.OnCalibrationEnd += OnCalibrationFinish;

    }

    void OnCalibrationFinish()
    {
        Play();
        Finch.FinchCalibration.OnCalibrationEnd -= OnCalibrationFinish;
    }

    /// <summary>
    /// Stop video
    /// </summary>
    public void Stop()
    {
        searchFinish?.Invoke();
        VideoPlayer.Stop();
        RawImage.texture = m_RawImageTexture;
        VideoPlayer.enabled = false;
        text.gameObject.SetActive(false);
        IsStoped = true;
    }

    /// <summary>
    /// Play video
    /// </summary>
    public void Play()
    {
        RawImage.enabled = true;

        VideoPlayer.Play();
        text.gameObject.SetActive(true);
        VideoPlayer.enabled = true;
    }

    public void Update()
    {
        if (Finch.FinchNodeManager.GetUpperArmCount() != 0 && !ignoreArmsCount)
            Stop();

        if (text.gameObject.activeInHierarchy)
        {
            Object[] objs = Object.FindObjectsOfType(typeof(DebugPlanePrefab));
            DebugPlanePrefab founded_plane = null;
            if (objs.Length > 0)
            {
                founded_plane = objs[0] as DebugPlanePrefab;
            }
            timer += Time.deltaTime;

            if(timer > 3f && founded_plane != null)
            {
                Stop();
            }
        }
    }

    private void _PrepareCompleted(VideoPlayer player)
    {
        RawImage.texture = player.texture;
    }
}

