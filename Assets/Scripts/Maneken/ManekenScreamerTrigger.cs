using UnityEngine;
using UnityEngine.SceneManagement;

public class ManekenScreamerTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
{
    // Проверяем, вошел ли в триггер игрок (по тегу)
    if (other.CompareTag("Player"))
    {
        SceneManager.LoadScene("Dead");
        enabled = false; // Отключаем скрипт после срабатывания
    }
}
}