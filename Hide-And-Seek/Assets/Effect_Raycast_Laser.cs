using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Effect_Raycast_Laser : MonoBehaviourPunCallbacks
{
    public GameObject Raybody; //쏘는 위치
    public GameObject ScaleDistance;
    public GameObject RayResult; // 충돌 위치 출력 결과

    GameObject director, player;

    private void Start()
    {
        director = GameObject.Find("GameDirector");
        player = GameObject.Find("Player3");
    }

    void Update()
    {

        if (!photonView.IsMine) return;

        if (Input.GetMouseButtonDown(0))
        {
            photonView.RPC("Laser", RpcTarget.Others, null);
            Laser();
        }
        //ScaleDistance.transform.localScale = new Vector3(1, 1, 1);
        //RayResult.transform.position = hit.point;
        //RayResult.transform.rotation = Quaternion.LookRotation(hit.normal);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌");
    }
    [PunRPC]
    void Laser()
    {
        RaycastHit hit;
        Debug.DrawRay(Raybody.transform.position, ScaleDistance.transform.forward * 1.0f, Color.red, 3.0f);
        if (Physics.Raycast(Raybody.transform.position, ScaleDistance.transform.forward, out hit, 1.0f))
        {
            if (hit.transform.tag == "Player")
            {
                Debug.Log("플레이어와 충돌!");
                GameObject player = hit.transform.gameObject;
                director.GetComponent<GameDirector>().PlayerDead(player);

            }
            else if (hit.transform.tag == "Computer")
            {
                Debug.Log("컴퓨터와 충돌!");
                GameObject computer = hit.transform.gameObject;
                director.GetComponent<GameDirector>().ComputerDead(computer);
            }
        }
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
