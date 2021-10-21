using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Photon")]
    public string gameVersion = "1.0";
    public PhotonView PV;

    [Header("Start Panel")]
    public Text StatusText;
    public InputField NickNameInput;
    public Button startButton;
    public GameObject startPanel;

    [Header("Lobby Panel")]
    public GameObject lobbyPanel, lobbyChatView;
    public GameObject[] playerList;
    public InputField lobbyChatInput;
    public Text currentPlayerCountText, roomMasterText;
    public Button gameStartButton, backButton;

    private Text[] lobbyChatList;

    [Header("InGame Panel")]
    public InputField ChatInput;
    public GameObject chatPanel, chatView, gameOverPanel;
    public List<string> nicknameList = new List<string>();

    private List<Transform> positionsList = new List<Transform>();
    private Text[] chatList;
    private Text victoryUserText;
    private int idx;
    private Button exitBtn;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        Transform[] positions = GameObject.Find("SpawnPosition").GetComponentsInChildren<Transform>();
        foreach (Transform pos in positions)
            positionsList.Add(pos);

        chatList = chatView.GetComponentsInChildren<Text>();
        lobbyChatList = lobbyChatView.GetComponentsInChildren<Text>();
        foreach (GameObject player in playerList)
            player.SetActive(false);

        victoryUserText = gameOverPanel.GetComponent<Text>();
        exitBtn = GameObject.Find("Canvas").transform.Find("GameOverPanel").transform.Find("GameExitButton").gameObject.GetComponent<Button>();

        exitBtn.onClick.AddListener(GameExit);
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
        StatusText.text = "Online: 마스터 서버와 연결 됨";
        startButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        startPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        //chatPanel.SetActive(true);

        ChatClear();
        RoomRenewal();

        if (PhotonNetwork.IsMasterClient)
        {
            nicknameList.Add(PhotonNetwork.NickName);
            roomMasterText.text = PhotonNetwork.NickName + "님의 방";
            PV.RPC("PlayerConnect", RpcTarget.AllBuffered, 0, PhotonNetwork.NickName);
            PV.RPC("ChatRPC", RpcTarget.All, "<color=blue>[방장] " + PhotonNetwork.NickName + "님이 참가하셨습니다</color>", 1);
            //PV.RPC("CreateComputerPlayer", RpcTarget.All);
            gameStartButton.interactable = true;
            exitBtn.interactable = true;
        }
        else
        {
            gameStartButton.interactable = false;
            exitBtn.interactable = false;
        }
        //idx = Random.Range(1, positionsList.Count);
        //PhotonNetwork.Instantiate("Player3", positionsList[idx].position, Quaternion.identity);
        //positionsList.RemoveAt(idx);
    }

    // 플레이어 접속 시 채팅 창 출력
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        nicknameList.Add(newPlayer.NickName);
        GameManager.instance.SetCount(PhotonNetwork.CurrentRoom.PlayerCount, true);
        PV.RPC("PlayerConnect", RpcTarget.All, 0, newPlayer.NickName);
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>", 1);
    }

    // 플레이어 퇴장 시 채팅 창 출력
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        nicknameList.Remove(otherPlayer.NickName);
        GameManager.instance.SetCount(PhotonNetwork.CurrentRoom.PlayerCount, false);
        PV.RPC("PlayerConnect", RpcTarget.All, 1, otherPlayer.NickName);
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + otherPlayer.NickName + "님이 나갔습니다</color>", 1);
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


    void GameExit() => photonView.RPC("GameExitRPC", RpcTarget.All);

    public int GetPlayerCount()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public void ChatClear()
    {
        ChatInput.text = "";
        lobbyChatInput.text = "";
        foreach (Text chat in chatList)
            chat.text = "";
        foreach (Text chat in lobbyChatList)
            chat.text = "";
    }

    public void RoomRenewal()
    {
        currentPlayerCountText.text = "";
        currentPlayerCountText.text = PhotonNetwork.PlayerList.Length + " / 4";
    }

    // 채팅 전송 함수
    public void Send()
    {
        if (ChatInput.text.Equals(""))
            return;
        string msg = "[" + PhotonNetwork.NickName + "] " + ChatInput.text;
        PV.RPC("ChatRPC", RpcTarget.All, msg, 0);
        ChatInput.text = "";
    }

    // 채팅 전송 함수
    public void LobbySend()
    {
        if (lobbyChatInput.text.Equals(""))
            return;
        string msg = "[" + PhotonNetwork.NickName + "] " + lobbyChatInput.text;
        PV.RPC("ChatRPC", RpcTarget.All, msg, 1);
        lobbyChatInput.text = "";
    }
    public void DeadSend(string msg) => PV.RPC("ChatRPC", RpcTarget.All, msg);
    
    [PunRPC]
    void ChatRPC(string msg, int state)
    {
        bool isInput = false;
        if(state == 0)
        {
            for (int i = 0; i < chatList.Length; i++)
                if (chatList[i].text == "")
                {
                    isInput = true;
                    chatList[i].text = msg;
                    break;
                }
            if (!isInput)
            {
                for (int i = 1; i < chatList.Length; i++) chatList[i - 1].text = chatList[i].text;
                chatList[chatList.Length - 1].text = msg;
            }
        }
        else
        {
            for (int i = 0; i < lobbyChatList.Length; i++)
                if (lobbyChatList[i].text == "")
                {
                    isInput = true;
                    lobbyChatList[i].text = msg;
                    break;
                }
            if (!isInput)
            {
                for (int i = 1; i < lobbyChatList.Length; i++) lobbyChatList[i - 1].text = lobbyChatList[i].text;
                lobbyChatList[lobbyChatList.Length - 1].text = msg;
            }
        }
    }

    [PunRPC]
    void PlayerConnect(int state, string NickName)
    {
        if(state == 0)
        {
            foreach(GameObject player in playerList)
            {
                if(player.activeSelf == false)
                {
                    player.SetActive(true);
                    player.GetComponentInChildren<Text>().text = NickName;
                    break;
                }
            }
        }
        else
        {
            foreach(GameObject player in playerList)
            {
                if(player.GetComponentInChildren<Text>().text == NickName)
                {
                    player.GetComponentInChildren<Text>().text = "";
                    player.SetActive(false);
                }
            }
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
    public void EndGame()
    {
        gameOverPanel.SetActive(true);
    }

    [PunRPC]
    public void GameExitRPC()
    {
        Application.Quit();
    }
}
