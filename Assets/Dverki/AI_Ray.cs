using UnityEngine;
using UnityEngine.AI; // Добавьте пространство имен для NavMeshAgent

public class AI_Ray : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;

    void Start()
    {
        // Находим игрока по тегу "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // Получаем компонент NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player != null && agent != null)
        {
            // Устанавливаем цель для преследования
            agent.SetDestination(player.position);
        }
    }
}