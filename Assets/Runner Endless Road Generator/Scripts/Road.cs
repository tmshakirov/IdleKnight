using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public int number;
    public bool cantspawn;
    [SerializeField] private MeshFilter MR;
    [SerializeField] private Mesh lastRoad;
    [SerializeField] private SpawnScript spawner;
    [SerializeField] private List<GameObject> enemies;

    private void Start()
    {
        spawner = transform.GetComponentInChildren<SpawnScript>();
    }

    public void AddEnemy (GameObject _e)
    {
        enemies.Add(_e);
    }

    public void Reset()
    {
        foreach (var e in enemies)
        {
            if (e != null)
                e.transform.GetComponentInChildren<EnemyScript>().Destruction();
        }
        enemies.Clear();
        spawner.Reset();
    }

    public void LastSet (Vector3 _pos)
    {
        transform.position = _pos;
        Reset();
    }

    public void LastSpawn()
    {
        MR = GetComponent<MeshFilter>();
        MR.mesh = lastRoad;
        spawner.LastSpawn();
    }

    public void Spawn()
    {
        if (!cantspawn)
            spawner.Spawn();
        else
        {
            spawner.Spawn(4);
            cantspawn = false;
        }
    }
}