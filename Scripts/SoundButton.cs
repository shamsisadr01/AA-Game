using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    [SerializeField] private Sprite sound;
    [SerializeField] private Sprite mute;

    private UIButtton uiButtton;
    private Image iconImage;

    private void Awake()
    {
        uiButtton = GetComponent<UIButtton>();
        iconImage = uiButtton.GetIcon.GetComponent<Image>();
        if (PlayerPrefs.HasKey("sound"))
        {
            int value = PlayerPrefs.GetInt("sound");
            if (value == 1)
            {
                iconImage.sprite = sound;
            }
            else
            {
                iconImage.sprite = mute;
            }
            GameController.Instance.Mute(value);
        }
    }

    public void Mute()
    {
        int value;
        if (iconImage.sprite == sound)
        {
            iconImage.sprite = mute;
            value = 0;
        }
        else
        {
            iconImage.sprite = sound;
            value = 1;
        }

        PlayerPrefs.SetInt("sound", value);
        GameController.Instance.Mute(value);
    }

  
}
