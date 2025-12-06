using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera cameraToKeep; // Камера, которую оставить
    public string playerTag = "Player"; // Тег объекта игрока

    void Start()
    {
        // Если не указана, ищем основную
        if (cameraToKeep == null)
        {
            cameraToKeep = Camera.main;
        }

        if (cameraToKeep == null)
        {
            Debug.LogWarning("Камера для сохранения не назначена и Main Camera не найдена.");
            return;
        }

        // Получаем все камеры
        Camera[] allCameras = Camera.allCameras;

        foreach (Camera cam in allCameras)
        {
            if (cam != cameraToKeep)
            {
                // Отключаем камеру
                cam.gameObject.SetActive(false);
            }
        }

        // Активируем нужную камеру, если она не активна
        if (!cameraToKeep.gameObject.activeInHierarchy)
        {
            cameraToKeep.gameObject.SetActive(true);
        }

        // Можно отключить объект с тегом Player, если нужно
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            // Отключаем объект игрока полностью
            playerObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Объект с тегом 'Player' не найден");
        }
    }
}