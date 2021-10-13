using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 점수와 게임 오버 여부, 게임 UI를 관리하는 게임 매니저
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    public static GameManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static GameManager m_instance; // 싱글톤이 할당될 static 변수
    public bool isGameover { get; private set; } // 게임 오버 상태
    public GameObject gameOverPanel;
    public ParticleSystem deathEffect;
    private int ComputerCount = 10, PlayerCount = 2;
    private GameObject countdownText, victoryUserText;
    private float count = 3.0f;

    // 주기적으로 자동 실행되는, 동기화 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 오브젝트라면 쓰기 부분이 실행됨
        if (stream.IsWriting)
        {
            // 네트워크를 통해 score 값을 보내기
            stream.SendNext(ComputerCount);
            stream.SendNext(PlayerCount);
            stream.SendNext(count);
        }
        else
        {
            // 리모트 오브젝트라면 읽기 부분이 실행됨         

            // 네트워크를 통해 score 값 받기
            ComputerCount = (int)stream.ReceiveNext();
            PlayerCount = (int)stream.ReceiveNext();
            count = (float)stream.ReceiveNext();
            // 동기화하여 받은 점수를 UI로 표시
            UIManager.instance.UpdateScoreText(ComputerCount, PlayerCount);
        }
    }


    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
    }

    public void Dead(GameObject obj)
    {
        AddScore(obj.tag);

        Vector3 hitPoint = obj.transform.position;
        hitPoint.y += 1.5f;
        Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitPoint)), deathEffect.main.startLifetimeMultiplier);
        obj.SetActive(false);
    }
    // 점수를 추가하고 UI 갱신
    public void AddScore(string who)
    {
        if(who.Equals("Computer"))
            ComputerCount -= 1;
        else if(who.Equals("Player"))
        {
            PlayerCount -= 1;
            if (PlayerCount == 1)
                photonView.RPC("EndGame", RpcTarget.All);
        }
        // 점수 UI 텍스트 갱신
        UIManager.instance.UpdateScoreText(ComputerCount, PlayerCount);
    }

    void CountDown()
    {
        for (count = 3.0f; count > 0; count -= Time.deltaTime)  UIManager.instance.UpdateCountdownText(countdownText.GetComponent<Text>(), (int)count);
        photonView.RPC("GameExit", RpcTarget.All);
    }

    [PunRPC]
    public void EndGame()
    {
        gameOverPanel.SetActive(true);
        victoryUserText = GameObject.Find("Canvas").transform.Find("GameOverPanel").transform.Find("VictoryText").gameObject;
        countdownText = GameObject.Find("Canvas").transform.Find("GameOverPanel").transform.Find("CountdownText").gameObject;
        
        victoryUserText.GetComponent<Text>().text = "Dalbok is win!";
        CountDown();
    }

    [PunRPC]
    public void GameExit()
    {
        Application.Quit();
    }


}