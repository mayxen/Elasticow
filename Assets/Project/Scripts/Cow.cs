using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cow : MonoBehaviour
{
    [Header("basic")]
    [SerializeField] protected List<Transform> bodys = new List<Transform>();
    [SerializeField] float raycastLenght = 0.3f;
    [SerializeField] Transform Head = null;
    [SerializeField] Transform Body = null;
    [SerializeField] Transform Legs = null;
    [Header("Prefabs")]
    [SerializeField] GameObject BodyPrefab = null;
    [SerializeField] GameObject BodyLPrefab = null;
    [SerializeField] SoundList SoundList = null;
    [SerializeField] AudioSource audioSource = null;


    public enum Direction { up, left, right, down }
    Direction lastDirection = Direction.up;

    Transform lastBody;
    [SerializeField] float velocity = 1f;

    Input controls;
    Input Controls {
        get {
            if (controls != null) { return controls; }
            return controls = new Input();
        }
    }
    Vector2 headInitialPos;
    Vector2 bodyInitialPos;
    Vector2 legInitialPos;
    Vector2 dirMov;
    LayerMask _obstacleLayerMask;
    float extendSprite;
    int bodyTotal = 0;

    private void Start()
    {
        SaveInitialPos();
        SetEvents();
        extendSprite = Body.GetComponent<Renderer>().bounds.extents.x;
    }

    void SetEvents()
    {
        Controls.Enable();
        Controls.Base.Player.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        _obstacleLayerMask = LayerMask.GetMask("Obstacle");
    }

    private void LateUpdate()
    {
        Move();
    }

    protected void SaveInitialPos()
    {
        headInitialPos = Head.position;
        bodyInitialPos = Body.position;
        legInitialPos = Legs.position;
        lastBody = Body;
        bodys.Add(Body);
    }

    protected void AddBodyToCow(Transform body)
    {
        bodys.Add(body);
    }
    public void ToggleEffects()
    {
        if(audioSource.isPlaying)
            audioSource.Stop();
        else
            audioSource.Play();
    }
    public void ResetCow()
    {
        while (bodys.Count != 1) {
            Transform body = bodys.FindLast(i => i);
            bodys.Remove(body);
            Destroy(body.gameObject);
        }
        ResetMovement();
        SystemController.instance.ResetSwitches(GetComponent<AudioSource>());
        lastBody = Body;
        Body.transform.localScale = new Vector3(1f, 1f, 1f);
        lastDirection = Direction.up;
        Head.rotation = Quaternion.Euler(0f, 0f, 0f);
        Head.transform.position = headInitialPos;
        Body.transform.position = bodyInitialPos;
        Legs.transform.position = legInitialPos;
    }

    void SetMovement(Vector2 movement)
    {
        if (dirMov != Vector2.zero) return;
        if (movement.x != 0)
            dirMov = new Vector2(movement.x, 0f);
        else if (movement.y != 0)
            dirMov = new Vector2(0f, movement.y);
    }

    void ResetMovement() => dirMov = Vector2.zero;

    void Move()
    {
        Vector2 movement = new Vector2(dirMov.x * (velocity / 2.96f) * Time.deltaTime
            , dirMov.y * (velocity / 3f) * Time.deltaTime);
        if (movement == Vector2.zero) {
            return;
        }
        MoveHead(movement);
    }

    private void MoveHead(Vector2 movement)
    {
        Vector2 initalPos = new Vector2(Head.position.x, Head.position.y);
        RaycastHit2D rayCastHit = Physics2D.Raycast(initalPos, movement, raycastLenght, _obstacleLayerMask);
        Debug.DrawRay(initalPos, movement, Color.red, 1f);
        if (rayCastHit.collider != null) {
            ResetMovement();
            return;
        }
        Direction newDirection = CheckDirection(movement);
        CreateOrStretchBody(newDirection);
        Head.position += new Vector3(movement.x, movement.y, 0f);
    }

    private Direction CheckDirection(Vector2 movement)
    {
        if (movement.x != 0) {
            if (movement.x > 0) return Direction.right;
            if (movement.x < 0) return Direction.left;
        } else if (movement.y != 0) {
            if (movement.y > 0) return Direction.up;
            if (movement.y < 0) return Direction.down;
        }
        Debug.LogError("I cant find any direction possible");
        return Direction.right;
    }

    private void CreateOrStretchBody(Direction newDirection)
    {
        if (newDirection != lastDirection) {
            Transform bodyInstance = CreateBody();
            PrepareBody(bodyInstance, newDirection);
        } else if (lastBody.localScale.y >= 2f) {
            Transform bodyInstance = CreateBody();
            PrepareBody(bodyInstance, newDirection);
        } else {
            lastBody.localScale += new Vector3(0, 0.22f * (velocity * 2.5f * Time.deltaTime), 0);
        }
    }

    private void PrepareBody(Transform bodyInstance, Direction newDirection)
    {
        switch (newDirection) {
            case Direction.up:

            if (lastDirection != newDirection) {
                Transform bodyLInstance = CreateLBody(newDirection);
                Head.rotation = Quaternion.Euler(0f, 0f, 0f);
                bodyLInstance.rotation = lastDirection == Direction.left
                    ? Quaternion.Euler(0f, 0f, 270f)
                    : Quaternion.Euler(0f, 0f, 0f);
                Head.localPosition += new Vector3(0f, 0.7f, 0f);
                bodyInstance.position = bodyLInstance.position + new Vector3(0f, bodyLInstance.GetComponent<Renderer>().bounds.extents.y, 0f);
                bodyLInstance.position += new Vector3(0f, 0.04f, 0f);
            } else {
                FixHeadLocation(new Vector3(0f, 0.09f, 0f));
                bodyInstance.position = lastBody.position + new Vector3(0f, 1.2f, 0f);
            }

            break;
            case Direction.down:
            if (lastDirection != newDirection) {
                Transform bodyLInstance = CreateLBody(newDirection);
                Head.rotation = Quaternion.Euler(0f, 0f, 180f);
                bodyLInstance.rotation = lastDirection == Direction.left
                    ? Quaternion.Euler(0f, 0f, 180f)
                    : Quaternion.Euler(0f, 0f, 90);
                Head.localPosition += new Vector3(0f, -0.7f, 0f);
                bodyInstance.position = bodyLInstance.position + new Vector3(0f, -bodyLInstance.GetComponent<Renderer>().bounds.extents.y, 0f);
                bodyLInstance.position += new Vector3(0f, -0.04f, 0f);
            } else {
                FixHeadLocation(new Vector3(0f, -0.09f, 0f));
                bodyInstance.position = lastBody.position + new Vector3(0f, -1.2f, 0f);
            }
            break;
            case Direction.right:
            if (lastDirection != newDirection) {
                Transform bodyLInstance = CreateLBody(newDirection);
                Head.rotation = Quaternion.Euler(0f, 0f, -90f);
                bodyLInstance.rotation = lastDirection == Direction.up
                    ? Quaternion.Euler(0f, 0f, 180f)
                    : Quaternion.Euler(0f, 0f, 270f);
                Head.localPosition += new Vector3(Body.GetComponent<Renderer>().bounds.extents.x * 2, 0f, 0f);
                bodyInstance.position = bodyLInstance.position + new Vector3(extendSprite, 0f, 0f);
                bodyLInstance.localPosition += new Vector3(0.04f, 0f, 0f);
            } else {
                FixHeadLocation(new Vector3(0.09f, 0f, 0f));
                bodyInstance.position = lastBody.position + new Vector3(1.2f, 0f, 0f);
            }

            break;
            case Direction.left:

            if (lastDirection != newDirection) {
                Transform bodyLInstance = CreateLBody(newDirection);
                Head.rotation = Quaternion.Euler(0f, 0f, 90f);
                bodyLInstance.rotation = lastDirection == Direction.up
                    ? Quaternion.Euler(0f, 0f, 90f)
                    : Quaternion.Euler(0f, 0f, 0f);
                Head.localPosition += new Vector3(-0.55f, 0f, 0f);
                bodyInstance.position = bodyLInstance.position + new Vector3(-0.2f, 0f, 0f);
            } else {
                FixHeadLocation(new Vector3(-0.09f, 0f, 0f));
                bodyInstance.position = lastBody.position + new Vector3(-1.2f, 0f, 0f);
            }

            break;
        }
        bodyInstance.rotation = Head.rotation;
        bodyInstance.localScale = new Vector3(1f, 0.1f, 1f);
        lastBody = bodyInstance;
        lastDirection = newDirection;
    }
    private Transform CreateBody()
    {
        Transform bodyInstance = Instantiate(BodyPrefab).transform;
        bodyInstance.localRotation = Quaternion.Euler(lastBody.rotation.x
            , 180f * UnityEngine.Random.Range(0, 2)
            , lastBody.rotation.z);
        bodyInstance.SetParent(transform);
        bodys.Add(bodyInstance);
        bodyTotal++;
        return bodyInstance;
    }

    private void FixHeadLocation(Vector3 fix)
    {
        if (bodyTotal % 8 == 0) {
            Head.position += fix;
        }
    }
    private Transform CreateLBody(Direction newDirection)
    {
        Transform bodyLInstance = Instantiate(BodyLPrefab).transform;
        bodyLInstance.SetParent(transform);
        bodys.Add(bodyLInstance);
        bodyLInstance.localPosition = Head.localPosition;
        bodyTotal = 0;
        return bodyLInstance;
    }
}

