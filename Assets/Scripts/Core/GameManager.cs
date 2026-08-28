using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
        Initialize();  
    }

    private void Initialize()
    {
        DOTween.Init(recycleAllByDefault: false,
            useSafeMode: true,
            logBehaviour: LogBehaviour.ErrorsOnly);

        ChangeState(GameState.MainMenu);
    }

    public void ChangeState(GameState newState)
    {
        if(newState == CurrentState)
        {
            return;
        }
        CurrentState = newState;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneLoader.Instance.LoadScene("Gameplay");
        }
    }
}

public enum GameState
{
    None,
    MainMenu,
    Playing,
    Paused,
    Gameover
}
