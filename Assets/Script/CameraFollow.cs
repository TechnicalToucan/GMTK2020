﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //public Collider2D PlayerCameraBoundsRef;

    public float FollowSpeed = 2f;

    [HideInInspector]
    public GameObject FollowTarget;

    private float nullTimerOnReentry;
    private float slowDownTimer = 0f;
    private float teleportedSpeedMod;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetTeleportedSpeedOverride()
    {
        teleportedSpeedMod = 3f;
    }

    public void ManuallySetFollowPlayer()
    {
        FollowTarget = Service.Player.gameObject;
        nullTimerOnReentry = 0f;
        slowDownTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (FollowTarget == null)
        {
            return;
        }
        else
        {
            if (nullTimerOnReentry > 0f)
            {
                nullTimerOnReentry -= GameConfig.GetDeltaTime();
                if (nullTimerOnReentry <= 0f)
                {
                    slowDownTimer = 2f;
                    return;
                }
            }
        }

        float speedMod = 1f;

        if (slowDownTimer > 0f)
        {
            slowDownTimer -= GameConfig.GetDeltaTime();

            if (slowDownTimer <= 0f)
            {
                FollowTarget = null;
                return;
            }

            speedMod = slowDownTimer / 2f;
        }

        if (teleportedSpeedMod > 1f)
        {
            teleportedSpeedMod -= GameConfig.GetDeltaTime();
            speedMod = teleportedSpeedMod;
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(FollowTarget.transform.position.x, FollowTarget.transform.position.y, transform.position.z), FollowSpeed * GameConfig.GetDeltaTime() * speedMod);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            nullTimerOnReentry = 1f;
            slowDownTimer = 0f;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            FollowTarget = other.gameObject;
            nullTimerOnReentry = 0f;
            slowDownTimer = 0f;
        }
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();

        if (col == null)
        {
            return;
        }
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        var pos = Camera.main.transform.position + new Vector3(col.offset.x, col.offset.y);
        pos.z = 0;
        Gizmos.DrawCube(pos, new Vector3(col.bounds.size.x, col.bounds.size.y, 1f));
    }
#endif
}
