using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineHandler : MonoBehaviour
{
    private LineRenderer _line;

    public Transform a;
    public Transform b;

    void Awake()
    {
        _line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = (b.position - a.position).normalized;
        Vector3[] positions = {a.position + dir * 0.45f, b.position - dir * 0.45f};
        _line.SetPositions(positions);    
    }
}
