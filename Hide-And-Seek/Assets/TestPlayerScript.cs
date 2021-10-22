using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Utility;

public class TestPlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public string nickname;
    public PhotonView PV;
    public ParticleSystem deathEffect;
    public Rigidbody rb;
    private Transform tr;
    private GameObject ChatInput;
    private NetworkManager manager;

    public float m_moveSpeed = 1;
    public float m_turnSpeed = 200;
    public float m_jumpForce = 4;
    public Animator m_animator;

    private float m_currentV = 0, v, h; //현재 가상 수직선 위치
    private readonly float m_interpolation = 10;
    private readonly float m_backwardRunScale = 0.9f;
    private Vector3 m_currentDirection = Vector3.zero;


    private void Start()
    {
        manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        ChatInput = GameObject.Find("Canvas").transform.Find("ChatPanel").transform.Find("ChatInputView").gameObject;
        ChatInput.SetActive(false);
        nickname = PV.Owner.NickName;

        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1.5f, 0); // 무게 중심점을 변경
        tr = GetComponent<Transform>();
        if (PV.IsMine)
            Camera.main.GetComponent<SmoothFollow>().target = tr.Find("CamPivot").transform;
    }
  
    void Update()
    { //메인 캐릭터 업데이트

        if (PV.IsMine)
        {
            v = Input.GetAxis("Vertical"); // 상하 이동 W키 : 0~1, S키: -1~0
            h = Input.GetAxis("Horizontal"); // 좌우 이동 D키: 0~1, A키: -1~0

            if (Input.GetKeyDown(KeyCode.Return) && manager.isGameStart)
            {
                if (ChatInput.activeSelf == false)
                    ChatInput.SetActive(true);
                else
                {
                    manager.Send();
                    ChatInput.SetActive(false);
                }
            }

            if (v < 0) v *= m_backwardRunScale;

            m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation); //상하 갱신

            tr.Translate(Vector3.forward * v * m_moveSpeed * Time.deltaTime);
            tr.Rotate(Vector3.up * h * m_turnSpeed * Time.deltaTime);
            m_animator.SetFloat("Speed", m_currentV); //애니메이션 갱신
            
        }
        else
        {
            if((tr.position - currPos).sqrMagnitude >= 10.0f * 10.0f)
            {
                tr.position = currPos;
                tr.rotation = currRot;
            }
            else
            {
                tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * 10.0f);
                tr.rotation = Quaternion.Slerp(tr.rotation, currRot, Time.deltaTime * 10.0f);
            }
        }
    }
    
    [PunRPC]
    public void RPCDestroy() => Destroy(gameObject);


    private Vector3 currPos;
    private Quaternion currRot;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
