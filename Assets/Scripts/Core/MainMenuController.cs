using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string gameplaySceneName = "Gameplay";

    [Header("Intro")]
    [SerializeField] private GameObject menuOptions;
    [SerializeField, Min(0f)] private float introDuration = 1.25f;

    private IEnumerator Start()
    {
        menuOptions.SetActive(false);
        yield return new WaitForSecondsRealtime(introDuration);
        menuOptions.SetActive(true);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
