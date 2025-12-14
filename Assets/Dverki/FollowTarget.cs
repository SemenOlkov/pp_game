using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    // Объект, за которым будем следовать
    public Transform targetToFollow;

    // Вариант выбора за кем следовать (можно выбрать в инспекторе)
    public enum FollowTargetOption
    {
        Player,
        AnotherObject
        // Можно добавить еще варианты
    }

    public FollowTargetOption followOption = FollowTargetOption.Player;

    // Если нужно выбрать конкретный объект в редакторе
    public Transform customTarget;

    private Transform currentTarget;

    void Start()
    {
        // Изначально выбираем цель
        UpdateTarget();
    }

    void Update()
    {
        // Обновляем цель, если меняется выбор
        UpdateTarget();

        // Следуем за выбранной целью
        if (currentTarget != null)
        {
            transform.position = currentTarget.position;
        }
    }

    void UpdateTarget()
    {
        switch (followOption)
        {
            case FollowTargetOption.Player:
                // Предполагается, что у игрока есть тег "Player"
                if (GameObject.FindGameObjectWithTag("Player") != null)
                {
                    currentTarget = GameObject.FindGameObjectWithTag("Player").transform;
                }
                break;

            case FollowTargetOption.AnotherObject:
                if (customTarget != null)
                {
                    currentTarget = customTarget;
                }
                break;

                // Можно добавить еще варианты
        }
    }
}