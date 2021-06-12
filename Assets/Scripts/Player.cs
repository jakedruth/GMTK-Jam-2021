using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class Player : MonoBehaviour
{
    public readonly float halfSize = 0.5f;
    public float moveSpeed;
    public float tiltDelta;
    public float tiltSpeed;
    public float tiltAcc;
    private float _tiltMultiplier;

    public LayerMask collisionLayer;
    public UnityEvent onStartMoving;
    public UnityEvent onEndMoving;

    public bool isMoving { get; set; } = false;
    public bool canMove { get; set; } = true;

    void Awake()
    {

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

        Vector3 direction = GetVectorFromInt(dir);

        Ray2D ray = new Ray2D(transform.position, direction);
        Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 0.1f, false);

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 20, collisionLayer);
        if (hit)
        {
            if (hit.transform.tag == "Wall" ||
                hit.transform.tag == "Player")
            {
                Vector3Int point = Vector3Int.RoundToInt(hit.point + hit.normal * halfSize);
                StartCoroutine(MoveToPoint(point));
            }
        }
        else
        {
            Debug.LogWarning($"Did not hit with raycast: {ray}");
        }
    }

    private IEnumerator MoveToPoint(Vector3 point)
    {
        isMoving = true;
        onStartMoving.Invoke();
        
        while (true)
        {
            if (transform.position == point)
                break;

            transform.position = Vector3.MoveTowards(transform.position, point, moveSpeed * Time.deltaTime);

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
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal"))
        {
            collision.GetComponent<GoalMarker>().SetIsOverlapping(false);
        }
    }
}
