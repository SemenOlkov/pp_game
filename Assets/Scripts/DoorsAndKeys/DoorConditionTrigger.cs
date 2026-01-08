using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DoorConditionTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<LinkedDoor> doorsToCheck = new List<LinkedDoor>();

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что вошел именно игрок
        if (other.CompareTag("Player"))
        {
            CheckCondition();
        }
    }

    private void CheckCondition()
    {
        bool anyDoorClosed = false;

        // Проверяем состояние каждой двери в списке
        foreach (LinkedDoor door in doorsToCheck)
        {
            if (door != null && !door.isOpened)
            {
                anyDoorClosed = true;
                break; // Если нашли хоть одну закрытую, дальше можно не проверять
            }
        }

        if (anyDoorClosed)
        {
            // Если хотя бы одна закрыта — смерть
            Debug.Log("Триггер: Есть закрытые двери! ExecuteDeath.");
            ExecuteDeath();
        }
        else
        {
            // Если все открыты — деактивируем триггер
            Debug.Log("Триггер: Все двери открыты. Триггер отключен.");
            gameObject.SetActive(false);
        }
    }

    private void ExecuteDeath()
    {
        if (PersistentObject.Instance != null)
        {
            Destroy(PersistentObject.Instance.gameObject);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Dead");
    }
}