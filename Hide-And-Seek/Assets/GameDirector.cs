﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
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
        player.SetActive(false);
    }

    public void ComputerDead(GameObject computer)
    {
        computer.SetActive(false);
    }
}
