using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    
    public Sprite onSprite;
    public Sprite offSprite;
    
    public bool startingState;

    public Box[] targetBoxes;

    private bool _isOn;
    public bool isOn
    {
        get { return _isOn; }
        set
        {
            _isOn = value;
            foreach (Box box in targetBoxes)
            {
                box.isSolid = startingState == box.startingState ? value : !value;
            }
            HandleSprite();
        }
    }

    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        isOn = startingState;
    }

    public void Reset()
    {
        isOn = startingState;
    }

    [ContextMenu("Toggle State")]
    public void ToggleIsOn()
    {
        isOn = !isOn;
    }

    private void HandleSprite()
    {
        _spriteRenderer.sprite = isOn ? onSprite : offSprite;
    }
}
