using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGame : MonoBehaviour
{
    [SerializeField] private GameObject uiMenu;
    [SerializeField] private GameObject uiGame;

    [SerializeField] private UIButtton[] buttons;
    [SerializeField] private float rotateSpeed = 100f;
    [SerializeField] private Transform rotator;

    [SerializeField] private GameObject arrowRight;
    [SerializeField] private GameObject arrowLeft;

    private GameController gameController;
    private RectTransform centralCircle;
    private bool switchMode;

    private void Start()
    {
        gameController = GetComponent<GameController>();
        centralCircle = rotator.parent.GetComponent<RectTransform>();
        CreateButtons();
    }

    private void CreateButtons()
    {
        for (int i = 0; i < rotator.childCount; i++)
        {
            Destroy(rotator.GetChild(i).gameObject);
        }

        float angle = 360f / buttons.Length;
        for (int i = 0; i < buttons.Length; i++)
        {
            rotator.rotation = Quaternion.Euler(0, 0, angle * i);
            UIButtton button = Instantiate(buttons[i], centralCircle, false);
            Vector2 anchoredPos = Vector2.down * (centralCircle.sizeDelta.x * 0.5f + button.Size);
            button.AnchoredPos = anchoredPos;
            button.transform.SetParent(rotator);
        }
    }

    private void Update()
    {
        if(!switchMode)
        {
            rotator.Rotate(0, 0, Time.deltaTime * rotateSpeed);
        }
    }

    public void Arrow(bool typeArrow)
    {
        if (gameController.IsPlaying)
        {
            if (typeArrow)
                gameController.Level++;
            else
                gameController.Level--;

            StartCoroutine(gameController.MovePanelGame(typeArrow));
        }
    }

    public void PlayGame()
    {
        switchMode = true;
        uiMenu.SetActive(false);
        uiGame.SetActive(true);
        gameController.GenerateLevel();
        gameController.IsPlaying = true;
        CheckedLevel();
    }

    public void Menu()
    {
        switchMode = false;
        uiMenu.SetActive(true);
        GameController.Instance.StopGame();
        uiGame.SetActive(false);
    }


    public void CheckedLevel()
    {
        if (PlayerPrefs.HasKey("level"))
        {
            int level = PlayerPrefs.GetInt("level");
            gameController.Level = Mathf.Clamp(gameController.Level, 1, level);
            if (level == gameController.Level)
            {
                arrowRight.SetActive(false);
            }else if(!arrowRight.activeSelf)
            {
                arrowRight.SetActive(true);
            }
            
            if (gameController.Level == 1)
            {
                arrowLeft.SetActive(false);
            }
            else if (!arrowLeft.activeSelf)
            {
                arrowLeft.SetActive(true);
            }
        }
    }
  
}
