using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CameraFollow.instance.StartShakeCamera();
        collision.GetComponentInParent<Cow>().ResetCow(); ;
    }
}
