using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Box : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    public Sprite solidSprite;
    public Sprite notSolidSprite;

    public Color solidColor;
    public Color notSolidColor;

    public bool startingState;
    private bool _isSolid;
    public bool isSolid
    {
        get { return _isSolid; }
        set
        {
            _isSolid = value; 
            HandleSprite();
        }
    }

    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        isSolid = startingState;
    }

    public void Reset()
    {
        isSolid = startingState;
    }

    [ContextMenu("Toggle State")]
    public void ToggleIsSolid()
    {
        isSolid = !isSolid;
    }


    private void HandleSprite()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.sprite = isSolid ? solidSprite : notSolidSprite;
        _spriteRenderer.color = isSolid ? solidColor : notSolidColor;
    }

    void OnValidate()
    {
        isSolid = startingState;
        HandleSprite();
    }
}
