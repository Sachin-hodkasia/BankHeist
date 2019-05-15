using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStats
{
    public int kills;
    public int matchPlayed;
    public int betsWon;
    public int streak;
    public int coins;
    public int cash;

    public UserStats(int kills,int matchPlayed, int betsWon,int streak,int coins,int cash)
    {
        this.kills = kills;
        this.matchPlayed = matchPlayed;
        this.betsWon = betsWon;
        this.streak = streak;
        this.coins = coins;
        this.cash = cash;
    }
}
