using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using static UnityEngine.Random;
using static UnityEngine.Debug;

public class TwelvetrapScript : MonoBehaviour
{
    static int moduleIdCounter = 1;
    int moduleId;

    public KMBombInfo Bomb;
	public KMAudio Audio;
	public KMBombModule Module;
	public KMColorblindMode Colorblind;
    public KMSelectable[] LEDSelectables;
    public SpriteRenderer Emblem, Arrow;
	public Sprite[] EmblemSprites, ArrowA, ArrowB, ArrowC, ArrowD, ArrowE, ArrowF;
    public SpriteRenderer HaloTemplate;
    public MeshRenderer[] LEDRends;
    public SpriteRenderer StarRend;
    public SpriteRenderer LowerGlow, UpperGlow;
	public Material[] LEDMats;
	public SpriteRenderer[] Glows;
	public TextMesh[] CBTexts;
	public TextMesh ArrowCB;

	public GameObject[] Menus;

	// High Level Sec stuff

	public TextMesh[] SecTexts;
	public SpriteRenderer FlagRender;
	public Sprite[] Flags;

	// Poison-Pen Letter stuff

	public TextMesh PoisonPenOrdinal;

	// My World Is Breaking stuff

	public TextMesh[] WorldBreakingCoords;

	private Coroutine[] ledPressAnimCoroutines;
	private Coroutine[] cycleCoroutines = new Coroutine[2];
	private Coroutine arrowCycle, solve;
	private List<Color> haloCycle = new List<Color>();
	private static readonly Color[] coloursForRends = new Color[] { new Color(1f, 0.35f, 0.35f), new Color(1f, 1f, 0.35f), new Color(0.35f, 1f, 0.35f), new Color(0.35f, 1f, 1f), new Color(0.35f, 0.35f, 1f), new Color(1f, 0.35f, 1f), Color.white };
	private List<int> colours, solutionColors, colorSwapIxes = new List<int>();
	private float ledInitPos;
	private int[] offLEDs = new int[2];
	private bool cannotPress = true, moduleSolved, cbActive;


	private HighLevelSecurity highLevelSec = new HighLevelSecurity();
	private SecOption selectedOption;

	private PoisonPenLetter poisonPenLetter = new PoisonPenLetter();
	private string[] poisonPenMessages;

	private MyWorldIsBreaking puzzleGenerationGivesMeAStroke = new MyWorldIsBreaking();

	private BeyondRepairing beyondRepairing;

	private Sprite[][] arrowSprites;

	private static readonly string[] colorNames = { "Red", "Yellow", "Green", "Cyan", "Blue", "Magenta", "White" };

	private Sprite GetEmblemSprite(string name) => EmblemSprites.First(x => x.name == name);

	void Awake()
    {
		moduleId = moduleIdCounter++;

		ledPressAnimCoroutines = new Coroutine[LEDSelectables.Length];
		ledInitPos = LEDSelectables[0].GetComponentsInChildren<MeshRenderer>().First(x => x.name == "LED").transform.localPosition.y;

		arrowSprites = new[] { ArrowA, ArrowB, ArrowC, ArrowD, ArrowE, ArrowF };

		foreach (KMSelectable led in LEDSelectables)
			led.OnInteract += delegate () { LEDPress(led); return false; };

		HaloTemplate.gameObject.SetActive(false);
		StarRend.transform.localScale = Vector3.zero;
		Emblem.transform.parent.localScale = Vector3.zero;
        StartCoroutine(EmblemJitter());
        StartCoroutine(EmblemScatter());

		var originalAlpha = LowerGlow.color.a;
        Module.OnActivate += delegate { StartCoroutine(IntroAnim(originalAlpha)); };
        LowerGlow.color = Color.clear;

		for (int i = 0; i < 12; i++)
			UnlightLED(i);

		cbActive = Colorblind.ColorblindModeActive;

		for (int i = 0; i < 3; i++)
			haloCycle.Add(Color.white);
    }

	
	void Start()
    {

		foreach (var text in CBTexts)
			text.text = string.Empty;

        Calculate();
    }

	void Calculate()
	{

        var colorChars = colorNames.Select(x => x[0]).Join("");

		var sections = new[] { new[] { 11, 0, 1 }, new[] { 2, 3, 4 }, new[] { 5, 6, 7 }, new[] { 8, 9, 10 } };

        // High-Level Security


        selectedOption = highLevelSec.SelectSec();
		var secColors = selectedOption.Colors;

		// Poison-Pen Letter

		poisonPenMessages = poisonPenLetter.GenerateMessage();

		var poisonColors = poisonPenLetter.GenerateColors().Select(x => colorChars.IndexOf(x)).ToArray();

		// My World Is Breaking

        puzzleGenerationGivesMeAStroke.GeneratePuzzle(Bomb.GetBatteryCount());


		var worldBreakingColors = puzzleGenerationGivesMeAStroke.Colors.Select(x => colorChars.IndexOf(x)).ToArray();

		// Beyond Re-Pairing

		beyondRepairing = new BeyondRepairing(Bomb.GetBatteryCount(), Bomb.GetBatteryHolderCount());

		Log($"[12trap #{moduleId}] The arrows paired for Beyond Re-Pairing are: {beyondRepairing.LogPairs()}");
		Log($"[12trap #{moduleId}] The arrows are displayed as follows: {beyondRepairing.LogDisplayed()}");
		beyondRepairing.LogColors(moduleId);

		var beyondRepairingColors = beyondRepairing.GetColors().Select(x => colorChars.IndexOf(x)).ToArray();

		var combinedSet = new int[12];

		for (int i = 0; i < 4; i++)
			for (int j = 0; j < 3; j++)
			{
				switch (i)
				{
					case 0:
						combinedSet[sections[i][j]] = secColors[j];
						break;
					case 1:
						combinedSet[sections[i][j]] = poisonColors[j];
						break;
					case 2:
						combinedSet[sections[i][j]] = worldBreakingColors[j];
						break;
					case 3:
						combinedSet[sections[i][j]] = beyondRepairingColors[j];
						break;
				}
			}

		solutionColors = combinedSet.ToList();
		colours = solutionColors.ToList().Shuffle();

		Log($"[12trap #{moduleId}] The solution in clockwise order from the topmost LED is: {solutionColors.Select(x => colorChars[x]).Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / 3).Select(x => x.Select(v => v.Value).Join("")).Join(";")}");
	}


    void LEDPress(KMSelectable led)
	{
		if (moduleSolved || cannotPress || solve != null)
			return;

		var ix = Array.IndexOf(LEDSelectables, led);

		if (ledPressAnimCoroutines[ix] != null)
			StopCoroutine(ledPressAnimCoroutines[ix]);
		ledPressAnimCoroutines[ix] = StartCoroutine(LEDPressAnim(ix));

		if (LEDRends[ix].material.color == Color.clear)
		{
			Log($"[12trap #{moduleId}] You tried to press an LED while it is off. Strike!");
			Module.HandleStrike();
			return;
		}

		if (!colorSwapIxes.Contains(ix))
		{
            colorSwapIxes.Add(ix);

			if (colorSwapIxes.Count == 2)
			{
				

                var temp = colours[colorSwapIxes[1]];
                var currentIx = colours[colorSwapIxes[0]];

                colours[colorSwapIxes[0]] = temp;
                colours[colorSwapIxes[1]] = currentIx;

				Log($"[12trap #{moduleId}] {colorSwapIxes[0] + 1} and {colorSwapIxes[1] + 1} have been swapped.");

                colorSwapIxes.Clear();

				AssignLEDs();

                foreach (var obj in Menus)
                    obj.SetActive(false);

                Emblem.enabled = true;

				haloCycle = Enumerable.Repeat(coloursForRends[6], 3).ToList();

                if (arrowCycle != null)
                {
                    StopCoroutine(arrowCycle);
                    arrowCycle = null;
                }

				if (colours.SequenceEqual(solutionColors))
				{
					foreach (var cycle in cycleCoroutines)
						StopCoroutine(cycle);

					solve = StartCoroutine(SolveAnimation());

					return;
				}

                Audio.PlaySoundAtTransform("accept", transform);

                return;
            }
        }	
		else if (colorSwapIxes.Contains(ix) && colorSwapIxes.Count != 2)
		{
            colorSwapIxes.Clear();

			foreach (var obj in Menus)
				obj.SetActive(false);

			Emblem.enabled = true;
            haloCycle = Enumerable.Repeat(coloursForRends[6], 3).ToList();

            if (arrowCycle != null)
			{
                StopCoroutine(arrowCycle);
                arrowCycle = null;
            }

			return;
        }
			

		switch (ix)
		{
			case 11:
			case 0:
			case 1:
				DisplayThings(0);
				break;
			case 2:
			case 3:
			case 4:
				var getOrdinalIx = new Dictionary<int, int>
				{
					{ 2, 0 },
					{ 3, 1 },
					{ 4, 2 }
				};
				DisplayThings(1, getOrdinalIx[ix]);
				break;
			case 5:
			case 6:
			case 7:
				var getCoordinateIx = new Dictionary<int, int>
				{
					{ 5, 0 },
					{ 6, 1 },
					{ 7, 2 }
				};
				DisplayThings(2, getCoordinateIx[ix]);
				break;
			case 8:
			case 9:
			case 10:
				if (arrowCycle != null)
					return;

				DisplayThings(3);
				break;

		}

    }

	void DisplayThings(int ix, int? groupIx = null)
	{
		Emblem.enabled = false;

		foreach (var obj in Menus)
			obj.SetActive(false);

		if (arrowCycle != null)
		{
			StopCoroutine(arrowCycle);
			arrowCycle = null;
		}

		Menus[ix].SetActive(true);

		switch (ix)
		{
			case 0:
				var flagNames = new[] { "Armenia", "China", "Greece", "Japan", "Korea", "Thailand", "U.K.", "U.S.A." };
				SecTexts[0].text = selectedOption.FirstName;
				SecTexts[1].text = selectedOption.FieldOfStudy;
				FlagRender.sprite = Flags[Array.IndexOf(flagNames, selectedOption.Nationality)];
                for (int i = 0; i < 3; i++)
                    haloCycle[i] = coloursForRends[6];
                break;
			case 1:
				PoisonPenOrdinal.text = poisonPenMessages[groupIx.Value];
                for (int i = 0; i < 3; i++)
                    haloCycle[i] = coloursForRends[6];
                break;
			case 2:
				var coordinateGroups = puzzleGenerationGivesMeAStroke.CoordinateGroups[groupIx.Value];
				var bombTypeGroups = puzzleGenerationGivesMeAStroke.BombTypeGroups[groupIx.Value];
				var colorGroup = new[] { coloursForRends[4], coloursForRends[6], coloursForRends[0]};
				var colorChars = "B R".ToCharArray();

				for (int i = 0; i < 3; i++)
				{
					WorldBreakingCoords[i].text = cbActive ? coordinateGroups[i] + colorChars[(int)bombTypeGroups[i]] : coordinateGroups[i];
					WorldBreakingCoords[i].color = haloCycle[i] = colorGroup[(int)bombTypeGroups[i]];
				}
				break;
			case 3:
				arrowCycle = StartCoroutine(CycleArrows());
				break;

		}
	}

	IEnumerator SolveAnimation()
	{
		Audio.PlaySoundAtTransform("Solve", transform);

		var solveA = StartCoroutine(SolveA());

		yield return new WaitForSeconds(9);

		StopCoroutine(solveA);
		Emblem.enabled = false;

		for (int i = 0; i < 12; i++)
			UnlightLED(i);
	}

	IEnumerator SolveA()
	{
		var speed = 0.25f;

		while (true)
		{
			colours = Enumerable.Range(0, 12).Select(_ => Range(0, 7)).ToList();
			AssignLEDsNoUnlit();
			yield return new WaitForSeconds(speed);

			if (speed == 0.05f)
				continue;

			speed -= 0.005f;
		}
	}



	void LightLED(int pos, Color colour)
	{
        LEDRends[pos].material = LEDMats[1];
        LEDRends[pos].material.color = Glows[pos].color = colour;
    }

	void UnlightLED(int pos)
	{
        LEDRends[pos].material = LEDMats[0];
        LEDRends[pos].material.color = Glows[pos].color = Color.clear;
    }

	void AssignLEDs()
	{
		for (int i = 0; i < LEDRends.Length; i++)
		{
            LightLED(i, coloursForRends[colours[i]]);
			CBTexts[i].text = cbActive && colours[i] != 6 ? colorNames[colours[i]][0].ToString() : string.Empty;
        }
			
        for (int i = 0; i < offLEDs.Length; i++)
		{
            UnlightLED(offLEDs[i]);
			CBTexts[offLEDs[i]].text = string.Empty;
        }
			
    }

	void AssignLEDsNoUnlit()
	{
		for (int i = 0; i < LEDRends.Length; i++)
		{
			CBTexts[i].text = string.Empty;
			LightLED(i, coloursForRends[colours[i]]);
		}
	}

	void CycleLEDs() //float cycleOneInterval = 2.5f /* 30 / 12f */, float cycleTwoInterval = 1f /* 12 / 12f */
    {
		offLEDs[0] = offLEDs[1] = 0;
		AssignLEDs();
		for (int i = 0; i < 2; i++)
		{
			if (cycleCoroutines[i] != null)
				StopCoroutine(cycleCoroutines[i]);
			cycleCoroutines[i] = StartCoroutine(PerformRotations(i, new[] { 2.5f, 1f }[i]));
        }
	}

	private IEnumerator PerformRotations(int ix, float interval)
	{
        while (true)
        {
			float timer = 0;
			while (timer < interval)
			{
				yield return null;
				timer += Time.deltaTime;
			}
			offLEDs[ix] = (offLEDs[ix] + 1) % LEDRends.Length;
            AssignLEDs();
        }
    }

	private IEnumerator LEDPressAnim(int ix, float duration = 0.05f)
    {
        Audio.PlaySoundAtTransform("led press", LEDSelectables[ix].transform);
        var target = LEDSelectables[ix].GetComponentsInChildren<MeshRenderer>().Where(x => x.name == "LED").First();
        target.transform.localPosition = new Vector3(target.transform.localPosition.x, ledInitPos, target.transform.localPosition.z);
        Glows[ix].transform.localScale = Vector3.one * 0.225f;
        duration /= 2;  //Because duration is for the whole animation
        float timer = 0;
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            target.transform.localPosition = new Vector3(target.transform.localPosition.x, Mathf.Lerp(ledInitPos, 0, timer / duration), target.transform.localPosition.z);
			Glows[ix].transform.localScale = Vector3.one * Mathf.Lerp(0.225f, 0.2f, timer / duration);
        }
        target.transform.localPosition = new Vector3(target.transform.localPosition.x, 0, target.transform.localPosition.z);
        Glows[ix].transform.localScale = Vector3.one * 0.2f;
        timer = 0;
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            target.transform.localPosition = new Vector3(target.transform.localPosition.x, Mathf.Lerp(0, ledInitPos, timer / duration), target.transform.localPosition.z);
            Glows[ix].transform.localScale = Vector3.one * Mathf.Lerp(0.2f, 0.225f, timer / duration);
        }
        target.transform.localPosition = new Vector3(target.transform.localPosition.x, ledInitPos, target.transform.localPosition.z);
        Glows[ix].transform.localScale = Vector3.one * 0.225f;
    }

	private IEnumerator IntroAnim(float lowerGlowOriginal, float openingPause = 0.5f, float activateInterval = 0.35f, float hologramSpreadDur = 0.05f)
	{
		Audio.PlaySoundAtTransform("boot", transform);
		float timer = 0;
		while (timer < openingPause)
		{
			yield return null;
			timer += Time.deltaTime;
			StarRend.transform.localEulerAngles = new Vector3(StarRend.transform.localEulerAngles.x, Easing.OutExpo(timer, 0, 180, openingPause), StarRend.transform.localEulerAngles.z);
			StarRend.color = new Color(1, 1, 1, Easing.InSine(timer, 1, 0, openingPause));
			StarRend.transform.localScale = Vector3.one * Mathf.Lerp(0.005f, 0.0035f, timer / openingPause);
			LowerGlow.color = new Color(1, 1, 1, Easing.InSine(timer, 0, lowerGlowOriginal, openingPause));
		}
		StarRend.gameObject.SetActive(false);
		LowerGlow.color = new Color(1, 1, 1, lowerGlowOriginal);
        StartCoroutine(SpawnHalos());

		for (int i = 0; i < 12; i++)
		{
            Audio.PlaySoundAtTransform("colour spawn", LEDSelectables[i].transform);
            LightLED(i, Color.white);
            timer = 0;
			while (timer < activateInterval / (i + 1))
			{
				yield return null;
				timer += Time.deltaTime;
			}
        }
		for (int i = 0; i < 12; i++)
			LightLED(i, coloursForRends[colours[i]]);
        Audio.PlaySoundAtTransform("loaded", transform);
        CycleLEDs();

        timer = 0;
        while (timer < 0.5f)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        Audio.PlaySoundAtTransform("hologram open", transform);

        timer = 0;
		while (timer < hologramSpreadDur)
		{
			yield return null;
			timer += Time.deltaTime;
            Emblem.transform.parent.localScale = new Vector3(Mathf.Lerp(0, 1, timer / hologramSpreadDur), 1, 0.05f);
        }
        Emblem.transform.parent.localScale = new Vector3(1, 1, 0.05f);
        timer = 0;
        while (timer < 0.2f - hologramSpreadDur)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        timer = 0;
        while (timer < hologramSpreadDur)
        {
            yield return null;
            timer += Time.deltaTime;
            Emblem.transform.parent.localScale = new Vector3(1, 1, Mathf.Lerp(0.05f, 1, timer / hologramSpreadDur));
        }
        Emblem.transform.parent.localScale = Vector3.one;
        cannotPress = false;
    }

	IEnumerator CycleArrows()
	{
		var getArrows = beyondRepairing.GeneratedArrows;

        while (true)
		{
			Arrow.enabled = true;

			for (int i = 0; i < 6; i++)
			{
				Arrow.sprite = arrowSprites[getArrows[i].ArrowType][getArrows[i].ArrowPattern];

				haloCycle = Enumerable.Repeat((Color)getArrows[i].Color, 3).ToList();
                Arrow.color = getArrows[i].Color;
				Arrow.transform.localEulerAngles = new Vector3(90, getArrows[i].GetDirRotation(), 0);
				ArrowCB.text = cbActive && getArrows[i].ColorName != "White" ? getArrows[i].ColorName : string.Empty;

                yield return new WaitForSeconds(1);
			}

			Arrow.enabled = false;
			ArrowCB.text = string.Empty;

			haloCycle = Enumerable.Repeat(coloursForRends[6], 3).ToList();

            yield return new WaitForSeconds(1.5f);
		}
    }

    private IEnumerator EmblemJitter(float bound = 0.0001f)
    {
        while (true)
        {
            yield return null;
            Emblem.transform.localPosition = new Vector3(Range(-bound, bound), Emblem.transform.localPosition.y, Range(-bound, bound));
            Emblem.color = new Color(1, 1, 1, Range(0.68f, 0.78f));
        }
    }

    private IEnumerator EmblemScatter(float lowerBound = 3f, float upperBound = 6f, float interval = 0.05f)
    {
		Emblem.sprite = GetEmblemSprite("emblem");
        while (true)
        {
			float timer = 0;
			while (timer < Range(lowerBound, upperBound))
			{
				yield return null;
				timer += Time.deltaTime;
			}
			var dir = Range(0, 4);
			for (int i = 0; i < 3; i++)
			{
				Emblem.sprite = GetEmblemSprite("emblem d" + dir.ToString() + i);
                timer = 0;
                while (timer < interval)
                {
                    yield return null;
                    timer += Time.deltaTime;
                }
            }
            Emblem.sprite = GetEmblemSprite("emblem");
        }
    }

	private IEnumerator SpawnHalos(float spawnRate = 2/3f)
	{
		int i = 0;
		while (true)
		{
			StartCoroutine(FollowHalo(i));
			i = (i + 1) % 3;
			float timer = 0;
			while (timer < spawnRate)
			{
				yield return null;
				timer += Time.deltaTime;
			}
		}
	}

    private IEnumerator FollowHalo(int ix, float lifeLength = 2f, float maxAlpha = 0.25f)
    {
		var halo = Instantiate(HaloTemplate, HaloTemplate.transform.parent);
		halo.transform.localPosition = HaloTemplate.transform.localPosition;
		halo.transform.localScale = Vector3.zero;
		halo.color = haloCycle[ix];
		halo.gameObject.SetActive(true);

		var posFrom = halo.transform.localPosition.y;
		var posTo = Emblem.transform.localPosition.y;

		float timer = 0;
		while (timer < lifeLength)
		{
			yield return null;
			timer += Time.deltaTime;
			halo.transform.localPosition = new Vector3(halo.transform.localPosition.x, Mathf.Lerp(posFrom, posTo, timer / lifeLength), halo.transform.localPosition.z);
			halo.transform.localScale = Vector3.one * Mathf.Lerp(0, HaloTemplate.transform.localScale.x, timer / lifeLength);
			halo.color = new Color(haloCycle[ix].r, haloCycle[ix].g, haloCycle[ix].b, Mathf.PingPong(Mathf.Lerp(0, 2, timer / lifeLength), 1) * maxAlpha);
        }
		Destroy(halo.gameObject);
    }

    // Twitch Plays


#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} something";
#pragma warning restore 414

	IEnumerator ProcessTwitchCommand(string command)
    {
		string[] split = command.ToUpperInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
		yield return null;
    }

	IEnumerator TwitchHandleForcedSolve()
    {
		yield return null;
    }


}