using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanel : MonoBehaviour
{
    [SerializeField] private float shakeSpeed = 250f;
    [SerializeField] private float shakeRadius = 50f;

    private RectTransform m_RectTransform;
    private Animation anim;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        anim = GetComponentInParent<Animation>();
    }

    private void OnEnable()
    {
        m_RectTransform.anchoredPosition = Vector2.zero;
    }

    public void Play(string animName)
    {
        anim.Play(animName);
    }

    public RectTransform RectTransform {  get { return m_RectTransform; } }

    public IEnumerator Shaking()
    {
        Vector2 tPos = Random.insideUnitCircle.normalized * shakeRadius;
        while (true)
        {
            m_RectTransform.anchoredPosition = Vector2.MoveTowards(m_RectTransform.anchoredPosition, tPos, shakeSpeed * Time.deltaTime);
            if (m_RectTransform.anchoredPosition == tPos)
            {
                if (tPos == Vector2.zero)
                    break;

                tPos = Vector2.zero;
            }
            yield return null;
        }
        m_RectTransform.anchoredPosition = Vector2.zero;
    }
}
