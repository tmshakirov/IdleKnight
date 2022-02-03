using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    [SerializeField] private bool spawned;
    [SerializeField] private List<SpawnInstance> spawnInstances;

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

    public void Spawn(int _offset = 0)
    {
        if (!spawned)
        {
            spawned = true;
        }
        spawnChance = GetAvailableObjects()[GetAvailableObjects().Count-1].spawnChance;
        for (int i = _offset; i < 8; i++)
        {
            RandomizeSpawn(i);
        }
    }

    private List<SpawnInstance> GetAvailableObjects()
    {
        List<SpawnInstance> tmpSpawns = new List<SpawnInstance>();
        foreach (var s in spawnInstances)
        {
            if (s.IsAvailable())
                tmpSpawns.Add(s);
        }
        return tmpSpawns;
    }

    private void RandomizeSpawn(int i)
    {
        int tmp = Random.Range(0, spawnChance);
        foreach (var s in GetAvailableObjects())
        {
            if (prevEnemy)
            {
                if (s.type == SpawnType.COIN)
                {
                    InitSpawn(s, transform.position - new Vector3(0, 0, 3.5f) + new Vector3(Random.Range(-1.5f, 1.5f), 0, i * (3f + GameHandler.Instance.moveSpeed - 2.5f)));
                    return;
                }
            }
            if (tmp < s.spawnChance)
            {
                InitSpawn(s, transform.position - new Vector3(0, 0, 3.5f) + new Vector3(Random.Range(-1.5f, 1.5f), 0, i * (3f + GameHandler.Instance.moveSpeed - 2.5f)));
                return;
            }
        }
    }

    private void InitSpawn (SpawnInstance s, Vector3 _pos)
    {
        switch (s.type)
        {
            case SpawnType.COIN:
                Instantiate(s.prefab, _pos + s.offset, s.prefab.transform.rotation).transform.SetParent(transform);
                break;
            default:
                var e = Instantiate(s.prefab, _pos + s.offset, s.prefab.transform.rotation);
                e.GetComponentInChildren<EnemyScript>().Init();
                transform.parent.GetComponent<Road>().AddEnemy(e.gameObject);
                prevEnemy = true;
                break;
        }
    }
}

public enum SpawnType { COIN, ENEMY }

[System.Serializable]
public class SpawnInstance
{
    public int spawnChance;
    public int minLevel;
    public SpawnType type;
    public GameObject prefab;
    public Vector3 offset;

    public bool IsAvailable()
    {
        return UpgradeHandler.Instance.GetLevel() >= minLevel;
    }
}
