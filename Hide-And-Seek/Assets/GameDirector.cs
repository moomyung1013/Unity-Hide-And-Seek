using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    public ParticleSystem deathEffect;
    public Text ComputerCountText, PlayerCountText;
    public int totalComputerCount, totalPlayerCount;
    int ComputerCount, PlayerCount;

    void Start()
    {
        ComputerCount = totalComputerCount;
        PlayerCount = totalPlayerCount;
        UpdateUI();
    }
    
    void Update()
    {
        
    }

    public void PlayerDead(GameObject player)
    {
        Vector3 hitPoint = player.transform.position;
        hitPoint.y += 1.5f;
        Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitPoint)), deathEffect.main.startLifetimeMultiplier);
        player.SetActive(false);
        PlayerCount -= 1;
        UpdateUI();

        if (PlayerCount == 1)
        { // 최후 1인 남았을 때 처리

        }
    }

    public void ComputerDead(GameObject computer)
    {
        Vector3 hitPoint = computer.transform.position;
        hitPoint.y += 1.5f;
        Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitPoint)), deathEffect.main.startLifetimeMultiplier);
        computer.SetActive(false);
        ComputerCount -= 1;
        UpdateUI();
    }

    public void UpdateUI()
    {
        ComputerCountText.text = "Computer: " + ComputerCount.ToString() + " / " + totalComputerCount.ToString();
        PlayerCountText.text = "Player: " + PlayerCount.ToString() + " / " + totalPlayerCount.ToString();
    }

    public void GameOver()
    {

    }
}
