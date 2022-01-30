using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CoinScript : MonoBehaviour
{
    private Transform player;
    private bool moving;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance (transform.position, player.position) <= 1f)
        {
            if (!moving)
            {
                transform.DOMove(player.transform.position, 0.15f).OnComplete(() =>
                {
                    player.GetComponent<PlayerController>().AddCoins();
                    Destroy(gameObject);
                });
                moving = true;
            }
        }
    }
}
