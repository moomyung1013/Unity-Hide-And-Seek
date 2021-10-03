using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LaserWand : MonoBehaviourPunCallbacks
{
    public Transform laserPos;
    public GameObject Raybody; //쏘는 위치
    public GameObject ScaleDistance;

    void Update()
    {
        if (!photonView.IsMine) return;
        if (Input.GetMouseButtonDown(0))
        {
            photonView.RPC("Laser", RpcTarget.Others, null);
            Laser();
        }
    }

    [PunRPC]
    void Laser()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit, 200);
        ScaleDistance.SetActive(true);
        ScaleDistance.transform.localScale = new Vector3(1, 1, 30);
        Invoke("test", 1.0f);
        //Instantiate(laser, laserPos.position, laserPos.rotation);
    }
    void test()
    {
        ScaleDistance.SetActive(false);
        ScaleDistance.transform.localScale = new Vector3(1, 1, 1);
    }
}
