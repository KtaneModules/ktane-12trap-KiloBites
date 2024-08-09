using System.Linq;
using static UnityEngine.Random;
using static UnityEngine.Debug;

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

    private bool beIncorrect;
    private SecOption selectedSec, modifiedSec;
    private int[] ixesToSelect;

    private static readonly string[][] allTypes =
    {
            new[] { "Arbott", "Korea", "Fine Art", "DISPATCHED" },
            new[] { "Archer", "China", "Economics", "KILLED IN ACT." },
            new[] { "Caleb", "U.S.A.", "Education", "CANDIDATE" },
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
        new[] { 4, 6, 1 },
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
        beIncorrect = Range(0, 2) == 0;

    tryagain:

        selectedSec = allSecTypes.PickRandom();

        

        var colors = selectedSec.Colors.ToArray();

        if (beIncorrect)
        {
            

            ixesToSelect = Enumerable.Range(0, 3).ToList().Shuffle().Take(2).OrderBy(x => x).ToArray();

            var doRandom = Enumerable.Range(0, 3).Select(ixesToSelect.Contains).ToArray();

            var tempColor = colors[ixesToSelect[1]];
            var currentColor = colors[ixesToSelect[0]];

            colors[ixesToSelect[0]] = tempColor;
            colors[ixesToSelect[1]] = currentColor;

            modifiedSec = new SecOption
                (!doRandom[0] ? allTypes.Select(x => x[0]).Where(x => !selectedSec.FirstName.Contains(x)).PickRandom() : selectedSec.FirstName,
                !doRandom[1] ? allTypes.Select(x => x[1]).Where(x => !selectedSec.Nationality.Contains(x)).PickRandom() : selectedSec.Nationality,
                !doRandom[2] ? allTypes.Select(x => x[2]).Where(x => !selectedSec.FieldOfStudy.Contains(x)).PickRandom() : selectedSec.FieldOfStudy, selectedSec.Status, colors);

            if ((!doRandom[1] && modifiedSec.Nationality == selectedSec.Nationality) || (!doRandom[2] && modifiedSec.FieldOfStudy == selectedSec.FieldOfStudy))
                goto tryagain;

            if (allSecTypes.Count(x => x.Nationality == modifiedSec.Nationality && x.FieldOfStudy == modifiedSec.FieldOfStudy) + 
                allSecTypes.Count(x => x.FirstName == modifiedSec.FirstName && x.FieldOfStudy == modifiedSec.FieldOfStudy) + 
                allSecTypes.Count(x => x.FirstName == modifiedSec.FirstName && x.Nationality == modifiedSec.Nationality) != 1)
                goto tryagain;
                

            return modifiedSec;

        }

        modifiedSec = selectedSec;

        return selectedSec;
    }

    public void LogHighLevelSec(int modId, char[] colorNames)
    {
        Log($"[12trap #{modId}] The current displayed information is as follows: [Name: {modifiedSec.FirstName}], [Nationality: {modifiedSec.Nationality}], [Field of Study: {modifiedSec.FieldOfStudy}]");

        if (beIncorrect)
        {
            var info = new[] { modifiedSec.FirstName, modifiedSec.Nationality, modifiedSec.FieldOfStudy };

            Log($"[12trap #{modId}] Only two pieces of information match this employee ({ixesToSelect.Select(x => info[x]).Join(", ")})");
            Log($"[12trap #{modId}] The original color sequence is {selectedSec.Colors.Select(x => colorNames[x]).Join("")}. After swapping positions {ixesToSelect.Select(x => x + 1).Join(" and ")}, the final color sequence is: {modifiedSec.Colors.Select(x => colorNames[x]).Join("")}");
        }
        else
            Log($"[12trap #{modId}] All three columns matched. Therefore, the final color sequence is: {selectedSec.Colors.Select(x => colorNames[x]).Join("")}");
    }
}