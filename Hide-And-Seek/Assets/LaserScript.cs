using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaserScript : MonoBehaviour
{
    public float speed = 20.0f;
    private Transform tr;

    private void Start()
    {
        tr = GetComponent<Transform>();
    }
    void Update()
    {
        tr.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
    }
}
