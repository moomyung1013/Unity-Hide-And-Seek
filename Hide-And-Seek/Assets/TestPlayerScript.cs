using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using UnityStandardAssets.Utility;

public class TestPlayerScript : MonoBehaviourPunCallbacks
{

    public PhotonView PV;
    public Rigidbody rb;
    private Transform tr;

    public float m_moveSpeed = 1;
    public float m_turnSpeed = 200;
    public float m_jumpForce = 4;
    public Animator m_animator;

    private float m_currentV = 0; //현재 가상 수직선 위치
    private float m_currentH = 0; //현재 가상 수평선 위치
    private readonly float m_interpolation = 10;
    private readonly float m_backwardRunScale = 0.9f;
    
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.1f;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1.5f, 0); // 무게 중심점을 변경
        tr = GetComponent<Transform>();
        if (PV.IsMine)
            Camera.main.GetComponent<SmoothFollow>().target = tr.Find("CamPivot").transform;
    }
  

    void Update()
    {//메인 캐릭터 업데이트

        if (!PV.IsMine && PhotonNetwork.IsConnected)
            return;

        float v = Input.GetAxis("Vertical"); //상하 이동 W키 : 0~1, S키: -1~0
        float h = Input.GetAxis("Horizontal"); //좌우 이동 D키: 0~1, A키: -1~0
        //float y_rot = 0;
        if (Input.GetMouseButton(0)) //마우스 왼쪽 버튼을 눌렀을 때
        {
            float y_rot = Input.GetAxisRaw("Mouse X");
            transform.Rotate(0, y_rot * m_turnSpeed * Time.deltaTime, 0); //로테이션
        }
        //PV.RPC("TankUpdate", RpcTarget.AllBuffered, v, h, y_rot);
        if (v < 0)
        { //s일때 뒤로 걷는 속도 적용
            v *= m_backwardRunScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation); //상하 갱신
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation); //좌우 갱신

        //transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime; //상하 이동
        //transform.position += transform.right * m_currentH * m_moveSpeed * Time.deltaTime; //좌우 이동

        tr.Translate(Vector3.forward * v * m_moveSpeed * Time.deltaTime);
        tr.Rotate(Vector3.up * h * m_turnSpeed * Time.deltaTime);
        m_animator.SetFloat("Speed", m_currentV); //애니메이션 갱신
    }

    [PunRPC]
    void TankUpdate(float v, float h, float y_rot)
    {
        if (v < 0)
        { //s일때 뒤로 걷는 속도 적용
            v *= m_backwardRunScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation); //상하 갱신
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation); //좌우 갱신

        transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime; //상하 이동
        transform.position += transform.right * m_currentH * m_moveSpeed * Time.deltaTime; //좌우 이동
        transform.Rotate(0, y_rot * m_turnSpeed * Time.deltaTime, 0); //로테이션
        m_animator.SetFloat("Speed", m_currentV); //애니메이션 갱신


        //tr.Translate(Vector3.forward * v * m_moveSpeed * Time.deltaTime);
        //tr.Rotate(Vector3.up * h * m_turnSpeed * Time.deltaTime);
        //m_animator.SetFloat("Direction", m_currentH); //애니메이션 갱신
    }
}
