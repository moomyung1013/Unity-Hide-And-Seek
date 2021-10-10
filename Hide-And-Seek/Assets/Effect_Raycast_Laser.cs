using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Effect_Raycast_Laser : MonoBehaviourPunCallbacks
{
    public GameObject Raybody; //쏘는 위치
    public GameObject ScaleDistance;
    public GameObject RayResult; // 충돌 위치 출력 결과
    public PhotonView PV;

    GameObject director, player;

    private void Start()
    {
        director = GameObject.Find("Computer Player");
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

    [PunRPC]
    void Laser()
    {
        RaycastHit hit;
        if (Physics.Raycast(Raybody.transform.position, ScaleDistance.transform.forward, out hit, 1.0f))
        {
            if (!PV.IsMine && hit.transform.tag == "Player" && hit.transform.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log("플레이어와 충돌!");
                GameObject player = hit.transform.gameObject;
                hit.transform.GetComponent<TestPlayerScript>().Dead(player);

            }
            else if (hit.transform.tag == "Computer")
            {
                Debug.Log("컴퓨터와 충돌!");
                GameObject computer = hit.transform.gameObject;
                computer.GetComponent<ComputerScript>().Dead(computer);
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
