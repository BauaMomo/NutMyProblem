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

    Transform parent;
    public ParticleSystem LeverParticleRed;
    public ParticleSystem LeverParticleBlue;
    public ParticleSystem LeverParticleGreen;
    public ParticleSystem LeverParticleOrange;
    public ParticleSystem LeverParticlePurple;
    public ParticleSystem LeverParticleWhite;

    // Start is called before the first frame update
    void Start()
    {
        parent = this.transform.parent;
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
                StopParticle();
                if (canBeDeactivated) isActive = false;
                break;
            case false:
                StartParticle();
                isActive = true;
                break;
        }

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
    void StartParticle()
    {
        switch(parent.GetComponent<DoorController>().doorColor)
        {
            case DoorController.Color.red:
                LeverParticleRed.Play();
                break;            
            case DoorController.Color.blue:
                LeverParticleBlue.Play();
                break;            
            case DoorController.Color.green:
                LeverParticleGreen.Play();
                break;            
            case DoorController.Color.purple:
                LeverParticlePurple.Play();
                break;            
            case DoorController.Color.orange:
                LeverParticleOrange.Play();
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
            case DoorController.Color.blue:
                LeverParticleBlue.Stop();
                break;
            case DoorController.Color.green:
                LeverParticleGreen.Stop();
                break;
            case DoorController.Color.purple:
                LeverParticlePurple.Stop();
                break;
            case DoorController.Color.orange:
                LeverParticleOrange.Stop();
                break;
                /*case DoorController.Color.white:
                    LeverParticleWhite.Stop();
                    break;*/
        }
    }
}
