using System.Linq;

public class HighLevelSec
{
    public string[] SecurityInformation { get; private set; }

    public HighLevelSec(string[] securityInformation)
    {
        SecurityInformation = securityInformation;
    }
}

public class HighLevelSecRules
{
    private readonly HighLevelSec[] Database;
    private readonly HighLevelSec SelectedData;

    public HighLevelSecRules(HighLevelSec[] database, HighLevelSec selectedData)
    {
        Database = database;
        SelectedData = selectedData;
    }

    public bool CheckRules()
    {
        var selectedSecInfo = SelectedData.SecurityInformation.Take(4).ToArray();
        var data = Database.Select(x => x.SecurityInformation.Take(4).ToArray()).ToArray();

        for (int i = 0; i < data.Length; i++)
            if (data[i].SequenceEqual(selectedSecInfo))
                return true;

        return false;
    }
}