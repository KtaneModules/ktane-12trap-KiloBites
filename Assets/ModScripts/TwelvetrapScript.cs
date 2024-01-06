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
    public KMSelectable[] LEDSelectables;
    public SpriteRenderer Emblem, Arrow;
    public Sprite[] EmblemSprites, ArrowOneSprites, ArrowTwoSprites, ArrowThreeSprites;
    public SpriteRenderer HaloTemplate;
    public MeshRenderer[] LEDRends;
    public SpriteRenderer StarRend;
    public SpriteRenderer LowerGlow;
	public Material[] LEDMats;
	public SpriteRenderer[] Glows;

	private Color[] coloursForRends = new Color[] { Color.red, new Color(1, 1, 0, 1), Color.green, Color.cyan, Color.blue, Color.magenta, Color.white }; //Why the hell is Color.yellow not 1, 1, 0. It's ugly.
	private List<int> colours = new List<int>();
	private bool cannotPress = true, moduleSolved;

	private Sprite[][] arrowSprites;

	private static readonly string[] colorNames = { "Red", "Yellow", "Green", "Cyan", "Blue", "Magenta", "White" };

	private static readonly HighLevelSec[] secSets =
	{
		new HighLevelSec(new string[] { "Arbott", "Korea", "Fine Art", "DISPATCHED", "CBC" }),
		new HighLevelSec(new string[] { "Archer", "China", "Economics", "KILLED IN ACT.", "WWY" }),
		new HighLevelSec(new string[] { "Caleb", "U.S.A.", "Education", "CANDIDATE", "GYR" }),
		new HighLevelSec(new string[] { "Connie", "Greece", "Astronomy", "CANDIDATE", "YMR" }),
		new HighLevelSec(new string[] { "Daniel", "Armenia", "Medicine", "CANDIDATE", "PWY" }),
		new HighLevelSec(new string[] { "Dekashi", "Japan", "Music", "AVAILABLE", "BMB" }),
		new HighLevelSec(new string[] { "Douma", "Armenia", "Chemistry", "UNWORTHY", "YBB" }),
		new HighLevelSec(new string[] { "Eriksson", "Greece", "Architecture", "DISPATCHED", "CYG" }),
		new HighLevelSec(new string[] { "Fangi", "Thailand", "Marketing", "AVAILABLE", "YYR" }),
		new HighLevelSec(new string[] { "Goodman", "U.K.", "Nat. Sciences", "CANDIDATE", "MRR" }),
		new HighLevelSec(new string[] { "Jackson", "U.K.", "Media & Comm.", "SUSPECT", "YRC" }),
		new HighLevelSec(new string[] { "John \"Scope\"", "Korea", "Business Mng.", "UNWORTHY", "GGM" }),
		new HighLevelSec(new string[] { "Jonathan", "U.S.A.", "Statistics", "AVAILABLE", "BRY" }),
		new HighLevelSec(new string[] { "King", "Korea", "Soc. Sciences", "CANDIDATE", "MMC" }),
		new HighLevelSec(new string[] { "Kusane", "Thailand", "Thai", "UNWORTHY", "MGW" }),
		new HighLevelSec(new string[] { "Manny", "China", "Int. Relations", "CANDIDATE", "WGW" }),
		new HighLevelSec(new string[] { "Nicholas", "U.K.", "Law", "SUSPECT", "BRW" }),
		new HighLevelSec(new string[] { "Paartas", "Armenia", "Engineering", "CANDIDATE", "YBM" }),
		new HighLevelSec(new string[] { "Jamie", "Greece", "Mathematics", "KILLED IN ACT.", "RRR" }),
		new HighLevelSec(new string[] { "Raymond", "China", "Business Adm.", "DISPATCHED", "RYW" }),
		new HighLevelSec(new string[] { "Shaun", "Japan", "Art & Design", "CANDIDATE", "CWM" }),
		new HighLevelSec(new string[] { "Vincent", "Japan", "Life Sciences", "CANDIDATE", "BBG" }),
		new HighLevelSec(new string[] { "William", "Thailand", "Management", "DEAD", "MYB" }),
		new HighLevelSec(new string[] { "T.O.A.S.T.", "U.S.A.", "Anthropology", "ANNOUNCER", "GRB" })
	};

	private HighLevelSec SelectedSec()
	{
		var selectedValues = new int[2];

		var rnd = Range(0, secSets.Length);

		selectedValues[0] = Range(0, 4);
		selectedValues[1] = Enumerable.Range(0, 4).Where(x => x != selectedValues[0]).PickRandom();

		var randomSelectValue = Range(0, 2) == 0;

		var selectedSecSet = secSets[rnd];

		var selected = new string[5];

		for (int i = 0; i < 5; i++)
			selected[i] = selectedValues.Contains(i) || i == 4 ? selectedSecSet.SecurityInformation[i] : 
				Enumerable.Range(0, secSets.Length).Where(x => x != rnd).Select(x => secSets[x]).Where(x => x.SecurityInformation[i] != selectedSecSet.SecurityInformation[selectedValues[0]] && x.SecurityInformation[i] != selectedSecSet.SecurityInformation[selectedValues[1]]).PickRandom().SecurityInformation[i];

		return randomSelectValue ? secSets.PickRandom() : new HighLevelSec(selected);
	}

	private HighLevelSecRules secRuleset;

	private Sprite GetEmblemSprite(string name) => EmblemSprites.Where(x => x.name == name).First();
	private Sprite GetArrowSprite(string name, int arr) => arrowSprites[arr].Where(x => x.name == name).First();

	void Awake()
    {
		moduleId = moduleIdCounter++;

		foreach (KMSelectable led in LEDSelectables)
			led.OnInteract += delegate () { LEDPress(led); return false; };

		HaloTemplate.gameObject.SetActive(false);
		StarRend.transform.localScale = Vector3.zero;
		Emblem.transform.parent.localScale = Vector3.zero;
        StartCoroutine(EmblemJitter());
        StartCoroutine(EmblemScatter());

		arrowSprites = new Sprite[][] { ArrowOneSprites, ArrowTwoSprites, ArrowThreeSprites };

		var originalAlpha = LowerGlow.color.a;
        Module.OnActivate += delegate { StartCoroutine(IntroAnim(originalAlpha)); };
        LowerGlow.color = Color.clear;

		for (int i = 0; i < 12; i++)
			UnlightLED(i);

		
    }

	
	void Start()
    {
		secRuleset = new HighLevelSecRules(secSets, SelectedSec());

        Calculate();
    }

	void Calculate()
	{
		for (int i = 0; i < 12; i++)
			colours.Add(Range(0, 7));
	}

	void LEDPress(KMSelectable led)
	{
		if (moduleSolved || cannotPress)
			return;

		var ix = Array.IndexOf(LEDSelectables, led);
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

	private IEnumerator ArrowScatter(int selectedArrow, float lowerBound = 2f, float upperBound = 5f, float interval = 0.025f)
	{
		Arrow.sprite = GetArrowSprite($"arrow {selectedArrow}", selectedArrow);

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
				Arrow.sprite = GetArrowSprite($"arrow {selectedArrow}d{dir + i}", selectedArrow);
				timer = 0;

				while (timer < interval)
				{
					yield return null;
					timer += Time.deltaTime;
				}
			}
            Arrow.sprite = GetArrowSprite($"arrow {selectedArrow}", selectedArrow);
        }
	}

	private IEnumerator SpawnHalos(float spawnRate = 2/3f)
	{
		while (true)
		{
			StartCoroutine(FollowHalo());
			float timer = 0;
			while (timer < spawnRate)
			{
				yield return null;
				timer += Time.deltaTime;
			}
		}
	}

    private IEnumerator FollowHalo(float lifeLength = 2f, float maxAlpha = 0.25f)
    {
		var halo = Instantiate(HaloTemplate, HaloTemplate.transform.parent);
		halo.transform.localPosition = HaloTemplate.transform.localPosition;
		halo.transform.localScale = Vector3.zero;
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
			halo.color = new Color(1, 1, 1, Mathf.PingPong(Mathf.Lerp(0, 2, timer / lifeLength), 1) * maxAlpha);
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