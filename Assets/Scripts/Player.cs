using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

[SelectionBase]
public class Player : MonoBehaviour
{
    public const float HalfSize = 0.5f;
    private Vector3 _startPos;

    public float moveSpeed;
    public float tiltDelta;
    public float tiltSpeed;
    public float tiltAcc;
    private float _tiltMultiplier;

    public LayerMask collisionLayer;
    public UnityEvent onStartMoving;
    public UnityEvent onEndMoving;

    public bool isMoving { get; set; }
    public bool canMove { get; set; }

    void Awake()
    {
        _startPos = transform.position;
    }

    void Update()
    {
        float targetTiltMultiplier = isMoving ? 1 : 0;
        _tiltMultiplier = Mathf.MoveTowards(_tiltMultiplier, targetTiltMultiplier, tiltAcc * Time.deltaTime);

        Vector3 rotation = transform.GetChild(0).eulerAngles;
        rotation.z = (Mathf.PingPong(Time.time * tiltSpeed, 2f) - 1) * _tiltMultiplier * tiltDelta;
        transform.GetChild(0).eulerAngles = rotation;
    }

    public void Move(int dir)
    {
        if (isMoving)
            return;

        if (!canMove)
            return;

        if (dir == 4)
        {
            onEndMoving.Invoke();
            return;
        }

        Vector3 direction = GetVectorFromInt(dir);
        Ray2D ray = new Ray2D(transform.position, direction);

        List<Vector3> points = GetPoints(ray, new List<Vector3>());

        StartCoroutine(MoveToPoint(points));
    }

    private List<Vector3> GetPoints(Ray2D ray, List<Vector3> points)
    {
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 20, collisionLayer);
        if (hit)
        {
            switch (hit.transform.tag)
            {
                case "Player":
                {
                    if (hit.transform == transform)
                    {
                        return GetPoints(new Ray2D(hit.transform.position, ray.direction), points );
                    }

                    Vector3Int point = Vector3Int.RoundToInt(hit.point + hit.normal * HalfSize);
                    points.Add(point);
                    return points;
                }
                case "Wall":
                {
                    Vector3Int point = Vector3Int.RoundToInt(hit.point + hit.normal * HalfSize);
                    points.Add(point);
                    return points;
                }
                case "Web":
                {
                    Vector3Int point = Vector3Int.RoundToInt(hit.transform.position);
                    points.Add(point);
                    return points;
                }
                case "Arrow":
                {
                    Vector3Int point = Vector3Int.RoundToInt(hit.transform.position);
                    points.Add(point);
                    Vector3 direction = hit.transform.right;
                    return GetPoints(new Ray2D((Vector3)point, direction), points );
                }
                default:
                {
                    Debug.Log($"hit.transform.tag, [{hit.transform.tag}], is not registered in the switch");
                    break;
                }
            }
        }
        else
        {
            Debug.LogWarning($"Did not hit with raycast: {ray}");
        }

        return points;
    }

    private IEnumerator MoveToPoint(List<Vector3> points)
    {
        isMoving = true;
        onStartMoving.Invoke();
        
        int i = 0;

        while (i < points.Count)
        {
            if (transform.position == points[i])
            {
                i++;
                continue;
            }
            
            transform.position = Vector3.MoveTowards(transform.position, points[i], moveSpeed * Time.deltaTime);

            yield return null;
        }

        isMoving = false;
        onEndMoving.Invoke();
    }

    Vector3 GetVectorFromInt(int i)
    {
        Vector3 vector;
        switch (i)
        {
            default:
                vector = Vector3.up;
                break;
            case 1:
                vector = Vector3.right;
                break;
            case 2:
                vector = Vector3.down;
                break;
            case 3:
                vector = Vector3.left;
                break;
        }

        return vector;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal"))
        {
            collision.GetComponent<GoalMarker>().SetIsOverlapping(true);
        }
        else if (collision.transform.CompareTag("Spike"))
        {
            Debug.Log("Hit Spike");
            FindObjectOfType<LevelHandler>().FailLevel(collision.ClosestPoint(transform.position));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal"))
        {
            collision.GetComponent<GoalMarker>().SetIsOverlapping(false);
        }
    }

    public void Reset()
    {
        StopAllCoroutines();

        transform.position = _startPos;
        _tiltMultiplier = 0;

        isMoving = false;
        canMove = false;
    }

    public void AllowInput(bool value)
    {
        canMove = value;
    }
}
