﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;

public class CaveGen : MonoBehaviour
{
    public int mapSizeX;
    public int mapSizeY;
    public int mapSizeZ;

    public float x;
    public float y;
    public float z;

    public GameObject worm;

    [Header("Worm Values")]
    public int seed = 0;
    public int segmentCount = 112;
    public float segmentLength = 1.0f;
    public float thickness = 1.0f;

    [Tooltip("Default: 2.0f")] public float lateralSpeed = 2.0f; // Speed of the segments
    [Tooltip("Default: 3.0f")] public float speed = 3.0f;        // Speed of the worm/segments
    [Tooltip("Default: 4.0f")] public float twistness = 4.0f;


    private float LATERALSPEED = 2.0f;
    private float SPEED = 3.0f; 
    private float TWISTNESS = 4.0f;

    ModuleBase module;
    ModuleBase module2;
    private float valueA;
    private float valueB;

    private Vector3 headNoisePos;
    private Vector3 headScreenPos;
    

    List<GameObject> wormSegments = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        worm.transform.localScale = new Vector3(thickness, thickness, thickness);
        
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject wormSegment = Instantiate(worm);
            wormSegments.Add(wormSegment);
        }

        module = new Perlin(1.0, 2.375, 0.5, 3, seed, QualityMode.Low);
        module2 = new Perlin(1.0, 2.375, 0.5, 3, seed + 1, QualityMode.Low);

        headNoisePos = new Vector3((7.0f / 2048.0f), (1163.0f / 2048.0f), (409.0f / 2048.0f));
    }

    void UpdateValues()
    {
        LATERALSPEED = lateralSpeed / 8192.0f;
        SPEED = speed / 2048.0f;
        TWISTNESS = twistness / 256.0f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateValues();

        // Get noise values
        valueA = (float)module.GetValue(headNoisePos.x, headNoisePos.y, headNoisePos.z) * 2.0f * Mathf.PI;
        valueB = (float)module2.GetValue(headNoisePos.x, headNoisePos.y, headNoisePos.z) * 2.0f * Mathf.PI;

        // Direction the worm moves in (opposite direction)
        headScreenPos.x -= (Mathf.Cos(valueA) * Mathf.Sin(valueB) * SPEED);
        headScreenPos.y -= (Mathf.Sin(valueA) * Mathf.Sin(valueB) * SPEED);
        headScreenPos.z -= (Mathf.Cos(valueB) * SPEED);

        // Noise stuff
        headNoisePos.x -= SPEED * 2;
        headNoisePos.y += LATERALSPEED;
        headNoisePos.z += LATERALSPEED;

        if (!Input.GetKey(KeyCode.Mouse0))
            CreateSegments();
    }

    void CreateSegments()
    {
        Vector3 curSegmentScreenPos = headScreenPos * 100.0f;
        Vector3 offsetPos;
        Vector3 curNoisePos;

        wormSegments[0].transform.position = curSegmentScreenPos;

        for (int curSegment = 1; curSegment < segmentCount; curSegment++)
        {
            // Noise Stuff
            curNoisePos.x = headNoisePos.x + (curSegment * 2 * TWISTNESS);
            curNoisePos.y = headNoisePos.y;
            curNoisePos.z = headNoisePos.z;
            float noiseValueA = (float)module.GetValue(curNoisePos.x, curNoisePos.y, curNoisePos.z) * 2.0f * Mathf.PI;
            float noiseValueB = (float)module2.GetValue(curNoisePos.x, curNoisePos.y, curNoisePos.z) * 2.0f * Mathf.PI;

            // Offsetting Segment
            offsetPos.x = Mathf.Cos(noiseValueA) * Mathf.Sin(noiseValueB);
            offsetPos.y = Mathf.Sin(noiseValueA) * Mathf.Sin(noiseValueB);
            offsetPos.z = Mathf.Cos(noiseValueB);

            Vector3 currentPos = curSegmentScreenPos + offsetPos;

            wormSegments[curSegment].transform.position = currentPos;

            curSegmentScreenPos += offsetPos;
        }
    }
}
