using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Utility;

public class TestPlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView PV;
    public ParticleSystem deathEffect;
    public Rigidbody rb;
    private Transform tr;

    public float m_moveSpeed = 1;
    public float m_turnSpeed = 200;
    public float m_jumpForce = 4;
    public Animator m_animator;

    private float m_currentV = 0; //현재 가상 수직선 위치
    private readonly float m_interpolation = 10;
    private readonly float m_backwardRunScale = 0.9f;
    private Vector3 m_currentDirection = Vector3.zero;

    private GameObject manager, ChatInput;
    public bool isChat;
    public string nickname;

    private void Start()
    {
        manager = GameObject.Find("NetworkManager");
        ChatInput = GameObject.Find("Canvas").transform.Find("ChatPanel").transform.Find("ChatInputView").gameObject;
        ChatInput.SetActive(false);
        isChat = false;
        nickname = PhotonNetwork.NickName;

        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1.5f, 0); // 무게 중심점을 변경
        tr = GetComponent<Transform>();
        if (PV.IsMine)
            Camera.main.GetComponent<SmoothFollow>().target = tr.Find("CamPivot").transform;
    }
  
    void Update()
    { //메인 캐릭터 업데이트

        if (!PV.IsMine && PhotonNetwork.IsConnected)
            return;

        float v = Input.GetAxis("Vertical"); //상하 이동 W키 : 0~1, S키: -1~0
        float h = Input.GetAxis("Horizontal"); //좌우 이동 D키: 0~1, A키: -1~0

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (ChatInput.activeSelf == false)
                ChatInput.SetActive(true);
            else
            {
                manager.GetComponent<NetworkManager>().Send();
                ChatInput.SetActive(false);
            }
        }
        
        if (v < 0)  v *= m_backwardRunScale;

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation); //상하 갱신

        tr.Translate(Vector3.forward * v * m_moveSpeed * Time.deltaTime);
        tr.Rotate(Vector3.up * h * m_turnSpeed * Time.deltaTime);
        m_animator.SetFloat("Speed", m_currentV); //애니메이션 갱신
    }
    
    [PunRPC]
    public void RPCDestroy() => Destroy(gameObject);
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
