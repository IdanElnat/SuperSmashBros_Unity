using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUps : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Enlarge(other , 1));
    }

    private IEnumerator Enlarge(Collider other , float duration)
    {
 
        Transform target = other.gameObject.transform;
        Vector3 originalScale = target.localScale;
        Vector3 targetScale = originalScale * 2.5f;
        float elapsed = 0f;
        Camera camera = target.GetComponentInChildren<Camera>();
  
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            target.localScale = Vector3.Lerp(originalScale, targetScale, progress);
    
            yield return null;
        }
        
        
        if (camera!= null)
        {

            Vector3 cameraPosition = camera.transform.position;
            Vector3 cameraTargetPosition = cameraPosition+transform.forward/5;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
        
                camera.transform.position = Vector3.Lerp(cameraPosition, cameraTargetPosition, progress);
               
                yield return null;
            }
            camera.transform.position = cameraTargetPosition;
        }
        other.GetComponent<Survivability>().Enlarge();
        target.localScale = targetScale;
        
        Destroy(gameObject);
      
    }

}
