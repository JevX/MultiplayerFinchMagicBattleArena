using System;
using System.Collections;
using UnityEngine;

namespace Main.Scripts
{
    public class SceneLoadManager : MonoBehaviour
    {
        [SerializeField] private string nameSceneLoad = null;

        public static SceneLoadManager Instance = null;
        
        private string sceneNameToBeLoaded;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(string _sceneName)
        {
            sceneNameToBeLoaded = _sceneName;
            StartCoroutine(InitializeSceneLoading());
        }



        IEnumerator InitializeSceneLoading()
        {
            //First, we load the Loading scene
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nameSceneLoad);

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
}
