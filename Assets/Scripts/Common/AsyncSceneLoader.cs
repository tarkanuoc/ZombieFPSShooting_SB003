using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncSceneLoader : MonoBehaviour
{
    [SerializeField] private ProgressBar progressBar;
    public string sceneName;
    public float fakeDuration;

    private AsyncOperation loadingOperation;
    private float startTime;

    public void StartLoadScene()
    {
        GameManager.Instance.IsGameReady = false;
        gameObject.SetActive(true);
        DontDestroyOnLoad(this);
        startTime = Time.unscaledTime;
        loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (loadingOperation == null) return;
        float fakeProgress = (Time.unscaledTime - startTime) / fakeDuration;

        float finalProgress = Mathf.Min(fakeProgress, loadingOperation.progress);
        progressBar.SetProgressValue(finalProgress);

        if (loadingOperation.isDone && finalProgress >= 1f)
        {
            FinishLoading();
        }
    }

    private void FinishLoading()
    { 
        Time.timeScale = 1;
        Destroy(gameObject);
        GameManager.Instance.IsGameReady = true;
    }
}
