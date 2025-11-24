using UnityEngine;
using System.Collections;

public class Trigger1Spawner : MonoBehaviour
{
    public GameObject monster; // Монстр, изначально скрыт
    public Light spotlight;    // Мигающий свет
    public AudioSource roarSound; // Звук рыка

    private bool monsterSpawned = false;

    public float spawnDelay = 0.5f;
    public float glowDuration = 2f;
    public float glowInterval = 0.2f;

    private void Start()
    {
        // Монстр изначально скрыт
        if (monster != null)
            monster.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !monsterSpawned)
        {
            StartCoroutine(SpawnMonster());
        }
    }

    private IEnumerator SpawnMonster()
    {
        yield return new WaitForSeconds(spawnDelay);

        if (monster != null)
        {
            monster.SetActive(true);
            monsterSpawned = true;

            // Воспроизведение рыка
            if (roarSound != null)
            {
                roarSound.Play();
            }

            // Мигающий свет (опционально)
            if (spotlight != null)
            {
                StartCoroutine(MakeLightBlink());
            }

            // Через 2 сек исчезает
            yield return new WaitForSeconds(glowDuration);
            if (spotlight != null)
            {
                StopCoroutine("MakeLightBlink");
                spotlight.enabled = false;
            }
            monster.SetActive(false);
        }
    }

    private IEnumerator MakeLightBlink()
    {
        while (true)
        {
            spotlight.enabled = !spotlight.enabled;
            yield return new WaitForSeconds(glowInterval);
        }
    }
}