using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager> 
{
    [SerializeField] private int startCoins;
    private int coins;

    [SerializeField] private int startLives;
    private int lives;

    private int enemiesAlive;

    public int Coins { get => coins; set => coins = value; }
    public int Lives { get => lives; set => lives = value; }
    public int EnemiesAlive { get => enemiesAlive; set => enemiesAlive = value; }

    private void Start()
    {
        Coins = startCoins;
        Lives = startLives;
        EnemiesAlive = 0;
    }
}
