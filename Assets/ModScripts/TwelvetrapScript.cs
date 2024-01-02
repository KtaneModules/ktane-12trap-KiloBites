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
    public SpriteRenderer Emblem;
    public Sprite[] EmblemSprites;
    public SpriteRenderer HaloTemplate;
    public MeshRenderer[] LEDRends;
    public SpriteRenderer StarRend;
    public SpriteRenderer LowerGlow;
	public Material[] LEDMats;
	public SpriteRenderer[] Glows;

	private Color[] coloursForRends = new Color[] { Color.red, new Color(1, 1, 0, 1), Color.green, Color.cyan, Color.blue, Color.magenta, Color.white }; //Why the hell is Color.yellow not 1, 1, 0. It's ugly.
	private List<int> colours = new List<int>();
	private bool cannotPress = true, moduleSolved;

	private static readonly string[] colorNames = { "Red", "Yellow", "Green", "Cyan", "Blue", "Magenta", "White" };

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

	private Sprite GetEmblemSprite(string name)
	{
		return EmblemSprites.Where(x => x.name == name).First();
	}

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

		var originalAlpha = LowerGlow.color.a;
        Module.OnActivate += delegate { StartCoroutine(IntroAnim(originalAlpha)); };
        LowerGlow.color = Color.clear;

		for (int i = 0; i < 12; i++)
			UnlightLED(i);

		Calculate();
    }

	
	void Start()
    {
		
    }

	void Calculate()
	{
		for (int i = 0; i < 12; i++)
			colours.Add(Range(0, 7));
	}

	void LEDPress(KMSelectable led)
	{
		if (moduleSolved)
			return;

		var ix = Array.IndexOf(LEDSelectables, led);
	}
	
	void Update()
    {

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

    private IEnumerator EmblemScatter(float lowerBound = 2f, float upperBound = 5f, float interval = 0.025f)
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