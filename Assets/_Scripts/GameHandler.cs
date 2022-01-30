using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : Singleton<GameHandler>
{
    public float moveSpeed = 1.5f;
    public GameObject road;
    public Transform spawner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Spawn()
    {
        Instantiate(road, spawner.transform.position, road.transform.rotation);
    }
}
