using UnityEngine;
 using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
