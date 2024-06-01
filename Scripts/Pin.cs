using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class Pin : MonoBehaviour
{

    private RectTransform myRectTrans;
    private RectTransform line;
    private Animation anim;
    private Text label;
    private AudioSource mySource;
    private bool isMove;
    private Vector2 sizeDelta;
    private Vector2 anchorTargetPos;
    private Vector2 initialAnchorPos;

    public bool SmoothMovement {  get; set; }
    public Vector2 AnchoredPos { get { return anchorTargetPos; } set { anchorTargetPos = value; } }
    public float Size { get { return line.sizeDelta.y + myRectTrans.sizeDelta.x; } }

    private void Awake()
    {
        myRectTrans = GetComponent<RectTransform>();
        label = GetComponentInChildren<Text>();
        line = (RectTransform)myRectTrans.GetChild(1);
        sizeDelta = line.sizeDelta;
        anim = line.GetComponent<Animation>();
        mySource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!isMove)
            return;


        if (SmoothMovement)
        {
            myRectTrans.anchoredPosition = Vector2.Lerp(myRectTrans.anchoredPosition, anchorTargetPos, Time.deltaTime * 20f);
        }
        else
        {
            myRectTrans.anchoredPosition = Vector2.MoveTowards(myRectTrans.anchoredPosition, anchorTargetPos, Time.deltaTime * 1000f);
            if (myRectTrans.anchoredPosition == anchorTargetPos)
            {
                line.gameObject.SetActive(true);
                myRectTrans.SetParent(GameController.Instance.Rotator);
                isMove = false;
                SmoothMovement = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!SmoothMovement && isMove)
        {
            if (GameController.Instance.IsPlaying)
            {
                StartCoroutine(GameController.Instance.RestartGame());
            }
            mySource.Play();
            isMove = false;
        }
    }

    public IEnumerator RadiusIncrease()
    {
        if (anim.isPlaying)
        {
            anim.Stop();
            line.sizeDelta = sizeDelta;
        }

        float aspect = Screen.height / Screen.width;
        Vector3 targetPos = myRectTrans.position - myRectTrans.up * 250 * aspect;
        Vector2 anchorPosition = myRectTrans.anchoredPosition;
    
        float timer = 0.0f;

        while (timer < 0.5f)
        {

            myRectTrans.position = Vector3.Lerp(myRectTrans.position, targetPos, Time.deltaTime * 10f);

            float magnitude = (myRectTrans.anchoredPosition - anchorPosition).magnitude;
            line.sizeDelta = sizeDelta + Vector2.up * magnitude;

            timer += Time.deltaTime;

            yield return null;
        }
    }

    public void PlayAnim()
    {
        anim.Play();
    }

    public void ResetPin()
    {
        myRectTrans.rotation = Quaternion.identity;
        myRectTrans.SetParent (GameController.Instance.GamePanel);
        anim.Stop();
        line.gameObject.SetActive(false);
        anchorTargetPos = initialAnchorPos;
        SmoothMovement = true;
        isMove = true;
    }

    public void PinType(Vector2 anchorPos,string message,bool isObstacle)
    {
        myRectTrans.anchoredPosition = anchorPos;
        label.text = message;
        if (!isObstacle)
        {
            this.anchorTargetPos = anchorPos;
            initialAnchorPos = anchorPos;

            isMove = true;
            SmoothMovement = true;
        }
        else
        {
            myRectTrans.SetParent(GameController.Instance.Rotator);
            line.gameObject.SetActive(true);
            isMove = false;
        }
    }
}
