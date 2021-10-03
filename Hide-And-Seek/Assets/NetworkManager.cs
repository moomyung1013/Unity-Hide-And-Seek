using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Text StatusText;
    public InputField NickNameInput;
    public GameObject Cube;
    public string gameVersion = "1.0";


    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        OnLogin();
    }

    void OnLogin()
    {
        PhotonNetwork.GameVersion = this.gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }
    

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected!");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Transform[] positions = GameObject.Find("SpawnPosition").GetComponentsInChildren<Transform>();
        int idx;
        for(int i = 0; i< 10; i ++)
        {
            idx = Random.Range(1, positions.Length);
            PhotonNetwork.Instantiate("Computer Player", positions[idx].position, Quaternion.identity);
        }
        idx = Random.Range(1, positions.Length);
        PhotonNetwork.Instantiate("Player3", positions[idx].position, Quaternion.identity);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        this.CreateRoom();
    }

    void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }
}
