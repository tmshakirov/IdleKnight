using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    [SerializeField] private bool spawned;
    [SerializeField] private CoinScript coin;
    [SerializeField] private GameObject enemy;

    private int spawnChance;
    private bool prevEnemy;

    public void Reset()
    {
        if (spawned)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            spawned = false;
        }
    }

    public void Spawn()
    {
        if (!spawned)
        {
            spawned = true;
        }
        for (int i = 0; i < 12; i++)
        {
            spawnChance = Random.Range(1, 101);
            if (spawnChance <= 40 && !prevEnemy)
            {
                var e = Instantiate(enemy, transform.position - new Vector3(0, 0, 3.5f) + new Vector3(Random.Range(-1.5f, 1.5f), -0.2f, i * 2), enemy.transform.rotation);
                e.GetComponentInChildren<EnemyScript>().Init();
                transform.parent.GetComponent<Road>().AddEnemy(e.gameObject);
                prevEnemy = true;
            }
            else
            {
                Instantiate(coin, transform.position - new Vector3(0, 0, 3.5f) + new Vector3(Random.Range(-1.5f, 1.5f), 0, i * 2), coin.transform.rotation).transform.SetParent(transform);
                prevEnemy = false;
            }
        }
    }
}
