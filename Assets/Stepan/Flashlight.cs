using UnityEngine;

public class Flashlight : MonoBehaviour, IInteractable
{
    public string itemName = "Flashlight"; // Название предмета
    public GameObject flashlightObject; // Объект фонарика у игрока
    public GameObject flashlightInWorld; // Объект фонарика в мире

    public void Interact()
    {
        // Добавляем в инвентарь
        PlayerInventory.Instance.AddItem(itemName);

        // Убираем/скрываем объект в мире
        if (flashlightInWorld != null)
            flashlightInWorld.SetActive(false);

        // Включаем фонарик у игрока
        if (flashlightObject != null)
            flashlightObject.SetActive(true);

        // Находим игрок и включаем spotlight
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerSpotlightController spotlightController = player.GetComponent<PlayerSpotlightController>();
            if (spotlightController != null)
            {
                spotlightController.EnableSpotlight();
            }
        }

        // Деактивируем сам предмет
        gameObject.SetActive(false);
    }

    public string GetDescription()
    {
        return "Press [E] to pick up the flashlight.";
    }
}