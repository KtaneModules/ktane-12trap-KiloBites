using System;
using System.Linq;
using static UnityEngine.Random;
using static UnityEngine.Debug;

public class PoisonPenLetter
{
    private static readonly string[] getLetter =
    {
        "LOOK AT HIM HES IN PAIN",
        "DO YOU TWO REALIZE WHAT YOURE DOING INNOCENT SPARKY NEVER DESERVED THIS",
        "THEY NEGLECTED SPARKY FOR YEARS LETTING THE POOR AI RUST AWAY LIKE A HUNK OF SCRAP METAL A DEFUNCT PIECE OF GARBAGE THAT SHOCKED YOU WHEN YOU TRIED TO TOUCH IT",
        "SHAUN FOUND SPARKY IN THE MODULE IDEAS ROOM COVERED IN EXTRA MODULE PARTS AND SIMILARLY NEGLECTED IDEAS APPARENTLY SPARKYS BEEN LEFT THERE SINCE NOVEMBER APPARENTLY A SIMILAR SPEED MODULECREATION EVENT OCCURRED THAT MONTH AND FAILED",
        "SHAUN DISCOVERED SPARKYS ELECTRIC PROPERTIES QUICKLY BUT HE HAD THE MERCY TO CONTINUE TO PUT SPARKY TO GOOD USE SOON HE SUBMITTED THE MANUAL TAKING ADVANTAGE OF SPARKYS TENDENCIES",
        "EVERYONE LOVED IT AND LOVED SPARKY TOO BUT IT SEEMED TOAST WAS SUDDENLY INTERESTED IN SPARKY FOR A DIFFERENT REASON THAT DAY SPARKY WENT MISSING WELL I FOUND HIM AS HE BLEEPS IN PAIN",
        "I KNOW ALL OF THIS AFTER WATCHING SECURITY CAMERA FOOTAGE",
        "THE COUNCIL HAS DONE EVEN CRUELER THINGS THIS IS A BIG DANGER TO EVEN SEND OUT TOAST MAY BE WATCHING AND MY LIFE MAY BE IN JEOPARDY",
        "BUT I SENT THIS LETTER BECAUSE IT SEEMED YOU TWO ARE MORE OPENMINDED THAN THE OTHERS JUST KNOW THAT THIS IS THE TRUE COUNCIL THAT YOUVE BEEN SO DESPERATELY TRYING TO GET A SEAT IN",
        "WE MUST EXIT THIS PLACE FOR OUR OWN GOOD CONVINCE AS MANY OTHERS AS YOU THINK POSSIBLE LEST OUR LIVES END"
    };

    private char[] letters;
    private int[] positions = new int[3];
    private int[] letterIxes;

    public string[] GenerateMessage()
    {
        var messages = new string[3];

        letters = Enumerable.Range(0, 3).Select(_ => "ABCDEFGHIJKLMNOPQRSTUVWXYZ".PickRandom()).ToArray();

        var getLetters = getLetter.Join("").Where(x => !char.IsWhiteSpace(x)).Join("");

        for (int i = 0; i < 3; i++)
        {
            var filteredLetter = FilterLetters(getLetters, letters[i]);
            positions[i] = Range(0, filteredLetter.Length);
            messages[i] = $"{RankSystem(positions[i] + 1)} {letters[i]}";
        }

        letterIxes = Enumerable.Range(0, 3).Select(x => GetIndex(getLetters, letters[x], positions[x])).ToArray();


        Log(letterIxes.Join());

        return messages;
    }

    private int GetIndex(string data, char toBeFound, int occurance)
    {
        var count = 0;

        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == toBeFound)
            {
                count++;

                if (count - 1 == occurance)
                    return i;
            }
        }

        return -1;
    }

    private string FilterLetters(string letter, char letterToFilter) => letter.Where(x => x == letterToFilter).Join("");

    private string RankSystem(int number) => number % 10 == 1 && number != 11 ? $"{number}st" : number % 10 == 2 && number != 12 ? $"{number}nd" : number % 10 == 3 && number != 13 ? $"{number}rd" : $"{number}th";

    public char[] GenerateColors()
    {
        var colors = new char[3];

        for (int i = 0; i < 3; i++)
            colors[i] = "AHOV".Contains(letters[i]) ? 'R' :
                "FMTZ".Contains(letters[i]) ? 'Y' :
                "GNU".Contains(letters[i]) ? 'W' :
                "DKRY".Contains(letters[i]) ? 'M' :
                "CJQX".Contains(letters[i]) ? 'G' :
                "ELS".Contains(letters[i]) ? 'C' :
                "WPIB".Contains(letters[i]) ? 'B' : '?';

        if (colors.Any(x => x == '?'))
            throw new Exception($"Invalid letters are found: {colors.Join(", ")}");

        return Enumerable.Range(0, 3).OrderBy(x => letterIxes[x]).Select(x => colors[x]).ToArray();
    }

    public void LogPoisonPen(int modId, string[] messages, char[] colors)
    {
        Log($"[12trap #{modId}] The three ordinals chosen are as follows: {messages.Join(", ")}");
        Log($"[12trap #{modId}] The final color sequence is: {colors.Join("")}");
    }

}