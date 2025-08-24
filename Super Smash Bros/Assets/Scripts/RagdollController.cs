using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UIElements;

public class RagdollController : MonoBehaviour
{
    [SerializeField] Rigidbody ragdollMainBody;
    [SerializeField] Rigidbody mainBody;
    [SerializeField] GameObject mesh;
    private int defaultLayer;
    private int localPlayerLayer;

    Animator animator;
    bool isRagdoll;
   


    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        SetRagdollActive(false);
        defaultLayer = LayerMask.NameToLayer("Default");
        localPlayerLayer = LayerMask.NameToLayer("LocalPlayer");
    }


    public void SetRagdollActive(bool active)
    {

        if (!active) AlignToRagdollPosition(mainBody.gameObject);
        Combat combat = GetComponent<Combat>();
        combat.enabled = !active;
        animator.enabled = !active;
        mainBody.isKinematic = active;
        ColliderSetUp(active);
        if (!active)
        {

            mainBody.velocity = ragdollMainBody.velocity;
        }
        Camera cam = GetComponentInChildren<Camera>();
        if (cam != null)
        {
            ToggleMeshVisibiltyToPlayer(active, cam);
        }

        RigidBodiesSetUp(active);
        

        isRagdoll = active;
    }

    private void ToggleMeshVisibiltyToPlayer(bool active, Camera cam)
    {
        if (active)
        {
            print("hiding");
            HideBody(cam);
        }
        else
        {
            ShowBody(cam);
        }
    }

    private void ColliderSetUp(bool active)
    {
        Collider mainCollider = mainBody.GetComponent<Collider>();
        mainCollider.enabled  = !active;
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            if (col != mainCollider)
                col.enabled = active;
        }
    }

    private void RigidBodiesSetUp(bool active)
    {
        foreach (Rigidbody rigidBody in GetComponentsInChildren<Rigidbody>())
        {
            if (rigidBody != mainBody)
            {
                rigidBody.isKinematic = !active;
                if (active)
                {
                    rigidBody.velocity = Vector3.zero;
                    rigidBody.angularVelocity = Vector3.zero;
                }

            }

        }
    }

    public IEnumerator ApplyKnockback(Vector3 force, float duration)
    {
        SetRagdollActive(true);

        yield return null;
        Camera cam = GetComponentInChildren<Camera>();

        ragdollMainBody.velocity = Vector3.zero;
        ragdollMainBody.angularVelocity = Vector3.zero;
        ragdollMainBody.AddForce(force, ForceMode.Impulse);
        
        if (cam != null)
        {
            StartCoroutine(AlignPlayerWithRagdoll(cam , duration));
        }
        else
        {
            yield return new WaitForSeconds(duration);
            SetRagdollActive(false);
        }
    }
    IEnumerator AlignPlayerWithRagdoll(Camera cam , float duration)
    {
        float timer = 0f;
        float camYPos = cam.transform.position.y;
        Vector3 camLocalPos = cam.transform.localPosition;

        while (timer<=duration)
        {
            timer += Time.deltaTime;

            AlignToRagdollPosition(cam.gameObject);
            yield return null;
        }
        Vector3 ragdollLocalPos = ragdollMainBody.transform.localPosition;

        cam.transform.position.Set(cam.transform.position.x, camYPos, cam.transform.position.z);

        SetRagdollActive(false);
        yield return null;
        cam.transform.localPosition = camLocalPos;
        ragdollMainBody.transform.position.Set(ragdollLocalPos.x, ragdollLocalPos.y, ragdollLocalPos.z);
    }
    public void HideBody(Camera playerCamera)
    {
        // Put the body on the LocalPlayer layer
        SetLayerRecursively(mesh, localPlayerLayer);

        // Make sure the camera ignores the LocalPlayer layer
        playerCamera.cullingMask &= ~(1 << localPlayerLayer);
    }

    
    public void ShowBody(Camera playerCamera)
    {
        // Put the body back on Default (or whatever layer it should normally be)
        SetLayerRecursively(mesh, defaultLayer);

        // Ensure camera can see that layer again
        playerCamera.cullingMask |= (1 << defaultLayer);
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    void AlignToRagdollPosition(GameObject toMove)
    {
        toMove.transform.position = ragdollMainBody.position;

    }
    public Rigidbody GetRagdollMainBody()
    {
        return ragdollMainBody;
    }
    public bool IsRagdoll()
    {
        return isRagdoll;
    }



}
