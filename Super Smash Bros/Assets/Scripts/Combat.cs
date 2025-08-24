using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Combat : MonoBehaviour
{

    [SerializeField] float power = 1f;
    [Range(0f, 100f)]
    [SerializeField] float force = 5f;
    [SerializeField] float hitCoolDown = 1f;
    [SerializeField] float stunLength = 1f;
    float timeSinceLastHit;
    private void Update()
    {
        timeSinceLastHit += Time.deltaTime;
    }
    public void Attack(GameObject otherObject)
    {
        timeSinceLastHit = 0;
        Vector3 direction = (transform.forward + Vector3.up*0.5f).normalized;
        Survivability enemySurvivability = otherObject.GetComponent<Survivability>();
        RagdollController rd = otherObject.GetComponent<RagdollController>();
        enemySurvivability.GetDamaged(power);
        

        enemySurvivability.SetStunTimer(stunLength);

        float enemyRelatedForce = (enemySurvivability.GetPrecentage() + 1) /enemySurvivability.GetHeaviness();
        StartCoroutine(rd.ApplyKnockback(enemyRelatedForce*direction*force, stunLength) );
    }
    public bool CanHit()
    {
        return timeSinceLastHit>hitCoolDown;
    }
    


}
