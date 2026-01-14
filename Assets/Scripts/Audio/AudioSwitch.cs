using UnityEngine;

public class AudioSwitcherTrigger : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource firstSource;  // Тот, что работает изначально
    [SerializeField] private AudioSource secondSource; // Тот, что включается в триггере

    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";

    private void Start()
    {
        // Убедимся, что на старте все в исходном состоянии
        if (firstSource != null && !firstSource.isPlaying) firstSource.Play();
        if (secondSource != null) secondSource.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Если в триггер вошел игрок
        if (other.CompareTag(playerTag))
        {
            SwitchAudio(toSecond: true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Если игрок вышел из триггера
        if (other.CompareTag(playerTag))
        {
            SwitchAudio(toSecond: false);
        }
    }

    private void SwitchAudio(bool toSecond)
    {
        if (firstSource == null || secondSource == null) return;

        if (toSecond)
        {
            firstSource.Pause();  // Используем Pause, чтобы звук не начинался с начала при выходе
            secondSource.Play();
        }
        else
        {
            secondSource.Stop();
            firstSource.UnPause(); // Продолжаем играть первый звук
        }
    }
}