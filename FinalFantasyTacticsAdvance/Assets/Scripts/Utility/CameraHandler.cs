using System.Collections;
using UnityEngine;

public class CameraHandler : Singleton<CameraHandler>
{
    [SerializeField] float smoothSpeed;
    private Transform target;
    Vector3 startingPos;
    private void Start()
    {
        startingPos = transform.position;
    }
    IEnumerator Movecamera()
    {
        // if (target is null)
        //     yield break;
        // Vector3 desiredPosition = target.position + startingPos;
        // float elapsedTime = 0;
        // while (elapsedTime < smoothSpeed)
        // {
        //     elapsedTime += Time.deltaTime;
        //     float t= elapsedTime / smoothSpeed;
        //     t=Mathf.Clamp01(t);
        //     transform.position = Vector3.Lerp(transform.position,desiredPosition, t);
        //     yield return null;  
        // }
        //
        // transform.position = desiredPosition;


        while(target!=null)
        {
            Vector3 desiredPosition = target.position + startingPos;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed*Time.deltaTime);
            transform.position = smoothedPosition;
            yield return null;
        }
    }
    public void SetTarget(GameObject newTarget)
    {
        if (newTarget != null)
        {
            StopCoroutine(Movecamera());
            target = newTarget.transform;
            StartCoroutine(Movecamera());
        }
    }
}
