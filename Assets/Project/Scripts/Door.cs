using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    public bool isOpen;

    [SerializeField] Animator animator = null;

    private void Start()
    {
        Switch.OnActivated += CheckSwitches;
    }

    private void OnDestroy()
    {
        Switch.OnActivated -= CheckSwitches;
    }

    void CheckSwitches()
    {
        if(SystemController.instance.CheckSwitches()) {
            return;
        }
        SystemController.instance.PlayDoorSound(GetComponent<AudioSource>());
        animator.SetTrigger("OpenDoor");
        isOpen = true;
    }

    public void ResetDoor()
    {
        animator.SetTrigger("CloseDoor");
        isOpen = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOpen) {
            SystemController.instance.ToggleNextLvl(GetComponent<AudioSource>());
        }
    }

    public void ToggleEffects()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource.isPlaying)
            audioSource.Stop();
        else
            audioSource.Play();
        
    }
}
