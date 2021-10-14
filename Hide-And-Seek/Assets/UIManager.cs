using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리자 관련 코드
using UnityEngine.UI; // UI 관련 코드

// 필요한 UI에 즉시 접근하고 변경할 수 있도록 허용하는 UI 매니저
public class UIManager : MonoBehaviour
{
    // 싱글톤 접근용 프로퍼티
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }

            return m_instance;
        }
    }

    private static UIManager m_instance; // 싱글톤이 할당될 변수
    
    public Text scoreText; // 점수 표시용 텍스트
    public GameObject gameoverUI; // 게임 오버시 활성화할 UI 

    public Text ComputerCountText, PlayerCountText;
    public int totalComputerCount, totalPlayerCount;
    private int ComputerCount, PlayerCount;
    private GameObject networkManager;

    private void Awake()
    {
        if (instance != this)
            Destroy(gameObject);
    }
    private void Start()
    {
        networkManager = GameObject.Find("NetworkManager");
        ComputerCount = totalComputerCount;
        PlayerCount = networkManager.GetComponent<NetworkManager>().GetPlayerCount();

        ComputerCountText.text = "Computer: " + ComputerCount + " / " + totalComputerCount;
        PlayerCountText.text = "Player: " + PlayerCount+ " / " + networkManager.GetComponent<NetworkManager>().GetPlayerCount();

    }
    public void DecCount(GameObject obj)
    {
        if (obj.tag == "Player")
            PlayerCount -= 1;
        else
            ComputerCount -= 1;
    }

    public void UpdateScoreText(int com, int play)
    {
        ComputerCountText.text = "Computer: " + com + " / " + totalComputerCount;
        PlayerCountText.text = "Player: " + play + " / " + networkManager.GetComponent<NetworkManager>().GetPlayerCount();
    }
}