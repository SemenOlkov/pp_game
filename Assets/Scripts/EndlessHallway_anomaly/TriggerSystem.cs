using UnityEngine;

public class TriggerSystem : MonoBehaviour
{
    [Header("Trigger References")]
    public GameObject firstTrigger;
    public GameObject secondTrigger;
    public GameObject thirdTrigger;

    [Header("Teleport Position")]
    public Vector3 firstTriggerPosition { get; private set; }

    void Start()
    {
        // Сохраняем позицию первого триггера ДО того как он может деактивироваться
        firstTriggerPosition = firstTrigger.transform.position;
        
        // Настраиваем начальное состояние
        firstTrigger.SetActive(true);
        secondTrigger.SetActive(false);
        thirdTrigger.SetActive(false);

        Debug.Log("Позиция первого триггера сохранена: " + firstTriggerPosition);
    }
}