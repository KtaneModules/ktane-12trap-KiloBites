using System.Collections.Generic;
using System.Linq;
using static UnityEngine.Random;
using static UnityEngine.Debug;

public enum BombType
{
    Dynamite,
    Bomb,
    SuperBomb
}

public class Bomb
{
    public int Position { get; private set; }
    public BombType BombType { get; private set; }
    public int?[] Adjacents { get; private set; }

    public Bomb(int position, BombType type, int?[] adjacents = null)
    {
        Position = position;
        BombType = type;
        Adjacents = adjacents;
    }
}
public class MyWorldIsBreaking
{
    private string GetRowColumn(int pos) => $"({(pos / 7) + 1}, {(pos % 7) + 1})";

    private string[] coordinates;

    public List<string[]> CoordinateGroups;
    public List<BombType[]> BombTypeGroups;

    private bool[] grid, modifiedGrid;

    private List<int> selectedCoords = new List<int>();

    public override string ToString() => grid.Select(x => x ? 'X' : '-').Join("");

    public List<Bomb> GeneratedBombs = new List<Bomb>();
    public char[] Colors;

    private int?[] OrthrogonalAdjacents(int pos)
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

        var getAdj = new int?[4];

        for (int i = 0; i < 4; i++)
            getAdj[i] = adj[i] < 0 || adj[i] > 6 ? null : ((i % 2 != 0 ? x : adj[i]) * 7) + (i % 2 == 0 ? y : adj[i]);

        return getAdj;
    }

    private int?[] DiagonalAdjacents(int pos)
    {
        var x = pos / 7;
        var y = pos % 7;

        int[][] adj =
        {
            new[] { x - 1, y - 1 },
            new[] { x - 1, y + 1 },
            new[] { x + 1, y + 1 },
            new[] { x + 1, y - 1 }
        };

        var actualAdj = new int?[4];

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

    private int?[] TwoStep(int pos)
    {
        var x = pos / 7;
        var y = pos % 7;

        int?[] adj =
        {
                x - 2,
                y + 2,
                x + 2,
                y - 2,
            };

        int?[] actualAdj = new int?[4];

        for (int i = 0; i < 4; i++)
            actualAdj[i] = adj[i] < 0 || adj[i] > 6 ? null : ((i % 2 != 0 ? x : adj[i]) * 7) + (i % 2 == 0 ? y : adj[i]);

        return actualAdj;

    }

    private bool[] GrabGrid(int batteryCount)
    {
        var grid = string.Empty;

        switch (batteryCount)
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
                grid = "x.xxxxxxxxxxxxxxx.xxxxx...xxxxx.xxxxxxxxxxxxxxxxx";
                break;
            default:
                grid = "xxxxxxxxxx.xxxxxxxxxx..x.x..xxxxxxxxxx.xxxxxxxxxx";
                break;
        }

        return grid.Select(x => x == 'x').ToArray();
    }

    public void GeneratePuzzle(int bat)
    {
        var colorGrid = "RMWBGCYGYRCBWMWBMRYGCMWGYCRBCRBGMYWBCYWRMGYGCMWBR".ToCharArray();

        grid = GrabGrid(bat);

        int dynamiteCount = Range(1, 4);

    tryagain:
        GeneratedBombs.Clear();
        selectedCoords.Clear();
        Bomb generatedBomb = null;
        int coord = 0;
        BombType bomb;
        

        modifiedGrid = grid.ToArray();

        do
        {
            if (9 - GeneratedBombs.Count < modifiedGrid.Count(x => x) - 3 && 9 - GeneratedBombs.Count <= dynamiteCount)
                goto tryagain;


            coord = Enumerable.Range(0, 49).Where(x => !selectedCoords.Contains(x)).PickRandom();

            if (9 - GeneratedBombs.Count <= dynamiteCount)
                bomb = BombType.Dynamite;
            else if (modifiedGrid.Count(x => x) - 3 - dynamiteCount <= (9 - GeneratedBombs.Count) * 5)
                bomb = (BombType)Enumerable.Range(0, 3).Where(x => x != 0).PickRandom();
            else if (modifiedGrid.Count(x => x) - 3 - dynamiteCount <= (9 - GeneratedBombs.Count) * 13)
                bomb = BombType.Bomb;
            else
                goto tryagain;

            switch (bomb)
            {
                case BombType.Dynamite:
                    generatedBomb = new Bomb(coord, bomb);
                    break;
                case BombType.Bomb:
                    generatedBomb = new Bomb(coord, bomb, OrthrogonalAdjacents(coord));
                    break;
                case BombType.SuperBomb:
                    generatedBomb = new Bomb(coord, bomb, OrthrogonalAdjacents(coord).Concat(TwoStep(coord)).Concat(DiagonalAdjacents(coord)).ToArray());
                    break;
            }

            if (bomb == BombType.Bomb || bomb == BombType.SuperBomb)
            {
                var filtered = generatedBomb.Adjacents.Where(x => x != null).Where(x => modifiedGrid[x.Value]).ToArray();

                modifiedGrid[generatedBomb.Position] = false;

                foreach (var adj in filtered)
                    modifiedGrid[adj.Value] = false;
            }
            else
                modifiedGrid[generatedBomb.Position] = false;

            GeneratedBombs.Add(generatedBomb);
            selectedCoords.Add(coord);
        }
        while (modifiedGrid.Count(x => x) != 3);

        if (GeneratedBombs.Count != 9)
            goto tryagain;

        
        Colors = Enumerable.Range(0, 49).Where(x => modifiedGrid[x]).Select(x => colorGrid[x]).ToArray();
        GeneratedBombs.Shuffle();
        coordinates = GeneratedBombs.Select(x => x.Position).Select(GetRowColumn).ToArray();
        CoordinateGroups = coordinates.Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / 3).Select(x => x.Select(v => v.Value).ToArray()).ToList();
        BombTypeGroups = GeneratedBombs.Select(x => x.BombType).Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / 3).Select(x => x.Select(v => v.Value).ToArray()).ToList();
    }

    public void LogMyWorldIsBreaking(int modId, int bat)
    {
        Log($"[12trap #{modId}] The coordinates selected were: {Enumerable.Range(0, 9).Select(x => $"{coordinates[x]} [{"BKR"[(int)GeneratedBombs[x].BombType]}]").Join(", ")}");
        Log($"[12trap #{modId}] Grid {bat} has been selected");
        Log($"[12trap #{modId}] Before grid: {Enumerable.Range(0, 7).Select(x => Enumerable.Range(0, 7).Select(y => grid[7 * x + y] ? 'X' : '-').Join("")).Join(";")}");
        Log($"[12trap #{modId}] After grid: {Enumerable.Range(0, 7).Select(x => Enumerable.Range(0, 7).Select(y => modifiedGrid[7 * x + y] ? 'X' : '-').Join("")).Join(";")}");

        Log($"[12trap #{modId}] The final colors were {Colors.Join("")}");
    }
}
