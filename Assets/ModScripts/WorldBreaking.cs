using System;
using System.Collections.Generic;
using System.Linq;
using KModkit;

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
            adj[i] = adj[i] < 0 || adj[i] > 6 ? null : ((i % 2 != 0 ? x : adj[i]) * 7) + (i % 2 == 0 ? y : adj[i]);

        return adj;
    }

    private int? TwoSpace(int pos, int ix)
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

        int? posToCheck = adj[ix] < 0 || adj[ix] > 6 ? null : ((ix % 2 != 0 ? x : adj[ix]) * 7) + (ix % 2 == 0 ? y : adj[ix]);

        return posToCheck;
    }

    private int?[] CheckDiagonalAdjacent(int pos)
    {
        var x = pos / 7;
        var y = pos % 7;

        int[][] adj =
        {
            new int[] { x - 1, y - 1},
            new int[] { x - 1, y + 1},
            new int[] { x + 1, y + 1},
            new int[] { x + 1, y - 1}
        };

        int?[] actualAdj = new int?[4];

        for (int i = 0; i < 4; i++)
        {
            if (adj[i].Any(num => num < 0 || num > 7))
            {
                actualAdj[i] = null;
                continue;
            }

            actualAdj[i] = adj[i][0] * 7 + adj[i][1];
        }

        return actualAdj;
    }

    private static readonly char[] ColorTable = "RMWBGCYGYRCBWMWBMRYGCMWGYCRBCRBGMYWBCYWRMGYGCMWBR".ToCharArray();

    public bool[] GoCommitExplode(int[] randomCoords, int[] bombType, bool[] selectedGrid)
    {
        var finalGrid = new bool[49];

        var ignoredNumbers = Enumerable.Range(0, 49).Where(x => !selectedGrid[x]).ToList();

        for (int i = 0; i < bombType.Length; i++)
            for (int j = 0; j < randomCoords.Length; j++)
                switch (bombType[i])
                {
                    case 0:
                        finalGrid[randomCoords[j]] = true;
                        break;
                    case 1:
                        for (int k = 0; k < 4; k++)
                        {
                            if (CheckAdjacent(randomCoords[j])[k] == null)
                                continue;

                            if (finalGrid[CheckAdjacent(randomCoords[j])[k].Value] || ignoredNumbers.Contains(CheckAdjacent(randomCoords[j])[k].Value))
                                continue;

                            finalGrid[CheckAdjacent(randomCoords[j])[k].Value] = true;
                        }
                        break;
                    case 2:
                        for (int k = 0; k < 4; k++)
                        {
                            if (CheckAdjacent(randomCoords[j])[k] == null)
                                continue;

                            if (TwoSpace(CheckAdjacent(randomCoords[j])[k].Value, k) == null)
                                continue;

                            if (CheckDiagonalAdjacent(randomCoords[j])[k] == null)
                                continue;

                            if (finalGrid[CheckAdjacent(randomCoords[j])[k].Value] || finalGrid[TwoSpace(CheckAdjacent(randomCoords[j])[k].Value, k).Value] ||
                                finalGrid[CheckDiagonalAdjacent(randomCoords[j])[k].Value] || 
                                ignoredNumbers.Any(x => CheckAdjacent(randomCoords[j])[k].Value == x || 
                                TwoSpace(CheckAdjacent(randomCoords[j])[k].Value, k).Value == x || 
                                CheckDiagonalAdjacent(randomCoords[j])[k].Value == x))
                                continue;

                            finalGrid[CheckAdjacent(randomCoords[j])[k].Value] = true;
                            finalGrid[TwoSpace(CheckAdjacent(randomCoords[j])[k].Value, k).Value] = true;
                            finalGrid[CheckDiagonalAdjacent(randomCoords[j])[k].Value] = true;
                        }
                        break;
                }


        return finalGrid;
    }

    public char[] FinalColors(int[] finalIdx)
    {
        var colors = Enumerable.Range(0, 49).Where(x => finalIdx.Contains(x)).Select(x => ColorTable[x]).ToArray();

        Array.Reverse(colors);

        return colors;
    }


}
