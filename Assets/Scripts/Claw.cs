using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum ClawState
{
    Idle,
    Lowering,
    Raising,
    RaisingWithoutPrize,
    Returning
}

public class Claw : MonoBehaviour
{
    public static Claw Instance;

    [SerializeField] private float speed;
    [SerializeField] private float verticalSpeed;
    [SerializeField] private float moveXLimit;
    [SerializeField] private Transform prizePoint;
    [SerializeField] private Transform outOfScreen, lowerBound;
    [SerializeField] private Sprite openSprite, closedSprite;
    
    private Face trackedFace;
    private Vector3 origPos;
    private ClawState clawState;
    private GameObject currentPrize;
    private SpriteRenderer renderer;

    public Action OnLower, OnRaise;
    public Action<GameObject> OnGetPrize;
    
    void Awake()
    {
        Instance = this;
        origPos = transform.position;
        renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!trackedFace)
            return;

        switch (clawState)
        {
            case ClawState.Idle:
                var right = transform.position.x + trackedFace.GetRollAngle() * speed * Time.deltaTime;
                right = Mathf.Clamp(right, origPos.x - moveXLimit, origPos.x + moveXLimit);
                transform.position = new Vector3(right, transform.position.y, transform.position.z);
                renderer.sprite = closedSprite;
                break;
            case ClawState.Lowering:
                transform.position += Vector3.down * (verticalSpeed * Time.deltaTime);
                renderer.sprite = openSprite;
                if (transform.position.y <= lowerBound.transform.position.y)
                {
                    clawState = ClawState.RaisingWithoutPrize;
                    OnRaise?.Invoke();
                }
                break;
            case ClawState.Raising:
                transform.position += Vector3.up * (verticalSpeed * Time.deltaTime);
                if (transform.position.y >= outOfScreen.position.y)
                {
                    clawState = ClawState.Returning;
                    OnLeaveScreen();
                }
                renderer.sprite = closedSprite;
                break;
            case ClawState.RaisingWithoutPrize:
                transform.position += Vector3.up * (verticalSpeed * Time.deltaTime);
                if (transform.position.y >= origPos.y)
                    clawState = ClawState.Idle;
                break;
            case ClawState.Returning:
                transform.position += Vector3.down * (verticalSpeed * Time.deltaTime);
                if (transform.position.y <= origPos.y)
                    clawState = ClawState.Idle;
                renderer.sprite = openSprite;
                break;
        }
    }

    private void OnTiltTrigger()
    {
        if (clawState == ClawState.Idle)
        {
            clawState = ClawState.Lowering;
            OnLower?.Invoke();
        }
    }

    private void OnLeaveScreen()
    {
       OnGetPrize?.Invoke(currentPrize);
       Destroy(currentPrize);
    }
    
    public void RegisterFace(Face face)
    {
        trackedFace = face;
        face.OnTiltTrigger += OnTiltTrigger;
    }

    public void UnregisterFace(Face face)
    {
        if (trackedFace == face)
            trackedFace = null;
        face.OnTiltTrigger -= OnTiltTrigger;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (clawState == ClawState.Lowering && other.CompareTag("Prize"))
        {
            clawState = ClawState.Raising;
            OnRaise?.Invoke();
            if (other.transform.parent)
                currentPrize = other.transform.parent.gameObject;
            else
                currentPrize = other.gameObject;
            
            currentPrize.transform.SetParent(prizePoint);
            other.attachedRigidbody.simulated = false;
        }
    }
}
