using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ParticleScript : MonoBehaviour
{
    [SerializeField] private float destrTimer;
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Destroy", destrTimer);
    }

    private void Update()
    {
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
        }
    }

    private void Destroy()
    {
        transform.DOScale(0, 0.25f).OnComplete(() => Destroy(gameObject));
    }

    public void SetTarget (Transform _target)
    {
        target = _target;
    }
}
