﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
    [SerializeField] private MagicWand magicWand;
    [SerializeField] private LineRenderer laserRenderer;
    bool laserActive;
    [SerializeField] float maxLaserLength;

    RaycastHit laserTargetHit;

    Vector3[] positions;

    private void Awake()
    {
        positions = new Vector3[2];
        positions[0] = Vector3.zero;
        positions[1] = Vector3.zero;
    }
    public void SetLaserON()
    {
        laserActive = true;
    }


    public void SetLaserOff()
    {
        laserActive = false;
    }

    public void ChangeLaserColor(Color color)
    {
        laserRenderer.startColor = color;
        laserRenderer.endColor = color;
    }

    public void ProcessLaserPointer()
    {
        if (Physics.Raycast(magicWand.wandEndPosition.position, magicWand.wandEndPosition.forward, out laserTargetHit))
            positions[1] = Vector3.forward * laserTargetHit.distance;
        else
            positions[1] = Vector3.forward * maxLaserLength;

        laserRenderer.SetPositions(positions);
    }

    private void FixedUpdate()
    {
        if (laserActive) ProcessLaserPointer();
    }
}