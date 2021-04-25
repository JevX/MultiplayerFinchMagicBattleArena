using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    public string sceneNameForLoad;

    public GameObject loadingScreen;
    public Slider slider;
    //public Text progressText;


    public void LoadSceneGame()
    {
       StartCoroutine(LoadAsynch());
    }

    IEnumerator LoadAsynch()
    {
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneNameForLoad);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            loadingScreen.SetActive(true);

            slider.value = progress;
            //progressText.text = (int)(progress * 100f) + "%";

            yield return null;
        }
    }
}
