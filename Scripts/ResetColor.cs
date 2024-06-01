using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetColor : MonoBehaviour
{
    private Image image;
    private Color color;
    private void Awake()
    {
        image = GetComponent<Image>();
        color = image.color;
    }
    private void OnEnable()
    {
        image.color = color;
    }
}
