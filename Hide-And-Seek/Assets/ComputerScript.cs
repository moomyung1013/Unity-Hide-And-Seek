using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ComputerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public float speed, turnSpeed;
    public Animator computer_animator;
    public ParticleSystem deathEffect;

    private float v, h;
    private int state;
    private float time, rotateTime, stopTime;

    void Start()
    {
        SetRandom();
        state = UnityEngine.Random.Range(0, 3);
    }

    void SetRandom()
    {
        time = UnityEngine.Random.Range(4.0f, 7.0f);
        rotateTime = UnityEngine.Random.Range(2.0f, 3.0f);
        stopTime = UnityEngine.Random.Range(1.0f, 3.0f);
        v = UnityEngine.Random.Range(-1.0f, 1.0f);
        h = UnityEngine.Random.Range(-1.0f, 1.0f);
    }
    /*
    void Update()
    {
        switch (state)
        {
            case 0: // 이동 -> 4초간 이동 후, 2초 정지
                if (time > 0)
                {
                    if (v < 0)
                    { //s일때 뒤로 걷는 속도 적용
                        v *= 0.9f;
                    }

                    m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation); //상하 갱신

                    this.transform.Translate(Vector3.forward * v * speed * Time.deltaTime);
                    this.transform.Rotate(Vector3.up * h * turnSpeed * Time.deltaTime);
                    computer_animator.SetFloat("Speed", m_currentV); //애니메이션 갱신
                    time -= Time.deltaTime;
                }
                else
                {
                    SetRandom();
                    state = 2;
                }
                break;
            case 1: // 회전 -> 2초간 회전 후, 이동 state
                if(rotateTime > 0)
                {
                    this.transform.Rotate(Vector3.up * h * turnSpeed * Time.deltaTime);
                    rotateTime -= Time.deltaTime;
                }
                else
                {
                    SetRandom();
                    state = 0;
                }
                break;
            case 2: // 2초간 정지 후, 0~1 state
                if (stopTime > 0)
                    stopTime -= Time.deltaTime;
                else
                {
                    SetRandom();
                    state = UnityEngine.Random.Range(0, 2);
                }
                break;
        }
    }
    */

        
    [PunRPC]
    public void RPCDestroy() => Destroy(gameObject);

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { 
    }
}
