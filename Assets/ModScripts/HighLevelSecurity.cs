using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Random;

public class SecOption
{
    public string FirstName { get; set; }
    public string Nationality { get; set; }
    public string FieldOfStudy { get; set; }
    public string Status { get; set; }
    public int[] Colors { get; set; }

    public SecOption(string firstName, string nationality, string fieldOfStudy, string status, int[] colors)
    {
        FirstName = firstName;
        Nationality = nationality;
        FieldOfStudy = fieldOfStudy;
        Status = status;
        Colors = colors;
    }
}


public class HighLevelSecurity
{

    private SecOption[] allSecTypes;

    private static readonly string[][] allTypes =
    {
            new[] { "Arbott", "Korea", "Fine Art", "DISPATCHED" },
            new[] { "Archer", "China", "Economics", "KILLED IN ACT." },
            new[] { "Caleb", "U.S.A", "Education", "CANDIDATE" },
            new[] { "Connie", "Greece", "Astronomy", "CANDIDATE" },
            new[] { "Daniel", "Armenia", "Medicine", "CANDIDATE" },
            new[] { "Dekashi", "Japan", "Music", "AVAILABLE" },
            new[] { "Douma", "Armenia", "Chemistry", "UNWORTHY" },
            new[] { "Eriksson", "Greece", "Architecture", "DISPATCHED" },
            new[] { "Fangi", "Thailand", "Marketing", "AVAILABLE" },
            new[] { "Goodman", "U.K.", "Nat. Sciences", "CANDIDATE" },
            new[] { "Jackson", "U.K.", "Media & Comm.", "SUSPECT" },
            new[] { "John \"Scope\"", "Korea", "Business Mng.", "UNWORTHY" },
            new[] { "Jonathan", "U.S.A.", "Statistics", "AVAILABLE" },
            new[] { "King", "Korea", "Soc. Sciences", "CANDIDATE" },
            new[] { "Kusane", " Thailand", "Thai", "UNWORTHY" },
            new[] { "Manny", "China", "Int. Relations", "CANDIDATE" },
            new[] { "Nicholas", "U.K.", "Law", "SUSPECT" },
            new[] { "Paartas", "Armenia", "Engineering", "CANDIDATE" },
            new[] { "Jamie", "Greece", "Mathematics", "KILLED IN ACT." },
            new[] { "Raymond", "China", "Business Adm.", "DISPATCHED" },
            new[] { "Shaun", "Japan", "Art & Design", "CANDIDATE" },
            new[] { "Vincent", "Japan", "Life Sciences", "CANDIDATE" },
            new[] { "William", "Thailand", "Management", "DEAD" },
            new[] { "T.O.A.S.T.", "U.S.A.", "Anthropology", "ANNOUNCER" }
    };

    private static readonly int[][] colorTypes =
    {
            new[] { 3, 4, 3 },
            new[] { 6, 6, 1 },
            Enumerable.Range(0, 3).Reverse().ToArray(),
            new[] { 1, 5, 0 },
            new[] { 4, 5, 4 },
            new[] { 1, 4, 4 },
            new[] { 3, 1, 2 },
            new[] { 1, 1, 0 },
            new[] { 5, 0, 0 },
            new[] { 1, 0, 3 },
            new[] { 2, 2, 5 },
            new[] { 4, 0, 1 },
            new[] { 5, 5, 3 },
            new[] { 5, 2, 6 },
            new[] { 6, 2, 6 },
            new[] { 4, 0, 6 },
            new[] { 1, 4, 5 },
            Enumerable.Repeat(0, 3).ToArray(),
            new[] { 0, 1, 6 },
            new[] { 3, 6, 5 },
            new[] { 4, 4, 2 },
            new[] { 5, 1, 4 },
            new[] { 2, 0, 4 }
    };

    public HighLevelSecurity()
    {

        allSecTypes = Enumerable.Range(0, allTypes.Length).Select(x => new SecOption(allTypes[x][0], allTypes[x][1], allTypes[x][2], allTypes[x][3], colorTypes[x])).ToArray();
    }

    public SecOption SelectSec()
    {
        var selectedSec = allSecTypes.PickRandom();

        var beIncorrect = Range(0, 2) == 0;

        var colors = selectedSec.Colors;

        if (beIncorrect)
        {
            var ixesToSelect = Enumerable.Range(0, 3).ToList().Shuffle().Take(2).ToArray();

            Array.Sort(ixesToSelect);

            var doRandom = Enumerable.Range(0, 3).Select(ixesToSelect.Contains).ToArray();

            var tempColor = colors[ixesToSelect[1]];
            var currentColor = colors[ixesToSelect[0]];

            colors[ixesToSelect[0]] = tempColor;
            colors[ixesToSelect[1]] = currentColor;

            return new SecOption
                (doRandom[0] ? allTypes.Select(x => x[0]).Where(x => !selectedSec.FirstName.Contains(x)).PickRandom() : selectedSec.FirstName, 
                doRandom[1] ? allTypes.Select(x => x[1]).Where(x => !selectedSec.Nationality.Contains(x)).PickRandom() : selectedSec.Nationality, 
                doRandom[2] ? allTypes.Select(x => x[2]).Where(x => !selectedSec.FieldOfStudy.Contains(x)).PickRandom() : selectedSec.FieldOfStudy, selectedSec.Status, colors);

        }

        return selectedSec;
    }
}