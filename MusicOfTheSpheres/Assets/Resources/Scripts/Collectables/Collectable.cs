using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public GameObject collect_fx;

    protected Animator anim;
    protected GameObject holder;
    protected bool collected;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        holder = anim.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;
        if (other.CompareTag("Player"))
            Collect();
    }

    protected void Collect()
    {
        collected = true;
        anim.SetBool("Collected", true);
        Destroy(holder, anim.GetCurrentAnimatorStateInfo(0).length);
    }
}
