using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LunarConsolePlugin;
using MoreMountains.Tools;

public class MotionEventListener : MMSingleton<MotionEventListener>, MMEventListener<MMGameEvent>
{
    public Transform targetToListen;

    public float targetVelocityThreshold = 5f;
    public float lowerVelocityThreshold = 1f;

    bool thresholdCrossed;

    private Vector3 velocity;

    List<Vector3> velocitySnapshots = new List<Vector3>();
    public int limitSnapshots = 5;
    public int sampleSnapshotsForEndMotion = 2;
    Vector3 lastFramePosition;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        velocitySnapshots.Add((targetToListen.position - lastFramePosition) / Time.deltaTime);
        lastFramePosition = targetToListen.position;

        if (velocitySnapshots.Count > limitSnapshots)
        {
            velocitySnapshots.RemoveAt(0);
        }

        velocity = GetMotionVelocity();

        if (!thresholdCrossed && velocity.magnitude > targetVelocityThreshold)
        {
            //Debug.Log("Velocity crossed on summary " + velocity + " with magnitude of " + velocity.magnitude);
            thresholdCrossed = true;
            MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.motionThresholdCrossed);
        }

        velocity = GetLastSnapshots(sampleSnapshotsForEndMotion) / sampleSnapshotsForEndMotion;

        if (thresholdCrossed && velocity.magnitude < lowerVelocityThreshold)
        {
            //Debug.Log("Velocity dropped at " + velocity + " with magnitude of " + velocity.magnitude);
            thresholdCrossed = false;
            MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.motionStopped);
        }
    }

    public Vector3 GetMotionVector()
    {
        return GetLastSnapshots(limitSnapshots).normalized;
    }

    public Vector3 GetMotionVelocity()
    {
        return GetLastSnapshots(limitSnapshots) / limitSnapshots;
    }

    private Vector3 GetLastSnapshots(int count)
    {
        count = Mathf.Clamp(count, 0, velocitySnapshots.Count);
        count = velocitySnapshots.Count - count;
        
        Vector3 summaryVectors = new Vector3();
        for (int i = velocitySnapshots.Count - 1; i >= count; i--)
        {
            summaryVectors += velocitySnapshots[i];
        }

        return summaryVectors;
    }

    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
    }

    public void OnMMEvent(MMGameEvent eventType)
    {

    }
}
