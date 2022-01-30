using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public int number;
    public bool cantspawn;
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
            Destroy (e.gameObject);
        }
        spawner.Reset();
    }

    public void Spawn()
    {
        if (!cantspawn)
            spawner.Spawn();
        else
            cantspawn = false;
    }
}