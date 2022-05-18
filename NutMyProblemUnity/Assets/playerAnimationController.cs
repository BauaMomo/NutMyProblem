using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimationController : MonoBehaviour
{
    playerController playerController;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<playerController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
