using UnityEngine;
using UnityEngine.SceneManagement;

public class StartNewGame : MonoBehaviour
{
    void OnEnable()
    {
        // Срабатывает при активации объекта или загрузке сцены
        SetCursorState(true);
    }

    void Update()
    {
        // Если что-то извне скрыло курсор — возвращаем его
        if (Cursor.visible == false) 
        {
            SetCursorState(true);
        }
    }

    private void SetCursorState(bool isMenu)
    {
        Cursor.lockState = isMenu ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isMenu;
    }

    public void StartNewGameButton()
    {
        SetCursorState(false);
        SceneManager.LoadScene("Scene_1");
    }
}