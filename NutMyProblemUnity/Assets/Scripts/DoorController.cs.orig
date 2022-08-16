using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    GameObject DoorOpened;
    GameObject DoorClosed;

    public enum Color { red, green, white, purple, yellow }
    public Color doorColor;

    Color oldDoorColor;

    Sprite Door_Red_Opened;
    Sprite Door_Green_Opened;
    Sprite Door_White_Opened;
    Sprite Door_Purple_Opened;
    Sprite Door_Yellow_Opened;

    Sprite Door_Red_Closed;
    Sprite Door_Green_Closed;
    Sprite Door_White_Closed;
    Sprite Door_Purple_Closed;
    Sprite Door_Yellow_Closed;

    public bool opened = false;

    // Start is called before the first frame update
    void Start()
    {
        DoorOpened = transform.Find("Door_Open").gameObject;
        DoorClosed = transform.Find("Door_Closed").gameObject;

        Door_Red_Opened = Resources.Load<Sprite>("Background2/GateSmall_Open_Red");         //put door sprites here
        Door_Green_Opened = Resources.Load<Sprite>("Background2/GateSmall_Open_Green");
        Door_White_Opened = Resources.Load<Sprite>("Background2/GateSmall_Open_White");
        Door_Purple_Opened = Resources.Load<Sprite>("Background2/GateSmall_Open_Purple");
        Door_Yellow_Opened = Resources.Load<Sprite>("Background2/GateSmall_Open_Yellow");

        Door_Red_Closed = Resources.Load<Sprite>("Background2/GateSmall_Closed_Red");
        Door_Green_Closed = Resources.Load<Sprite>("Background2/GateSmall_Closed_Green");
        Door_White_Closed = Resources.Load<Sprite>("Background2/GateSmall_Closed_White");
        Door_Purple_Closed = Resources.Load<Sprite>("Background2/GateSmall_Closed_Purple");
        Door_Yellow_Closed = Resources.Load<Sprite>("Background2/GateSmall_Closed_Yellow");
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
            case Color.white:
                DoorOpened.GetComponent<SpriteRenderer>().sprite = Door_White_Opened;
                DoorClosed.GetComponent<SpriteRenderer>().sprite = Door_White_Closed;
                break;
            case Color.purple:
                DoorOpened.GetComponent<SpriteRenderer>().sprite = Door_Purple_Opened;
                DoorClosed.GetComponent<SpriteRenderer>().sprite = Door_Purple_Closed;
                break;
            case Color.yellow:
                DoorOpened.GetComponent<SpriteRenderer>().sprite = Door_Yellow_Opened;
                DoorClosed.GetComponent<SpriteRenderer>().sprite = Door_Yellow_Closed;
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

        DoorClosed.SetActive(false);
        DoorOpened.SetActive(true);
    }

    void CloseDoor()
    {
        if (!opened) return;
        opened = !opened;

        DoorClosed.SetActive(true);
        DoorOpened.SetActive(false);
    }
}