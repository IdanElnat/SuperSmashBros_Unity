using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Survivability : MonoBehaviour
{
    [SerializeField] float precentage = 0f;
    [SerializeField] float heaviness = 1f;
    [SerializeField] float defaultPrecentageIncrease = 10;
    float hitstunTimer;
    private bool Enlarged = false;

    void Start()
    {
        hitstunTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (hitstunTimer > 0f)
            hitstunTimer -= Time.deltaTime;
    }
    public float GetPrecentage()
    {
        return precentage;
    }
    public void GetDamaged(float power)
    {
        Debug.Log("got damagde");
        precentage += defaultPrecentageIncrease*power;


    }
    public void SetStunTimer(float time)
    {
        hitstunTimer = time;
    }
    public bool IsStunned()
    {
        return hitstunTimer > 0f;
    }
    public void Enlarge()
    {
        this.Enlarged = true;
        
    }
    public bool IsEnlarged()
    {
        return Enlarged;
    }
    public float GetHeaviness()
    {
        return heaviness;
    }
}
