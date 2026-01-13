using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Игра закрывается...");

        // Закрывает скомпилированное приложение
        Application.Quit();

        // Останавливает режим игры в UNITY EDITOR
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}