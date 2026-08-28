using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private bool isLoading;

    [SerializeField] private LoadingScreen loadingScreen;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        if (isLoading) return;

        isLoading = true;
        loadingScreen.Show(() =>
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        });
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.SetProgress(0f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            float progress = operation.progress / 0.9f;
            loadingScreen.SetProgress(progress);
            yield return null;
        }
        loadingScreen.SetProgress(1f);

        operation.allowSceneActivation = true;
        yield return operation;

        loadingScreen.Hide(() => isLoading = false);
    }
}
