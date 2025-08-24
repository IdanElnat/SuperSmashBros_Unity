using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPool : MonoBehaviour
{
    [SerializeField] GameObject killParticle = null;
    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(KillOutOfBounds(collision));
    }

    private IEnumerator KillOutOfBounds(Collision collision)
    {
        Destroy(collision.gameObject);
        GameObject particle = Instantiate(killParticle , collision.transform.position, Quaternion.LookRotation(Vector3.up) ,transform);
        yield return new WaitForSeconds(5f);
        Destroy(particle);
    }
}
