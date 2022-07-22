using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractPrompts : MonoBehaviour
{
    GameObject child;

    SpriteRenderer sr;

    Sprite KeyboardButton;
    Sprite ControllerButton;

    GameObject player;

    String currentControls;

    public float maxMoveRange;
    public float speed;
    public float smoothingStrength;
    float lerpPos;
    float curveLerpPos;
    Vector2 startPos = new Vector2(0, 0);
    Vector2 rPoint1;
    Vector2 rPoint2 = new Vector2();

    Vector2 curvePointStart = new Vector2();
    Vector2 curvePointEnd = new Vector2();

    float sizeIncrease = .1f;
    bool growing = false;
    bool shrinking = false;

    public bool test = false;
    public bool test2 = false;

    // Start is called before the first frame update
    void Start()
    {
        child = transform.Find("Sprite").gameObject;
        startPos = child.transform.localPosition;

        sr = child.GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");

        player.GetComponent<PlayerInput>().controlsChangedEvent.AddListener(OnControlChange);

        KeyboardButton = Resources.Load<Sprite>("Sprites/F_Key_Light");
        ControllerButton = Resources.Load<Sprite>("Sprites/XboxSeriesX_Y");

        child.transform.localScale *= 0;
        rPoint1 = new Vector2(maxMoveRange/2, 0);
        GetNewRandomPoints(ref rPoint1, ref rPoint2);
    }

    // Update is called once per frame
    void Update()
    {
        SetSprite(currentControls);

        if (growing) child.transform.localScale = child.transform.localScale + new Vector3(sizeIncrease, sizeIncrease, sizeIncrease);
        if (child.transform.localScale.x >= 1) growing = false;

        if (shrinking) child.transform.localScale = child.transform.localScale - new Vector3(sizeIncrease, sizeIncrease, sizeIncrease);
        if (child.transform.localScale.x <= 0.1) shrinking = false;

        if (Vector2.Distance(transform.position, player.transform.position) <= 3f) RespawnParticle();
        if (Vector2.Distance(transform.position, player.transform.position) > 3f) DespawnParticle();

        MoveSprite();
    }

    void MoveSprite()
    {
        float localSpeed = speed * 0.01f;
        lerpPos += localSpeed;

        Vector2 v1 = 2 * startPos - 4 * rPoint1 + 2 * rPoint2;
        Vector2 v2 = -2 * startPos + 2 * rPoint1;

        if (curveLerpPos == 0) curveLerpPos = 0.001f;

        curveLerpPos = curveLerpPos + localSpeed / (Vector2.Distance(curveLerpPos * v1, v2));

        curvePointStart = Vector2.Lerp(startPos, rPoint1, curveLerpPos);
        curvePointEnd = Vector2.Lerp(rPoint1, rPoint2, curveLerpPos);

        child.transform.localPosition = Vector2.MoveTowards(child.transform.localPosition, Vector2.Lerp(curvePointStart, curvePointEnd, curveLerpPos), 
                                        Vector2.Distance(child.transform.localPosition, Vector2.Lerp(curvePointStart, curvePointEnd, curveLerpPos)) * smoothingStrength);

        Debug.DrawLine(startPos + (Vector2)transform.position, rPoint1 + (Vector2)transform.position, Color.blue);
        Debug.DrawLine(rPoint1 + (Vector2)transform.position, rPoint2 + (Vector2)transform.position, Color.blue);
        Debug.DrawLine(curvePointStart + (Vector2)transform.position, curvePointEnd + (Vector2)transform.position, Color.red);

        if (curveLerpPos >= 1) GetNewRandomPoints(ref rPoint1, ref rPoint2);
        if (curveLerpPos >= 1) curveLerpPos = 0;

    }

    void GetNewRandomPoints(ref Vector2 _p1, ref Vector2 _p2)
    {
        Vector2 minPos = new Vector2(-maxMoveRange, -maxMoveRange);
        Vector2 maxPos = new Vector2(maxMoveRange, maxMoveRange);

        Vector2 p2Copy = _p2;

        startPos = p2Copy;
        _p1 = p2Copy + (p2Copy - _p1).normalized * (maxMoveRange / 2);
        p2Copy = GetRandomPointAroundP2(_p2);

        _p2 = p2Copy;

        //Debug.Log(_p1 + ", " + _p2);
    }

    Vector2 GetRandomPointAroundP2(Vector2 _p2)
    {
        Vector2 minPos = new Vector2(-maxMoveRange, -maxMoveRange);
        Vector2 maxPos = new Vector2(maxMoveRange, maxMoveRange);

        Vector2 newP2 = new Vector2();

        for (int i = 0; i < 20; i ++)
        {
            Vector2 random = new Vector2(UnityEngine.Random.Range(minPos.x, maxPos.x), UnityEngine.Random.Range(minPos.y, maxPos.y));
            newP2 = rPoint1 + (random - rPoint1).normalized * (maxMoveRange /2);
            if (Vector2.Distance(newP2, new Vector2(0, 0)) > maxMoveRange) newP2 = new Vector2(0, 0);
            else break;
        }
        return newP2;
    }

    void RespawnParticle()
    {
        if (child.transform.localScale.x >= 0.1f) return;


        child.transform.localScale = child.transform.localScale * 0;
        growing = true;

    }

    void DespawnParticle()
    {
        if (child.transform.localScale.x <= 0.1f) return;
        shrinking = true;
    }

    void SetSprite(string _ctrlScheme)
    {

    }

    public void OnControlChange(PlayerInput input)
    {
        currentControls = input.currentControlScheme;

        if (input.currentControlScheme == "Keyboard&Mouse") sr.sprite = KeyboardButton;
        if (input.currentControlScheme == "Gamepad") sr.sprite = ControllerButton;
    }
}
