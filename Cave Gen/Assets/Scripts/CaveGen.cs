using System.Collections;
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

    [Tooltip("Speed of the segments")]      public float lateralSpeed = 2.0f;
    [Tooltip("Speed of the worm/segments")] public float speed = 3.0f;
    public float twistness = 4.0f;


    private float LATERALSPEED = 2.0f;
    private float SPEED = 3.0f; 
    private float TWISTNESS = 4.0f;

    ModuleBase module;
    ModuleBase module2;
    private float valueA;
    private float valueB;

    private Vector3 headNoisePos;
    private Vector3 headScreenPos;
    

    List<GameObject> wormSegments;

    // Start is called before the first frame update
    void Start()
    {
        wormSegments = new List<GameObject>();

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
        // Delete worm/segments
        foreach(GameObject wormSegment in wormSegments)
        {
            Destroy(wormSegment);
        }
        wormSegments.Clear();

        Vector3 curSegmentScreenPos = headScreenPos;
        Vector3 offsetPos;
        Vector3 curNoisePos;

        GameObject wormHead = Instantiate(worm, headScreenPos, worm.transform.rotation);
        wormHead.transform.localScale = new Vector3(thickness, thickness, thickness);
        wormSegments.Add(wormHead);

        for (int curSegment = 0; curSegment < segmentCount; curSegment++)
        {
            // Noise Stuff
            curNoisePos.x = headNoisePos.x + (curSegment * TWISTNESS);
            curNoisePos.y = headNoisePos.y;
            curNoisePos.z = headNoisePos.z;
            float noiseValueA = (float)module.GetValue(curNoisePos.x, curNoisePos.y, curNoisePos.z);
            float noiseValueB = (float)module2.GetValue(curNoisePos.x, curNoisePos.y, curNoisePos.z);

            // Offsetting Segment
            offsetPos.x = Mathf.Cos(noiseValueA * 2.0f * Mathf.PI) * Mathf.Sin(noiseValueB * 2.0f * Mathf.PI);
            offsetPos.y = Mathf.Sin(noiseValueA * 2.0f * Mathf.PI) * Mathf.Sin(noiseValueB * 2.0f * Mathf.PI);
            offsetPos.z = Mathf.Cos(noiseValueB * 2.0f * Mathf.PI);

            Vector3 currentPos = curSegmentScreenPos + offsetPos;

            // Create Segment
            GameObject wormSegment = Instantiate(worm, currentPos, worm.transform.rotation);
            wormSegment.transform.localScale = new Vector3(thickness, thickness, thickness);
            wormSegments.Add(wormSegment);

            ++curSegment;
            curSegmentScreenPos += offsetPos;
        }
    }
}
