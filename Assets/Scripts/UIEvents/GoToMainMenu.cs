using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMainMenu : MonoBehaviour
{
    public void OnButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}