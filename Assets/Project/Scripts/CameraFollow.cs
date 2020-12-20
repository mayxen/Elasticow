using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public struct PointInSpace
    {
        public Vector3 Position;
    }
    public static CameraFollow instance;
    public Transform target;

    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private float speed = 5;
    private Vector3 lastPosition = Vector3.zero;
    private Queue<PointInSpace> pointsInSpace = new Queue<PointInSpace>();

    private float shakeMagnitude = 0.5f;
    private float shakeDuration = 0f;
    private float CameraZPos;

    Vector3 initialPosition;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        CameraZPos = Camera.main.transform.position.z;
    }

    public void StartShakeCamera()
    {
        initialPosition = transform.position;
        shakeDuration = 0.3f;
    }

    private void LateUpdate()
    {
        ShakeCamera();
        FollowHead();
    }

    private void ShakeCamera()
    {
        if (shakeDuration > 0f) {
            Vector3 shakeDirection = Random.insideUnitSphere;
            shakeDirection.z = CameraZPos;
            transform.localPosition = initialPosition + shakeDirection * shakeMagnitude;
            shakeDuration -= Time.deltaTime;
        }
    }

    private void FollowHead()
    {
        pointsInSpace.Enqueue(new PointInSpace() {
            Position = new Vector3(target.position.x, target.position.y, transform.position.z)
        });
        while (pointsInSpace.Count > 0) {
            transform.position = Vector3.Lerp(transform.position, pointsInSpace.Dequeue().Position + offset, Time.deltaTime * speed);
        }
    }

    public void AddPointInSpace()
    {
        if (!lastPosition.Equals(target.position)) {
            lastPosition = target.position;
            pointsInSpace.Enqueue(new PointInSpace() {
                Position = new Vector3(target.position.x, target.position.y, transform.position.z)
            });
        }
    }
}
