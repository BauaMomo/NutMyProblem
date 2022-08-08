using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OnSwitchEvent : UnityEvent<bool>
{
}

public class LeverController : MonoBehaviour
{
    public bool isActive = false;
    [SerializeField] bool canBeDeactivated;

    SpriteRenderer spriteRenderer;
    Sprite LeverActive;
    Sprite LeverInactive;

    public OnSwitchEvent OnSwitch;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        LeverActive = Resources.Load<Sprite>("Background2/lever_activated");
        LeverInactive = Resources.Load<Sprite>("Background2/lever_standard");

        if (OnSwitch == null) OnSwitch = new OnSwitchEvent();
    }

    public void SwitchLever()
    {
        switch (isActive)
        {
            case true:
                if (canBeDeactivated) isActive = false;
                break;
            case false:
                isActive = true;
                break;
        }

        FindObjectOfType<AudioManager>().Play("Lever");
        OnSwitch.Invoke(isActive);
    }

    // Update is called once per frame
    void Update()
    {
        switch (isActive)
        {
            case false:
                spriteRenderer.sprite = LeverInactive;
                break;
            case true:
                spriteRenderer.sprite = LeverActive;
                break;
        }
    }
}
