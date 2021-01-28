using System;
using System.Collections;
using UnityEngine;

public class ArcadeButton : MonoBehaviour
{
    [SerializeField] private Sprite idleSprite, litSprite;

    private SpriteRenderer renderer;
    private void Start()
    {
        Claw.Instance.OnLower += OnLower;
        Claw.Instance.OnRaise += OnRaise;
        renderer = GetComponent<SpriteRenderer>();
    }

    private void OnRaise()
    {
        renderer.sprite = idleSprite;
    }

    private void OnLower()
    {
        renderer.sprite = litSprite;
    }
}