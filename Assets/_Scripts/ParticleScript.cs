using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ParticleScript : MonoBehaviour
{
    [SerializeField] private float scale;
    [SerializeField] private float startScaleTimer;
    [SerializeField] private float destrTimer;
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        if (startScaleTimer > 0)
        {
            transform.DOScale(scale, startScaleTimer);
        }
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
        if (startScaleTimer > 0)
        {
            transform.DOScale(0, startScaleTimer/2).OnComplete(() => Destroy(gameObject));
        }
        else
        {
            transform.DOScale(0, 0.25f).OnComplete(() => Destroy(gameObject));
        }
    }

    public void SetTarget (Transform _target)
    {
        target = _target;
    }
}
