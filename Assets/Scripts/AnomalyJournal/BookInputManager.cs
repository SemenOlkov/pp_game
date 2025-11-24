using UnityEngine;
using UnityEngine.InputSystem;

public class BookInputManager : MonoBehaviour
{
    [Header("Book References")]
    public GameObject BGUI;
    public GameObject HotbarPanel;
    
    private Keyboard keyboard;
    private BookItem currentBookItem;

    private void Start()
    {
        keyboard = Keyboard.current;
        
        // Изначально скрываем UI
        if (BGUI != null)
            BGUI.SetActive(false);
    }

    private void Update()
    {
        if (BGUI != null && BGUI.activeSelf && keyboard != null && keyboard.fKey.wasPressedThisFrame)
        {
            CloseBookUI();
        }
    }

    public void OpenBookUI(BookItem bookItem)
    {
        currentBookItem = bookItem;
        
        if (BGUI != null)
        {
            BGUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (HotbarPanel != null)
                HotbarPanel.SetActive(false);
        }
    }

    private void CloseBookUI()
    {
        if (BGUI != null)
        {
            BGUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (HotbarPanel != null)
                HotbarPanel.SetActive(true);
        }
    }
}