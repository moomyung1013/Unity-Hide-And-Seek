using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public ParticleSystem deathEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerDead(GameObject player)
    {
        Vector3 hitPoint = player.transform.position;
        hitPoint.y += 1.5f;
        Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitPoint)), deathEffect.main.startLifetimeMultiplier);
        player.SetActive(false);
    }

    public void ComputerDead(GameObject computer)
    {
        Vector3 hitPoint = computer.transform.position;
        hitPoint.y += 1.5f;
        Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitPoint)), deathEffect.main.startLifetimeMultiplier);
        computer.SetActive(false);
    }
}
