using UnityEngine;

public class FlashPass : MonoBehaviour
{
    private void Awake()
    {
        // Как только фонарик появился в сцене, он сообщает о себе менеджеру
        if (FlashLightManager.Instance != null)
        {
            FlashLightManager.Instance.RegisterFlashlight(this.gameObject);
        }
    }
}