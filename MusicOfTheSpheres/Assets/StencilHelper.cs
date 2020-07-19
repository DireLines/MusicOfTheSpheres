using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StencilHelper : MonoBehaviour
{

    [SerializeField] [Range(0.1f, 2f)] float cutoutSpeed;
    [SerializeField] AnimationCurve tweenCurve;
    [SerializeField] LayerMask layerMask;

    RaycastHit hit;
    Camera cam;
    [SerializeField] bool playerInSight;
    float t;
    Vector3 size;

    void Start()
    {
        cam = Camera.main;
        playerInSight = true;
        t = 0f;
        size = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        Vector3 origin = cam.transform.position;
        Vector3 direction = transform.position - cam.transform.position;
        float maxDistance = direction.magnitude * 1.1f;
        if (Physics.Raycast(origin, direction, out hit, maxDistance, layerMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (!playerInSight)
                {
                    playerInSight = true;
                }
            }
            else
            {
                if (playerInSight)
                {
                    playerInSight = false;
                }
            }
        }

        t += cutoutSpeed * Time.deltaTime * (playerInSight ? -1 : 1);
        t = Mathf.Clamp01(t);
        transform.localScale = Vector3.Lerp(Vector3.zero, size, tweenCurve.Evaluate(t));
    }
}
