using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Text StatusText;
    public InputField NickNameInput, ChatInput;
    public Button startButton;
    public GameObject startPanel, chatPanel, chatView;
    public string gameVersion = "1.0";
    public PhotonView PV;

    private List<Transform> positionsList = new List<Transform>();
    private Text[] chatList;
    private int idx;


    public ParticleSystem deathEffect;
    public Text ComputerCountText, PlayerCountText;
    public int totalComputerCount, totalPlayerCount;
    [SerializeField] int ComputerCount, PlayerCount;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        Transform[] positions = GameObject.Find("SpawnPosition").GetComponentsInChildren<Transform>();
        chatList = chatView.GetComponentsInChildren<Text>();
        foreach (Transform pos in positions)
            positionsList.Add(pos);

        ComputerCount = totalComputerCount;
        PlayerCount = totalPlayerCount;

        startButton.onClick.AddListener(JoinRoom);

        OnLogin();
    }

    void OnLogin()
    {
        PhotonNetwork.GameVersion = this.gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        startButton.interactable = false;
        StatusText.text = "마스터 서버에 접속중...";
    }
    
    void JoinRoom()
    {
        if (NickNameInput.text.Equals(""))
            PhotonNetwork.LocalPlayer.NickName = "unknown";
        else
            PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        PhotonNetwork.JoinRandomRoom();
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected!");
        StatusText.text = "Online: 마스터 서버와 연결 됨";
        startButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        startPanel.SetActive(false);
        chatPanel.SetActive(true);

        ComputerCountText.text = "Computer: " + ComputerCount.ToString() + " / " + totalComputerCount.ToString();
        PlayerCountText.text = "Player: " + PlayerCount.ToString() + " / " + totalPlayerCount.ToString();

        ChatInput.text = "";
        foreach (Text chat in chatList)
            chat.text = "";

        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>[방장] " + PhotonNetwork.NickName + "님이 참가하셨습니다</color>");
            PV.RPC("CreateComputerPlayer", RpcTarget.All);
        }
        idx = Random.Range(1, positionsList.Count);
        PhotonNetwork.Instantiate("Player3", positionsList[idx].position, Quaternion.identity);
        positionsList.RemoveAt(idx);
    }

    // 플레이어 접속 시 채팅 창 출력
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
    }
    // 플레이어 퇴장 시 채팅 창 출력
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + otherPlayer.NickName + "님이 나갔습니다</color>");
    }

    // 입장할 방이 없을 시, 방 생성
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        this.CreateRoom();
    }

    // 방 생성 함수
    void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }


    public void ComputerDead(GameObject computer)
    {
        Vector3 hitPoint = computer.transform.position;
        hitPoint.y += 1.5f;
        Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitPoint)), deathEffect.main.startLifetimeMultiplier);
        computer.GetComponent<PhotonView>().RPC("RPCDestroy", RpcTarget.AllBuffered);
        PV.RPC("UpdateUI", RpcTarget.All);
    }

    // 채팅 전송 함수
    public void Send()
    {
        if (ChatInput.text.Equals(""))
            return;
        string msg = "[" + PhotonNetwork.NickName + "] " + ChatInput.text;
        PV.RPC("ChatRPC", RpcTarget.All, msg);
        ChatInput.text = "";
    }

    [PunRPC]
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i<chatList.Length; i++)
            if(chatList[i].text == "")
            {
                isInput = true;
                chatList[i].text = msg;
                break;
            }
        if(!isInput)
        {
            for (int i = 1; i < chatList.Length; i++) chatList[i - 1].text = chatList[i].text;
            chatList[chatList.Length - 1].text = msg;
        }
    }

    [PunRPC]
    void CreateComputerPlayer()
    {
        for (int i = 0; i < 10; i++)
        {
            idx = Random.Range(1, positionsList.Count);
            PhotonNetwork.Instantiate("Computer Player", positionsList[idx].position, Quaternion.identity);
            positionsList.RemoveAt(idx);
        }
    }

    [PunRPC]
    public void UpdateUI()
    {
        ComputerCount = ComputerCount - 1;
        ComputerCountText.text = "Computer: " + ComputerCount.ToString() + " / " + totalComputerCount.ToString();
        PlayerCountText.text = "Player: " + PlayerCount.ToString() + " / " + totalPlayerCount.ToString();
    }
    
}
