using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private RectTransform rotator;
    [SerializeField] private GamePanel gamePanel;
    [SerializeField] private RectTransform launcher;
    [SerializeField] private Text labelNumberPins;
    [SerializeField] private Text levelText;
    [SerializeField] private Pin pinPrefabs;
    [SerializeField] private char specialCharacter;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip fireClip;

    private float speed;
    private int level = 1;
    private List<Pin> pins = new List<Pin>();
    private RectTransform centralCircle;
    private AudioSource mySource;
    private int numberPins;
    private int allNumPins;

    private float timer;
    private bool isTimer = true;
    private float targetSpeed;
    private float factorSpeed;
    private int directionSpeed;
    private UIGame uiGame;
    public bool IsPlaying { get;  set; }
    public static GameController Instance { get; private set; }
    public RectTransform GamePanel { get { return gamePanel.RectTransform; } }
    public RectTransform Rotator { get { return rotator; } }

    public int Level {  get { return level; } set { level = value; } }

    private void Awake()
    {
        Instance = this;
        centralCircle = rotator.parent.GetComponent<RectTransform>();
        mySource = gameObject.AddComponent<AudioSource>();
        mySource.playOnAwake = false;
        if (!PlayerPrefs.HasKey("level"))
            PlayerPrefs.SetInt("level", level);
        else
            level = PlayerPrefs.GetInt("level");
        uiGame = GetComponent<UIGame>();
    }

    public void Mute(int value)
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        pinPrefabs.GetComponent<AudioSource>().mute = value == 1 ? false : true;

        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].mute = value == 1 ? false : true;
        }
    }

    public void GenerateLevel()
    {
        Random.InitState(level);
        directionSpeed = Random.Range(-1, 2);

        ClearAll();
        CreateObstacles();
        CreatePins();
        SpeedInitialization();
    }

    private void SpeedInitialization()
    {
        targetSpeed = 200f + (level / 1000f) * 300f;
        if (directionSpeed == 1)
            targetSpeed *= Random.Range(0, 1f);
        else if (directionSpeed == -1)
            targetSpeed *= Random.Range(-1f, 0f);
        else
            targetSpeed *= Random.Range(-1f, 1f);

        factorSpeed = (100f + (level / 1000f) * 150f) * Random.Range(0.25f, 1f);
    }

    private IEnumerator NextLevel()
    {
        IsPlaying = false;

        gamePanel.Play("GamePanel_Background_Win");

        Pin[] r_Pins = rotator.GetComponentsInChildren<Pin>();
        for (int i = 0; i < r_Pins.Length; i++)
        {
            StartCoroutine(r_Pins[i].RadiusIncrease());
        }

        yield return new WaitForSeconds(0.5f);

        if (PlayerPrefs.HasKey("level"))
        {
            int lev = PlayerPrefs.GetInt("level");
            level = Mathf.Clamp(++level, 1, lev);
            if (lev == level && lev <= 1000)
            {
                level++;
                PlayerPrefs.SetInt("level", level);
            }
        }

        StartCoroutine(MovePanelGame(true));
    }

    private void CreatePins()
    {
        labelNumberPins.text = numberPins.ToString();
        for (int i = 0; i < numberPins; i++)
        {
            Pin newPoint = Instantiate(pinPrefabs, GamePanel);
            Vector2 anchoredPos = launcher.anchoredPosition - new Vector2(0, 60 * i);
            newPoint.PinType(anchoredPos, i.ToString(), false);
            pins.Add(newPoint);
        }
        pins.Reverse();
    }

    private void CreateObstacles()
    {
        levelText.text = "Level " + level + "/" + "1000";
        int value = level % 5;
        int obstacles = Random.Range(value, value + 6);
      
        float angle = 360f / obstacles;
        for (int i = 0; i < obstacles; i++)
        {
            Rotator.rotation = Quaternion.Euler(0, 0, angle * i);
            Pin newPoint = Instantiate(pinPrefabs, GamePanel, false);
            Vector2 anchoredPos = Vector2.down * (centralCircle.sizeDelta.x + newPoint.Size);
            anchoredPos = anchoredPos + centralCircle.anchoredPosition;
            newPoint.PinType(anchoredPos, specialCharacter.ToString(), true);
        }

        numberPins = 15 - obstacles;

        allNumPins = obstacles + numberPins;
    }

    private void Update()
    {
        if (!IsPlaying)
            return;


        if(Rotator.childCount == allNumPins)
        {
            mySource.clip = winClip;
            mySource.Play();
            StartCoroutine(NextLevel());
            return;
        }

        if (isTimer)
        {
            SpeedAdjustment();
        }
        else
        {
            timer += Time.deltaTime;
            if(timer > 1f)
            {
                isTimer = true;
                timer = 0f;
            }
        }

        rotator.Rotate(0, 0, Time.deltaTime * speed);
    }

    private void SpeedAdjustment()
    {
        speed = Mathf.MoveTowards(speed, targetSpeed, Time.deltaTime * factorSpeed);
        if (speed == targetSpeed)
        {
            if(targetSpeed == 0)
            {
                PlayAnimPin();
            }
            else
            {
                int state = Random.Range(0, 2);
                if (state == 1)
                    targetSpeed = 0;
                else
                {
                    speed = 0;
                    targetSpeed = 0;
                }
            }
        }
    }

    public void Launch()
    {
        if (numberPins <= 0 || !IsPlaying)
            return;

        mySource.clip = fireClip;
        mySource.Play();

        for (int i = 0; i < numberPins - 1; i++)
        {
            pins[i].AnchoredPos = pins[i + 1].AnchoredPos;
        }

        Vector2 anchoredPos = Vector2.down * (centralCircle.sizeDelta.x + pins[numberPins - 1].Size);
        pins[numberPins - 1].AnchoredPos = anchoredPos + centralCircle.anchoredPosition;
        pins[numberPins - 1].SmoothMovement = false;
        numberPins--;
        labelNumberPins.text = numberPins.ToString();
    }

    private void PlayAnimPin()
    {
        SpeedInitialization();

        isTimer = false;
        Pin[] r_Pins = rotator.GetComponentsInChildren<Pin>();
        for (int i = 0; i < r_Pins.Length; i++)
        {
            r_Pins[i].PlayAnim();
        }
    }

    public IEnumerator RestartGame()
    {
        IsPlaying = false;

        gamePanel.Play("GamePanel_Background_Lose");

        StartCoroutine(gamePanel.Shaking());

        yield return new WaitForSeconds(1f);


        for (int i = 0; i < pins.Count; i++)
        {
            pins[i].ResetPin();
        }
        numberPins = pins.Count;
        labelNumberPins.text = numberPins.ToString();

        rotator.rotation = Quaternion.identity;

        yield return new WaitForSeconds(0.5f);

        speed = 0;
        IsPlaying = true;
    }

    public IEnumerator MovePanelGame(bool typeArrow)
    {
        IsPlaying = false;

        uiGame.CheckedLevel();

        Vector2 right = Vector2.right * 720;
        Vector2 left = Vector2.left * 720f;

        Vector2 m_Position = typeArrow ? left : right;


        while (GamePanel.anchoredPosition != m_Position)
        {
            GamePanel.anchoredPosition = Vector2.MoveTowards(GamePanel.anchoredPosition, m_Position, Time.deltaTime * 2000f);
            yield return null;
        }

        GamePanel.anchoredPosition = typeArrow ? right : left;
        m_Position = Vector2.zero;


        GenerateLevel();

        while (GamePanel.anchoredPosition != m_Position)
        {
            GamePanel.anchoredPosition = Vector2.MoveTowards(GamePanel.anchoredPosition, m_Position, Time.deltaTime * 2000f);
            yield return null;
        }

        IsPlaying = true;

    }

    public void StopGame()
    {
        IsPlaying = false;
        speed = 0;
        ClearAll();
        StopAllCoroutines();
    }

    private void ClearAll()
    {
        for (int i = 0; i < pins.Count; i++)
        {
            Destroy(pins[i].gameObject);
        }
        pins.Clear();

        for (int i = 0; i < rotator.childCount; i++)
        {
            Destroy(rotator.GetChild(i).gameObject);
        }
    }
}

