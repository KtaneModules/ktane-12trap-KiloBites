using System;
using System.Collections.Generic;
using System.Linq;
using KModkit;
using UnityEngine;

public class WorldBreaking
{
    public static bool[] GenerateGrid(KMBombInfo bomb)
    {
        var grid = string.Empty;

        switch (bomb.GetBatteryCount())
        {
            case 0:
                grid = "xxxxx.xxxx.xxxxx.xxxxxxxxxx..xxxxxxxxxx.xxx.xxxxxx";
                break;
            case 1:
                grid = "";
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            default:
                break;
        }

        return grid.Select(cell => cell == 'x').ToArray();
    }

    private int?[] CheckAdjacent(int pos)
    {
        var x = pos / 7;
        var y = pos % 7;

        int?[] adj =
        {
            x - 1,
            y + 1,
            x + 1,
            y - 1
        };

        for (int i = 0; i < adj.Length; i++)
            adj[i] = adj[i] < 0 || adj[i] > 7 ? null : ((i % 2 != 0 ? x : adj[i]) * 7) + (i % 2 == 0 ? y : adj[i]);

        return adj;
    }

    private static readonly char[] ColorTable = "RMWBGCYGYRCBWMWBMRYGCMWGYCRBCRBGMYWBCYWRMGYGCMWBR".ToCharArray();


}
