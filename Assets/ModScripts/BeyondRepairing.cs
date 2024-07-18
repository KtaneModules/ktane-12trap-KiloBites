using System;
using System.Collections.Generic;
using System.Linq;

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

}