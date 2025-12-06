using UnityEngine;
using UnityEngine.SceneManagement;

public class LookAtObjectToChangeScene : MonoBehaviour
{
    public Camera playerCamera; // Камера игрока
    public string sceneName; // Название сцены для загрузки
    public float maxDistance = 10f; // Расстояние проверки

    private bool hasChangedScene = false;

    void Update()
    {
        if (hasChangedScene) return; // Чтобы переключение было однократным

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("SceneTrigger")) // Объект, на который смотрим
            {


                // Переход на другую сцену
                ChangeScene();
            }
        }
    }

    public void ChangeScene()
    {
        hasChangedScene = true;
        SceneManager.LoadScene(sceneName);
    }
}