using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    GameObject DoorOpened;
    GameObject DoorClosed;

    enum Color { red, green, blue, purple, orange }
    [SerializeField] Color doorColor;

    Color oldDoorColor;

    Sprite Door_Red_Opened;
    Sprite Door_Green_Opened;
    Sprite Door_Blue_Opened;
    Sprite Door_Purple_Opened;
    Sprite Door_Orange_Opened;

    Sprite Door_Red_Closed;
    Sprite Door_Green_Closed;
    Sprite Door_Blue_Closed;
    Sprite Door_Purple_Closed;
    Sprite Door_Orange_Closed;

    public bool opened = false;

    // Start is called before the first frame update
    void Start()
    {
        DoorOpened = transform.Find("Door_Open").gameObject;
        DoorClosed = this.gameObject;

        Door_Red_Opened = Resources.Load<Sprite>("Background2/doorway_broken");         //put door sprites here
        Door_Green_Opened = Resources.Load<Sprite>("Background2/");
        Door_Blue_Opened = Resources.Load<Sprite>("Background2/");
        Door_Purple_Opened = Resources.Load<Sprite>("Background2/");
        Door_Orange_Opened = Resources.Load<Sprite>("Background2/");

        Door_Red_Closed = Resources.Load<Sprite>("Background2/door_side_red");
        Door_Green_Closed = Resources.Load<Sprite>("Background2/");
        Door_Blue_Closed = Resources.Load<Sprite>("Background2/");
        Door_Purple_Closed = Resources.Load<Sprite>("Background2/");
        Door_Orange_Closed = Resources.Load<Sprite>("Background2/");
    }

    // Update is called once per frame
    void Update()
    {
        changeDoorColor();
    }

    void changeDoorColor()
    {
        if (doorColor == oldDoorColor) return;

        switch (doorColor)
        {
            case Color.red:
                DoorOpened.GetComponent<SpriteRenderer>().sprite = Door_Red_Opened;
                DoorClosed.GetComponent<SpriteRenderer>().sprite = Door_Red_Closed;
                break;
            case Color.green:
                DoorOpened.GetComponent<SpriteRenderer>().sprite = Door_Green_Opened;
                DoorClosed.GetComponent<SpriteRenderer>().sprite = Door_Green_Closed;
                break;
            case Color.blue:
                DoorOpened.GetComponent<SpriteRenderer>().sprite = Door_Blue_Opened;
                DoorClosed.GetComponent<SpriteRenderer>().sprite = Door_Blue_Closed;
                break;
            case Color.purple:
                DoorOpened.GetComponent<SpriteRenderer>().sprite = Door_Purple_Opened;
                DoorClosed.GetComponent<SpriteRenderer>().sprite = Door_Purple_Closed;
                break;
            case Color.orange:
                DoorOpened.GetComponent<SpriteRenderer>().sprite = Door_Orange_Opened;
                DoorClosed.GetComponent<SpriteRenderer>().sprite = Door_Orange_Closed;
                break;
        }
        //Debug.Log("ping");
        oldDoorColor = doorColor;
    }

    public void OnLeverSwitch(bool _isLeverActive)
    {
        switch (_isLeverActive)
        {
            case true:
                OpenDoor();
                break;
            case false:
                CloseDoor();
                break;
        }
    }

    void OpenDoor()
    {
        if (opened) return;
        opened = !opened;

        DoorClosed.GetComponent<SpriteRenderer>().enabled = false;
        DoorClosed.GetComponent<BoxCollider2D>().enabled = false;
    }

    void CloseDoor()
    {
        if (!opened) return;
        opened = !opened;

        DoorClosed.GetComponent<SpriteRenderer>().enabled = true;
        DoorClosed.GetComponent<BoxCollider2D>().enabled = true;
    }
}
