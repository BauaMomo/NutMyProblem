using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractPrompts : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.MainModule psMain;

    Sprite KeyboardButton;
    Sprite ControllerButton;

    GameObject player;

    String currentControls;

    float sizeIncrease = .1f;
    bool growing = false;
    bool shrinking = false;

    public bool test = false;
    public bool test2 = false;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        player = GameObject.FindGameObjectWithTag("Player");
        psMain = ps.main;

        player.GetComponent<PlayerInput>().controlsChangedEvent.AddListener(OnControlChange);

        KeyboardButton = Resources.Load<Sprite>("Sprites/F_Key_Light");
        ControllerButton = Resources.Load<Sprite>("Sprites/XboxSeriesX_Y");

        transform.localScale *= 0;
    }

    // Update is called once per frame
    void Update()
    {
        SetSprite(currentControls);

        if (growing) transform.localScale = transform.localScale + new Vector3(sizeIncrease, sizeIncrease, sizeIncrease);
        if (transform.localScale.x >= 1) growing = false;

        if (shrinking) transform.localScale = transform.localScale - new Vector3(sizeIncrease, sizeIncrease, sizeIncrease);
        if (transform.localScale.x <= 0.1) shrinking = false;

        if (Vector2.Distance(transform.position, player.transform.position) <= 3f) RespawnParticle();
        if (Vector2.Distance(transform.position, player.transform.position) > 3f) DespawnParticle();
    }

    void RespawnParticle()
    {
        if (transform.localScale.x >= 0.1f) return;

        ps.Stop();
        transform.localScale = transform.localScale * 0;
        growing = true;
        ps.Play();

    }

    void DespawnParticle()
    {
        if (transform.localScale.x <= 0.1f) return;
        shrinking = true;
    }

    void ResetParticle()
    {
        ps.Stop();
        ps.Play();
    }

    void SetSprite(string _ctrlScheme)
    {   
        
    }

    public void OnControlChange(PlayerInput input)
    {
        currentControls = input.currentControlScheme;

        if (input.currentControlScheme == "Keyboard&Mouse") ps.textureSheetAnimation.SetSprite(0, KeyboardButton);
        if (input.currentControlScheme == "Gamepad") ps.textureSheetAnimation.SetSprite(0, ControllerButton);
    }
}
