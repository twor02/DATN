using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Trap_Saw : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sr;
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float cooldown = 1;
    [SerializeField] private Transform[] wayPoint;
    private Vector3[] wayPointPosition;

    public int wayPointIndex = 1;
    public int moveDirection = 1;
    private bool canMove = true;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateWaypointInfo();
        transform.position = wayPointPosition[0];
    }

    private void UpdateWaypointInfo()
    {
        List<Trap_SawWayPoint> wayPointList = new List<Trap_SawWayPoint>(GetComponentsInChildren<Trap_SawWayPoint>());
        if(wayPointList.Count != wayPoint.Length)
        {
            wayPoint = new Transform[wayPointList.Count];
            for(int i = 0; i < wayPointList.Count; i++)
            {
                wayPoint[i] = wayPointList[i].transform;
            }
        }

        wayPointPosition = new Vector3[wayPoint.Length];

        for (int i = 0; i < wayPoint.Length; i++)
        {
            wayPointPosition[i] = wayPoint[i].position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("active", canMove);

        if(canMove == false)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, wayPointPosition[wayPointIndex], moveSpeed * Time.deltaTime);
        if(Vector2.Distance(transform.position, wayPointPosition[wayPointIndex]) < 0.1f)
        {
            if (wayPointIndex == wayPointPosition.Length - 1 || wayPointIndex == 0)
            {
                moveDirection = moveDirection * -1;
                StartCoroutine(StopMovement(cooldown));
            }
            wayPointIndex = wayPointIndex + moveDirection;
        }
    }
    private IEnumerator StopMovement(float delay)
    {
        canMove = false;
        yield return new WaitForSeconds(delay);
        canMove = true;
        sr.flipX = !sr.flipX;
    }
}
