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
                grid = "xxxxx.xxxx.xxxxx.xxxxxxxxxx..xxxxxxxxxx.xxx.xxxxx";
                break;
            case 1:
                grid = "xxx.xxxxx.x.xxxxxxxxxxxx.xxxxxxxxxxxx.x.xxxxx.xxx";
                break;
            case 2:
                grid = "xxxxxxxx.x.xxxxxxx.xxxxx.xxxxx.xxxxxxx.x.xxxxxxxx";
                break;
            case 3:
                grid = ".x.x.x.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.x.x.x";
                break;
            case 4:
                grid = "x.xxxxxxxxxxxxxxx.xxxxx...xxxxx.xxxxxxxxxxxxxxx.x";
                break;
            default:
                grid = "xxxxxxxxxx.xxxxxxxxxx..x.x..xxxxxxxxxx.xxxxxxxxxx";
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

    public static string Coordinate(int pos) => $"{"ABCDEFG"[pos % 7]}{(pos / 7) + 1}";


}
