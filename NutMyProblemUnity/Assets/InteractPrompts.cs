using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractPrompts : MonoBehaviour
{
    GameObject child;
    GameObject player;

    SpriteRenderer sr;
    Sprite KeyboardButton;
    Sprite ControllerButton;

    [SerializeField] float maxMoveRange;
    [SerializeField] float speed;
    float curveLerpPos;

    Vector2 startPos = new Vector2(0, 0);
    Vector2 rPoint1;
    Vector2 rPoint2 = new Vector2();

    Vector2 curvePointStart = new Vector2();
    Vector2 curvePointEnd = new Vector2();

    float spriteSizeIncrease = .2f;
    float spriteScale;
    Vector3 spriteStartScale;
    bool growing = false;
    bool shrinking = false;

    // Start is called before the first frame update
    void Start()
    {
        child = transform.Find("Sprite").gameObject;

        sr = child.GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");

        KeyboardButton = Resources.Load<Sprite>("Sprites/E_Key_Light");
        ControllerButton = Resources.Load<Sprite>("Sprites/XboxSeriesX_Y");

        player.GetComponent<PlayerInput>().controlsChangedEvent.AddListener(OnControlChange);

        startPos = child.transform.localPosition;
        spriteStartScale = child.transform.localScale;

        rPoint1 = new Vector2(maxMoveRange/2, 0);
        GetNewRandomPoints(ref rPoint1, ref rPoint2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {        
        if (growing) spriteScale += spriteSizeIncrease;
        if (shrinking) spriteScale -= spriteSizeIncrease;

        child.transform.localScale = spriteStartScale * spriteScale;

        if (Vector2.Distance(transform.position, player.transform.position) < 3f) RespawnPrompt();
        if (Vector2.Distance(transform.position, player.transform.position) > 3f) DespawnPrompt();

        MoveSprite();
    }

    void MoveSprite()
    {
        float localSpeed = speed * 0.01f;
        curveLerpPos += localSpeed;

        curvePointStart = Vector2.Lerp(startPos, rPoint1, curveLerpPos);    //moves the sprite along a bezier curve
        curvePointEnd = Vector2.Lerp(rPoint1, rPoint2, curveLerpPos);

        child.transform.localPosition = Vector2.Lerp(curvePointStart, curvePointEnd, curveLerpPos);

        Debug.DrawLine(startPos + (Vector2)transform.position, rPoint1 + (Vector2)transform.position, Color.blue);      // Debug lines for looking at exclusive bezier curve bts :)
        Debug.DrawLine(rPoint1 + (Vector2)transform.position, rPoint2 + (Vector2)transform.position, Color.blue);
        Debug.DrawLine(curvePointStart + (Vector2)transform.position, curvePointEnd + (Vector2)transform.position, Color.red);

        if (curveLerpPos >= 1)
        {
            GetNewRandomPoints(ref rPoint1, ref rPoint2);
            curveLerpPos = 0;
        }
    }

    void GetNewRandomPoints(ref Vector2 _p1, ref Vector2 _p2)
    {
        startPos = _p2;                                                 //sets all points for next bezier curve
        _p1 = _p2 + (_p2 - _p1).normalized * (maxMoveRange / 2);        //sets middle point in a straight line after the previous end point

        int tries = 0;

        for (int i = 0; i < 20; i++)
        {
            tries = i;
            Vector2 newRandomPoint = UnityEngine.Random.insideUnitCircle * maxMoveRange;            //random point inside the maxMoveRange
            _p2 = _p1 + (newRandomPoint - _p1).normalized * (maxMoveRange / 2);                     //sets new end point on the vector to the random point, so the distance between points is always the same
            if (Vector2.Distance(_p2, new Vector2(0, 0)) > maxMoveRange) _p2 = new Vector2(0, 0);   //this loop only runs 20 times to prevent lag
            else break;                                                                            //if it finds a point, it cancels immediately, if not the new point is (0,0)
        }
        tries += 1;
        //if(tries > 1) Debug.Log(tries);
    }

    void RespawnPrompt()
    {
        if (spriteScale >= 0.9f)
        {
            growing = false;
            spriteScale = 1;
            return;
        }
        if(shrinking) shrinking = false;

        growing = true;
    }

    void DespawnPrompt()
    {
        if (spriteScale <= 0.1f)
        {
            shrinking = false;
            spriteScale = 0;
            return;
        }
        if(growing) growing = false;

        shrinking = true;
    }

    public void OnControlChange(PlayerInput input)
    {
        if (input.currentControlScheme == "Keyboard&Mouse") sr.sprite = KeyboardButton;
        if (input.currentControlScheme == "Gamepad") sr.sprite = ControllerButton;
    }
}
