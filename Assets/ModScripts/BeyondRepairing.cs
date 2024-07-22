using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Random;
using KModkit;

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

    public List<Arrow> GeneratedArrows;
    private List<Arrow[]> arrowPairs = new List<Arrow[]>();

    private int currentPos;

    public BeyondRepairing(KMBombInfo bomb)
    {
        while (arrowPairs.Count < 3)
        {
            var arrowPair = new Arrow[2];


        }
    }

}