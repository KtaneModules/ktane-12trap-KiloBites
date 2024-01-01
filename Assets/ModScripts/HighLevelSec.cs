public class HighLevelSec
{
    public string Name { get; private set; }
    public string Nationality { get; private set; }
    public string FieldOfStudy { get; private set; }
    public string Status { get; private set; }
    public string ColorSet { get; private set; }

    public HighLevelSec(string name, string nationality, string fieldOfStudy, string status, string colorSet)
    {
        Name = name;
        Nationality = nationality;
        FieldOfStudy = fieldOfStudy;
        Status = status;
        ColorSet = colorSet;
    }
}