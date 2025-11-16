using UnityEngine;

public class PlayerFleshlight : MonoBehaviour
{
    public GameObject spotlight; // объект spotlight, висит на игроке

    private void Start()
    {
        // по умолчанию выключен
        if (spotlight != null)
            spotlight.SetActive(false);
    }

    // Метод для включения spotlight, вызываемый после получения фонарика
    public void EnableSpotlight()
    {
        if (spotlight != null)
            spotlight.SetActive(true);
    }
}
