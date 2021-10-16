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
    
    public Text ComputerCountText, PlayerCountText;

    private void Awake()
    {
        if (instance != this)
            Destroy(gameObject);
    }
    private void Start()
    {
    }

    public void UpdateScoreText(int com, int play, int tcom, int tplay)
    {
        ComputerCountText.text = "Computer: " + com + " / " + tcom;
        PlayerCountText.text = "Player: " + play + " / " + tplay;
    }
}