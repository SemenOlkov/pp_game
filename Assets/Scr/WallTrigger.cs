using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    public GameObject screamerUI; // Панель со скримером
    public Animator screamerAnimator; // Аниматор монстра

    private bool isScreaming = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isScreaming)
        {
            StartScream();
        }
    }

    public void StartScream()
    {
        if (isScreaming) return; // Уже идет
        isScreaming = true;

        // Включаем UI скримера
        screamerUI.SetActive(true);

        // Запускаем анимацию
        screamerAnimator.SetTrigger("StartScream");
    }

    // Можно добавить метод для отключения, который вызывается по событию в анимации
    public void StopScream()
    {
        // Отключаем UI
        screamerUI.SetActive(false);
        isScreaming = false;
    }
}