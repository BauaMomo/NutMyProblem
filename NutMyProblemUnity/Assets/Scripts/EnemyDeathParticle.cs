using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathParticle : MonoBehaviour
{

    public ParticleSystem DeathParticle;
    void Start()
    {
        DeathParticle.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
