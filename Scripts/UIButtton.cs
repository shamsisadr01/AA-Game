using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtton : MonoBehaviour
{

    [SerializeField] private RectTransform line;
    [SerializeField] private RectTransform icon;

    private RectTransform m_RectTransform;

    public float Size { get { return line.sizeDelta.y + m_RectTransform.sizeDelta.x; } }
    public Vector2 AnchoredPos {  set { m_RectTransform.anchoredPosition = value; } }

    public RectTransform GetIcon {  get { return icon; } }

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        icon.rotation = Quaternion.identity;
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
