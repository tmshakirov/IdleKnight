using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpawnScript : MonoBehaviour
{
    [SerializeField] private bool spawned;
    [SerializeField] private List<SpawnInstance> spawnInstances;
    [SerializeField] private GameObject border, borderStop, boss;

    private int spawnChance;
    private bool prevEnemy;

    public void Reset()
    {
        if (spawned)
        {
            spawned = false;
        }
        foreach (Transform child in transform)
        {
            if (child.gameObject != null)
                Destroy(child.gameObject);
        }
    }

    public void Spawn(int _offset = 0)
    {
        if (!spawned)
        {
            spawnChance = GetAvailableObjects()[GetAvailableObjects().Count - 1].spawnChance;
            for (int i = _offset; i < 8; i++)
            {
                RandomizeSpawn(i);
            }
            spawned = true;
        }
    }

    public void LastSpawn()
    {
        spawnChance = GetAvailableObjects()[GetAvailableObjects().Count - 1].spawnChance;
        for (int i = 0; i < 4; i++)
        {
            RandomizeSpawn(i);
        }
        Instantiate(border, transform.position + new Vector3 (0, 0, 9), Quaternion.identity).transform.SetParent (transform);
        Instantiate(borderStop, transform.position + new Vector3 (0, 0, 18.25f), Quaternion.identity).transform.SetParent(transform);
        var b = Instantiate(boss, new Vector3 (transform.position.x, PlayerController.Instance.transform.position.y, transform.position.z) + new Vector3(0, 0, 18.75f), boss.transform.rotation);
        b.transform.SetParent(transform);
        PlayerController.Instance.SetMaxDistance(b.transform);
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
                    prevEnemy = false;
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
