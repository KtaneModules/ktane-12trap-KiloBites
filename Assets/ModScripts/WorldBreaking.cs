using System;
using System.Collections.Generic;
using System.Linq;
using KModkit;

public class WorldBreaking
{
    public int[] GenerateCoordinate(bool[] grid, int[] cases)
    {
        var candidates = new List<int>();

        var modifiedGrid = grid.ToArray();

        

        for (int i = 0; i < cases.Length; i++)
        {
            var candidateNumbers = new Queue<int>(Enumerable.Range(0, 49).Where(x => modifiedGrid[x]).ToList().Shuffle());
            var number = 0;


            while (candidateNumbers.Count > 0)
            {
                var num = candidateNumbers.Dequeue();
                var adj = CheckAdjacent(num);
                var diag = CheckDiagonalAdjacent(num);

                switch (cases[i])
                {
                    case 0:
                        if (modifiedGrid[num])
                        {
                            number = num;
                            goto found;
                        }
                        break;
                            
                    case 1:
                        if (adj.Where(x => x != null).All(x => modifiedGrid[x.Value]))
                        {
                            number = num;
                            goto found;
                        }
                        break;

                    case 2:
                        if (adj.Where(x => x != null).All(x => modifiedGrid[x.Value]) && 
                            adj.Where(x => x != null).Select(x => TwoSpace(x.Value, Array.IndexOf(adj, x))).Where(x => x != null).All(x => modifiedGrid[x.Value]) &&
                            diag.Where(x => x != null).All(x => modifiedGrid[x.Value]))
                        {
                            number = num;
                            goto found;
                        }
                        break;
                }
            }

            throw new Exception("Puzzle cannot be generated!");

        found:;
            candidates.Add(number);

        }




            return candidates.ToArray();
    }

    public int[] GenerateCoordinateCases() => new[] { 2, 2, 2, 0, 0, 0, 1, 1, 1 }.ToArray();

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

            if (adj[i].Any(num => num < 0 || num > 6))
            {
                actualAdj[i] = null;
                continue;
            }

            actualAdj[i] = adj[i][0] * 7 + adj[i][1];
        }

        return actualAdj;
    }

    private static readonly char[] ColorTable = "RMWBGCYGYRCBWMWBMRYGCMWGYCRBCRBGMYWBCYWRMGYGCMWBR".ToCharArray();

    public int[] GoCommitExplode(bool[] grid, int[] generatedCoordinates, int[] generatedCases)
    {
        var modifiedGrid = grid.ToArray();

        for (int i = 0; i < generatedCoordinates.Length; i++)
        {
            var adjacents = CheckAdjacent(generatedCoordinates[i]);
            var diagonals = CheckDiagonalAdjacent(generatedCoordinates[i]);
            

            switch (generatedCases[i])
            {
                case 0:

                    modifiedGrid[generatedCoordinates[i]] = false;
                    break;
                case 1:

                    modifiedGrid[generatedCoordinates[i]] = false;

                    for (int j = 0; j < 4; j++)
                    {
                        if (adjacents[j] == null)
                            continue;

                        modifiedGrid[adjacents[j].Value] = false;

                    }
                    break;
                case 2:

                    for (int j = 0; j < 4; j++)
                    {
                        if (adjacents[j] != null)
                        {
                            modifiedGrid[adjacents[j].Value] = false;

                            if (TwoSpace(adjacents[j].Value, j) != null)
                                modifiedGrid[TwoSpace(adjacents[j].Value, j).Value] = false;
                        }
                            
                        if (diagonals[j] != null)
                            modifiedGrid[diagonals[j].Value] = false;


                    }
                    break;
                    
            }
        }

        return Enumerable.Range(0, 49).Where(x => modifiedGrid[x]).ToArray();
    }

    public char[] FinalColors(int[] finalIndexes)
    {

        var colors = Enumerable.Range(0, 49).Where(x => finalIndexes.Contains(x)).Select(x => ColorTable[x]).ToArray();

        Array.Reverse(colors);

        return colors;
    }


}
