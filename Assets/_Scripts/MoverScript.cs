using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverScript : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Vector3.forward * GameHandler.Instance.moveSpeed * Time.deltaTime);
    }
}
