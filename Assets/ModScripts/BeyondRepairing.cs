using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Random;

public class Arrow
{
    public int ArrowType { get; private set; }
    public Color32 Color { get; private set; }
    public int ArrowPattern { get; private set; }
    public int Direction { get; private set; }

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
        var ooo = Range(0, 3);
        var isIrrelevant = Range(0, 2) == 0;

        var attribs = GetAttribs(ooo, isIrrelevant);

        int chosenAttrib = ooo;

        if (isIrrelevant)
        {
            if (bat == holder)
                chosenAttrib = ooo == 1 ? 0 : 1;

            else if (bat > holder)
                chosenAttrib = ooo == 0 ? 2 : 0;

            else if (bat < holder)
                chosenAttrib = ooo == 2 ? 1 : 2;
        }

        var toBeGrouped = new List<int>();

        for (int i = 0; i < 5; i++)
            if (!toBeGrouped.Contains(attribs[chosenAttrib][i]))
                toBeGrouped.Add(attribs[chosenAttrib][i]);

        for (int i = 0; i < 3; i++)
        {
            var arrowPair = new Arrow[2];

            var grabIxes = Enumerable.Range(0, 6).Where(x => attribs[chosenAttrib][x] == toBeGrouped[i]).ToArray();

            for (int j = 0; j < 2; j++)
                arrowPair[j] = new Arrow(attribs[0][grabIxes[j]], arrowColors[attribs[1][grabIxes[j]]], attribs[2][grabIxes[j]], Range(0, 6));

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
                currentPos = hexDestinations[currentPos][arrow.Direction];

            colors[i] = hexGridColors[currentPos];
        }

        return colors;
    }

}