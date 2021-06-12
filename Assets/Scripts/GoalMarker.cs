using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class GoalMarker : MonoBehaviour
{
    private SpriteRenderer _sprite;

    public Color notActive;
    public Color active;

    public bool isOverlapping { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        _sprite = transform.GetComponentInChildren<SpriteRenderer>();
        _sprite.color = notActive;
    }

    public void SetIsOverlapping(bool value)
    {
        isOverlapping = value;
        _sprite.color = isOverlapping ? active : notActive;
    }
}
