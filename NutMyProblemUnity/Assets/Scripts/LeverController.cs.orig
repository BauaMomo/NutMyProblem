using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OnSwitchEvent : UnityEvent<bool> { }

public class LeverController : MonoBehaviour
{
    GameObject Door;
    DoorController doorController;
    public bool isActive = false;
    [SerializeField] bool canBeDeactivated;

    SpriteRenderer spriteRenderer;
    Sprite LeverActive;
    Sprite LeverInactive;

    public OnSwitchEvent OnSwitch;

    Transform parent;
    public ParticleSystem LeverParticleRed;
    public ParticleSystem LeverParticleBlue;
    public ParticleSystem LeverParticleGreen;
    public ParticleSystem LeverParticleYellow;
    public ParticleSystem LeverParticlePurple;
    public ParticleSystem LeverParticleWhite;
    Sprite LeverRedOn;
    Sprite LeverGreenOn;
    Sprite LeverWhiteOn;
    Sprite LeverPurpleOn;
    Sprite LeverYellowOn;
    Sprite LeverRedOff;
    Sprite LeverGreenOff;
    Sprite LeverWhiteOff;
    Sprite LeverPurpleOff;
    Sprite LeverYellowOff;

    // Start is called before the first frame update
    void Start()
    {
        parent = this.transform.parent;
        Door = transform.parent.gameObject;
        doorController = Door.GetComponent<DoorController>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (OnSwitch == null) OnSwitch = new OnSwitchEvent();

        LeverRedOn = Resources.Load<Sprite>("Background2/lever_red_on");
        LeverGreenOn = Resources.Load<Sprite>("Background2/lever_green_on");
        LeverWhiteOn = Resources.Load<Sprite>("Background2/lever_white_on");
        LeverPurpleOn = Resources.Load<Sprite>("Background2/lever_purple_on");
        LeverYellowOn = Resources.Load<Sprite>("Background2/lever_yellow_on");

        LeverRedOff = Resources.Load<Sprite>("Background2/lever_red_off");
        LeverGreenOff = Resources.Load<Sprite>("Background2/lever_green_off");
        LeverWhiteOff = Resources.Load<Sprite>("Background2/lever_white_off");
        LeverPurpleOff = Resources.Load<Sprite>("Background2/lever_purple_off");
        LeverYellowOff = Resources.Load<Sprite>("Background2/lever_yellow_off");
    }

    public void SwitchLever()
    {
        switch (isActive)
        {
            case true:
                StopParticle();
                if (canBeDeactivated) isActive = false;
                break;
            case false:
                StartParticle();
                isActive = true;
                break;
        }

        FindObjectOfType<AudioManager>().Play("Lever");
        OnSwitch.Invoke(isActive);
    }

    // Update is called once per frame
    void Update()
    {
        ChangeLeverColor();

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
    void StartParticle()
    {
        switch(parent.GetComponent<DoorController>().doorColor)
        {
            case DoorController.Color.red:
                LeverParticleRed.Play();
                break;            
            case DoorController.Color.white:
                LeverParticleWhite.Play();
                break;            
            case DoorController.Color.green:
                LeverParticleGreen.Play();
                break;            
            case DoorController.Color.purple:
                LeverParticlePurple.Play();
                break;            
            case DoorController.Color.yellow:
                LeverParticleYellow.Play();
                break;
                /*case DoorController.Color.white:
                    LeverParticleWhite.Play();
                    break;*/
        }
    }
    void StopParticle()
    {
        switch (parent.GetComponent<DoorController>().doorColor)
        {
            case DoorController.Color.red:
                LeverParticleRed.Stop();
                break;
            case DoorController.Color.white:
                LeverParticleWhite.Stop();
                break;
            case DoorController.Color.green:
                LeverParticleGreen.Stop();
                break;
            case DoorController.Color.purple:
                LeverParticlePurple.Stop();
                break;
            case DoorController.Color.yellow:
                LeverParticleYellow.Stop();
                break;
                /*case DoorController.Color.white:
                    LeverParticleWhite.Stop();
                    break;*/
        }
    }
    void ChangeLeverColor()
    {
        try
        {
            switch (doorController.doorColor)
            {
                case DoorController.Color.red:
                    LeverActive = LeverRedOn;
                    LeverInactive = LeverRedOff;
                    break;
                case DoorController.Color.green:
                    LeverActive = LeverGreenOn;
                    LeverInactive = LeverGreenOff;
                    break;
                case DoorController.Color.white:
                    LeverActive = LeverWhiteOn;
                    LeverInactive = LeverWhiteOff;
                    break;
                case DoorController.Color.purple:
                    LeverActive = LeverPurpleOn;
                    LeverInactive = LeverPurpleOff;
                    break;
                case DoorController.Color.yellow:
                    LeverActive = LeverYellowOn;
                    LeverInactive = LeverYellowOff;
                    break;
            }
        }
        catch
        {
            LeverActive = LeverRedOn;
            LeverInactive = LeverRedOff;
        }
    }
}
