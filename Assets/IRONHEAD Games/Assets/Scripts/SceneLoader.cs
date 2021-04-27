﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private string sceneNameToBeLoaded;

    public void LoadScene(string _sceneName)
    {
        sceneNameToBeLoaded = _sceneName;

        StartCoroutine(InitializeSceneLoading());
    }

    IEnumerator InitializeSceneLoading()
    {
        //First, we load the Loading scene
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("LoadingScene");

        //Load the actual scene
        StartCoroutine(LoadActualyScene());
    }

    IEnumerator LoadActualyScene()
    {
        var asyncSceneLoading = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneNameToBeLoaded);

        //this value stops the scene from displaying when it is still loading...
        asyncSceneLoading.allowSceneActivation = false;

        while (!asyncSceneLoading.isDone)
        {
            Debug.Log(asyncSceneLoading.progress);

            if (asyncSceneLoading.progress >= 0.9f )
            {
                //Finally, show the scene.
                asyncSceneLoading.allowSceneActivation = true; 
            }

            yield return null;
        }
    }
}
