using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public Transform enemy;
    public float speed = 200f;

    /*how close the enemy needs to be into a waypoint before it moves on to the next one*/
    public float nextWayPointDistance = 3f;
    
    //public AIPath aiPath;
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    private Seeker seeker;
    private Rigidbody2D rb;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        
        /*generate a path*/
        InvokeRepeating("UpdatePath", 0f, .5f);
        UpdatePath();
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);    
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if(path == null) return;
        
        /*if reached the end of the path*/
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        reachedEndOfPath = false;
        Vector2 direction = ((Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);
        
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        /*it reached the waypoint*/
        if (distance < nextWayPointDistance)
        {
            currentWaypoint++;
        }
        
        if (force.x >= 0.01f)
        {
            enemy.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if(force.x <= -0.01f)
        {
            enemy.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    /*older alternative*/
    //public AIPath aiPath;
    /*void Update()
    {
        if (aiPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if(aiPath.desiredVelocity.x <= 0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }*/
}
