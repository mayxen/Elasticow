using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour
{
    public delegate void SwitchActivated();
    public static event SwitchActivated OnActivated;
    public bool Active { get; set; }

    [SerializeField] Animator animator = null;


    public void SetSwitchOff()
    {
        Active = false;
        animator.SetTrigger("Deactivate");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Active) {
            animator.SetTrigger("Activate");
            Active = true;
            SystemController.instance.PlaySwitchSound(GetComponent<AudioSource>());
            if (OnActivated != null)
                OnActivated();
        }

    }

}
