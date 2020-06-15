using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private int maximumEnemiesToSpawn = 3;

    [SerializeField]
    private float timeToWaitBeforeSpawn = 3.5f;

    [SerializeField]
    private GameObject enemyPrefab;

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        for (int i = 0; i < maximumEnemiesToSpawn; i++)
        {
            yield return new WaitForSeconds(timeToWaitBeforeSpawn);
            Instantiate(enemyPrefab, transform.position, transform.rotation);
            EnemyManager.Instance.IncreaseEnemiesCounter();
        }
    }
}
