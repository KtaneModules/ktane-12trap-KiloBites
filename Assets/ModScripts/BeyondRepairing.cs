using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Random;
using static UnityEngine.Debug;

public class Arrow
{
    public int ArrowType { get; set; }
    public Color32 Color { get; set; }
    public int ArrowPattern { get; set; }
    public int Direction { get; set; }

    public Arrow(int arrowType, Color32 color, int arrowPattern, int direction)
    {
        ArrowType = arrowType;
        Color = color;
        ArrowPattern = arrowPattern;
        Direction = direction;
    }

    public int GetDirRotation()
    {
        switch (Direction)
        {
            case 0:
                return 0;
            case 1:
                return 45;
            case 2:
                return 135;
            case 3:
                return 180;
            case 4:
                return 225;
            case 5:
                return 315;
        }

        throw new ArgumentOutOfRangeException($"{Direction} is an invalid direction index. It must be within the range of 0-5.");
    }
}

public class BeyondRepairing
{
    /*
     * Hexagon diagram is as follows:
     * 
     *   0
     * 1   2
     *   3
     * 4   5
     *   6
     */

    private static readonly Dictionary<int, int[]> hexDestinations = new Dictionary<int, int[]>
    {
        // The array is navigated in this order: U, UR, DR, D, DL, UL.

        { 0, new[] { 6, 1, 2, 3, 1, 2 } },
        { 1, new[] { 4, 0, 3, 4, 0, 5 } },
        { 2, new[] { 5, 4, 0, 5, 3, 0 } },
        { 3, new[] { 0, 2, 5, 6, 4, 1 } },
        { 4, new[] { 1, 3, 6, 1, 2, 6 } },
        { 5, new[] { 2, 6, 1, 2, 6, 3 } },
        { 6, new[] { 3, 5, 4, 0, 5, 4 } }
    };

    private static readonly char[] hexGridColors = "RYMWGBC".ToCharArray();

    private static readonly Color32[] arrowColors = { new Color32(255, 0, 0, 200), new Color32(255, 255, 0, 200), new Color32(0, 255, 0, 200), new Color32(0, 255, 255, 200), new Color32(0, 0, 255, 200), new Color32(255, 0, 255, 200), new Color32(255, 255, 255, 200) };

    public List<Arrow> GeneratedArrows = new List<Arrow>();
    private List<Arrow[]> arrowPairs = new List<Arrow[]>();

    private int currentPos;


    private List<List<int>> GetAttribs(int oddOneOut, bool isIrrelevant)
    {

        var attribs = new List<List<int>>();

        for (int i = 0; i < 3; i++)
        {
            attribs.Add(new List<int>());

            if (isIrrelevant ^ (i == oddOneOut))
                for (int j = 0; j < 3; j++)
                {
                    var chosenAttrib = Enumerable.Range(0, 6 + (i % 2)).Where(x => !attribs[i].Contains(x)).PickRandom();
                    for (int k = 0; k < 2; k++)
                        attribs[i].Add(chosenAttrib);
                }
            else
            {
            tryagain:
                attribs[i] = new List<int>();

                for (int j = 0; j < 6; j++)
                    attribs[i].Add(Range(0, 6 + (i % 2)));

                var counts = new int[6 + (i % 2)];

                foreach (var attrib in attribs[i])
                    counts[attrib]++;

                Array.Sort(counts);

                var counter = 0;

                var reference = new[] { 0, 0, 0, 0, 2, 2, 2 };

                for (int j = (i + 1) % 2; j < reference.Length; j++)
                    if (reference[i] == counts[j + (i % 2) - 1])
                        counter++;

                if (counter >= counts.Length)       
                    goto tryagain;

            }
        }

        return attribs;
    }

    public BeyondRepairing(int bat, int holder)
    {
        var attribs = GetAttribs(Range(0, 3), Range(0, 2) == 0);


        while (arrowPairs.Count < 3)
        {
            var arrowPair = new Arrow[2];

            var dupes = attribs.Select(x => x[bat == holder ? 1 : bat > holder ? 0 : 2]).GroupBy(x => x).Where(x => x.Count() == 2).Select(x => x.Key).ToList();
            var grabIxes = Enumerable.Range(0, 6).Where(attribs[bat == holder ? 1 : bat > holder ? 0 : 2].Contains).ToArray();

            for (int i = 0; i < 2; i++)
                arrowPair[i] = new Arrow(attribs[0][grabIxes[i]], arrowColors[attribs[1][grabIxes[i]]], attribs[2][grabIxes[i]], Range(0, 6));

            arrowPairs.Add(arrowPair);

        }

        arrowPairs.ForEach(GeneratedArrows.AddRange);

        GeneratedArrows.Shuffle();
    }

    public char[] GetColors()
    {
        var colors = new char[3];

        for (int i = 0; i < 3; i++)
        {
            currentPos = 3;

            foreach (var arrow in arrowPairs[i])
            {
                var allHexPossibilities = hexDestinations[currentPos];
                currentPos = allHexPossibilities[arrow.Direction];
            }

            colors[i] = hexGridColors[currentPos];
        }

        return colors;
    }

}