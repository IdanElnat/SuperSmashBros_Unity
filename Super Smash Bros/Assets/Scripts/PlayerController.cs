using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float range = 5f;
    bool isLedged = false;
    Survivability survivability;
    RagdollController rd;
  
    private void Start()
    {
        survivability = GetComponent<Survivability>();
        rd = GetComponent<RagdollController>();

   
    }

    void Update()
    {

        if (survivability.IsStunned())
        {
           return;
        }

        if (HasHit()) return;
    }
    bool HasHit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("cheking raycast");
            RaycastHit[] hits = RayCastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject != gameObject)
                {
                    HandleRaycast(hit);
                    return true;
                }
            }

        }
        return false;
    }
    RaycastHit[] RayCastAllSorted()
    {
        RaycastHit[] hits = Physics.RaycastAll(GetMouseRay(), range);
        float[] distances = new float[hits.Length];
        for (int i = 0; i<hits.Length; i++)
        {
            distances[i] = hits[i].distance;
        }
        Array.Sort(distances, hits);
        return hits;
    }
    void HandleRaycast(RaycastHit hit)
    {
        GameObject otherObject = hit.collider.gameObject;
        if (otherObject.GetComponent<Survivability>() != null)
        {
            gameObject.GetComponent<Combat>().Attack(otherObject);
        }
        else if (otherObject.GetComponent<Ledge>() != null && hit.point.y+1 >= transform.position.y)
        {
            
            GrabLedge(hit);
        }
    
    }

    private void GrabLedge(RaycastHit hit)
    {
        Debug.Log("grabbing");
        StartCoroutine(MovingToLedge(hit , 0.5f));
        StartCoroutine(WaitForJump());

    }

    private IEnumerator MovingToLedge(RaycastHit hit , float duration)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = hit.point -transform.forward;
        print(transform.forward);
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = targetPosition;
    }

    private IEnumerator WaitForJump()
    {
        isLedged = true;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        FirstPersonController controller = gameObject.GetComponent<FirstPersonController>();
        rb.useGravity = false;
        controller.playerCanMove = false;

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null; // wait for the next frame
        }

        rb.useGravity = true;
        controller.playerCanMove = true;
        isLedged = false;
    }
    public bool IsLedged()
    {
        return isLedged;
    }
    private static Ray GetMouseRay()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }
}
