using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private int damage;
    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = GameHandler.Instance.moveSpeed * 4.1f;
        Invoke("Destruction", 7.5f);
    }

    public void SetTarget (Vector3 _target, int _damage)
    {
        transform.LookAt(_target);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, transform.eulerAngles.z);
        damage = _damage;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.back * Time.deltaTime * moveSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().GetDamage(damage, true);
            Destroy(gameObject);
        }
    }

    private void Destruction()
    {
        Destroy(gameObject);
    }
}
