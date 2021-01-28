using System;
using UnityEngine;
using UnityEngine.UI;

public class Face : MonoBehaviour
{
    [SerializeField] private float tiltTriggerThreshold;
    public Action OnTiltTrigger;
    private float lastPitchAngle;
    
    private void Start()
    {
        Claw.Instance.RegisterFace(this);
    }

    private void OnDestroy()
    {
        Claw.Instance.UnregisterFace(this);
    }

    private void Update()
    {
        var pitch = GetPitchAngle();
        if ((pitch - lastPitchAngle) > 0 && pitch >= tiltTriggerThreshold && lastPitchAngle < tiltTriggerThreshold)
        {
            OnTiltTrigger?.Invoke();
        }

        lastPitchAngle = pitch;
    }

    public float GetRollAngle()
    {
        var fwd = transform.forward;
        fwd.y = 0;
        fwd *= Mathf.Sign(transform.up.y);
        var right = Vector3.Cross(Vector3.up, fwd).normalized;
        return Vector3.Angle(right, transform.right) * Mathf.Sign(transform.right.y);
    }

    public float GetPitchAngle()
    {
        var right = transform.right;
        right.y = 0;
        right *= Mathf.Sign(transform.up.y);
        var fwd = Vector3.Cross(right, Vector3.up).normalized;
        return Vector3.Angle(fwd, transform.forward) * Mathf.Sign(transform.forward.y);
    }
}