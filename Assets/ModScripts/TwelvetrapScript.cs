using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using static UnityEngine.Random;
using static UnityEngine.Debug;

public class TwelvetrapScript : MonoBehaviour {

	public KMBombInfo Bomb;
	public KMAudio Audio;
	public KMBombModule Module;

	public KMSelectable[] ledButtons;

	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;

	private static readonly string[] colorNames = { "Black", "Red", "Green", "Blue", "Yellow", "Magenta", "Cyan", "White" };

	private static readonly HighLevelSec[] secSets =
	{
		new HighLevelSec("Arbott", "Korea", "Fine Art", "DISPATCHED", "CBC"),
		new HighLevelSec("Archer", "China", "Economics", "KILLED IN ACT.", "WWY"),
		new HighLevelSec("Caleb", "U.S.A.", "Education", "CANDIDATE", "GYR"),
		new HighLevelSec("Connie", "Greece", "Astronomy", "CANDIDATE", "YMR"),
		new HighLevelSec("Daniel", "Armenia", "Medicine", "CANDIDATE", "PWY"),
		new HighLevelSec("Dekashi", "Japan", "Music", "AVAILABLE", "BMB"),
		new HighLevelSec("Douma", "Armenia", "Chemistry", "UNWORTHY", "YBB"),
		new HighLevelSec("Eriksson", "Greece", "Architecture", "DISPATCHED", "CYG"),
		new HighLevelSec("Fangi", "Thailand", "Marketing", "AVAILABLE", "YYR"),
		new HighLevelSec("Goodman", "U.K.", "Nat. Sciences", "CANDIDATE", "MRR"),
		new HighLevelSec("Jackson", "U.K.", "Media & Comm.", "SUSPECT", "YRC"),
		new HighLevelSec("John \"Scope\"", "Korea", "Business Mng.", "UNWORTHY", "GGM"),
		new HighLevelSec("Jonathan", "U.S.A.", "Statistics", "AVAILABLE", "BRY"),
		new HighLevelSec("King", "Korea", "Soc. Sciences", "CANDIDATE", "MMC"),
		new HighLevelSec("Kusane", "Thailand", "Thai", "UNWORTHY", "MGW"),
		new HighLevelSec("Manny", "China", "Int. Relations", "CANDIDATE", "WGW"),
		new HighLevelSec("Nicholas", "U.K.", "Law", "SUSPECT", "BRW"),
		new HighLevelSec("Paartas", "Armenia", "Engineering", "CANDIDATE", "YBM"),
		new HighLevelSec("Jamie", "Greece", "Mathematics", "KILLED IN ACT.", "RRR"),
		new HighLevelSec("Raymond", "China", "Business Adm.", "DISPATCHED", "RYW"),
		new HighLevelSec("Shaun", "Japan", "Art & Design", "CANDIDATE", "CWM"),
		new HighLevelSec("Vincent", "Japan", "Life Sciences", "CANDIDATE", "BBG"),
		new HighLevelSec("William", "Thailand", "Management", "DEAD", "MYB"),
		new HighLevelSec("T.O.A.S.T.", "U.S.A.", "Anthropology", "ANNOUNCER", "GRB")
	};

	void Awake()
    {

		moduleId = moduleIdCounter++;

		foreach (KMSelectable led in ledButtons)
			led.OnInteract += delegate () { LEDPress(led); return false; };

    }

	
	void Start()
    {
		
    }

	void LEDPress(KMSelectable led)
	{
		if (moduleSolved)
			return;

		var ix = Array.IndexOf(ledButtons, led);
	}
	
	
	void Update()
    {

    }

	// Twitch Plays


#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"!{0} something";
#pragma warning restore 414

	IEnumerator ProcessTwitchCommand (string command)
    {
		string[] split = command.ToUpperInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
		yield return null;
    }

	IEnumerator TwitchHandleForcedSolve()
    {
		yield return null;
    }


}





