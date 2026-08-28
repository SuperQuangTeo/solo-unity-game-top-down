using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string firstSceneName;

    private void Start()
    {
        SceneLoader.Instance.LoadScene(firstSceneName);
    }
}
