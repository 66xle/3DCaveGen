using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;

public class Worm : MonoBehaviour
{
    public GameObject worm;
    public GameObject map;

    private CaveGen script;
    private LayerMask layerMask = (1 << 0);

    [Header("Perlin Values")]
    public double frequency = 1.0;
    public double lacunarity = 2.375;
    public double persistence = 0.5;
    public int octaves = 3;
    public int seed = 0;

    [Header("Worm Values")]
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

    private Vector3 curSegmentScreenPos;

    List<GameObject> wormSegments = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        script = map.GetComponent("CaveGen") as CaveGen;

        worm.transform.localScale = new Vector3(thickness, thickness, thickness);

        // Spawn segments
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject wormSegment = Instantiate(worm);
            wormSegments.Add(wormSegment);
            wormSegment.transform.SetParent(transform);
        }

        module = new Perlin(frequency, lacunarity, persistence, octaves, seed, QualityMode.High);
        module2 = new Perlin(frequency, lacunarity, persistence, octaves, seed + 1, QualityMode.High);

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

        headScreenPos.x = Mathf.Clamp(headScreenPos.x, -1, 1);
        headScreenPos.y = Mathf.Clamp(headScreenPos.y, -1, 1);
        headScreenPos.z = Mathf.Clamp(headScreenPos.z, -1, 1);

        if (!Input.GetKey(KeyCode.Mouse0))
            MoveWorm();

    }

    void MoveWorm()
    {
        Vector3 originOffSet = new Vector3(script.mapSizeX / 2, script.mapSizeY / 2, script.mapSizeZ / 2);

        curSegmentScreenPos = (headScreenPos * 100.0f) + originOffSet;
        Vector3 offsetPos;
        Vector3 curNoisePos;

        curSegmentScreenPos.x = Mathf.Clamp(curSegmentScreenPos.x, 1, script.mapSizeX - 1);
        curSegmentScreenPos.y = Mathf.Clamp(curSegmentScreenPos.y, 1, script.mapSizeY - 1);
        curSegmentScreenPos.z = Mathf.Clamp(curSegmentScreenPos.z, 1, script.mapSizeZ - 1);

        wormSegments[0].transform.position = curSegmentScreenPos; // Set worm head position
        //transform.position = curSegmentScreenPos; // Set worm holder position

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

            Raycast(curSegmentScreenPos, wormSegments[curSegment - 1].transform.position);
        }
    }

    void Raycast(Vector3 position, Vector3 previousPosition)
    {
        RaycastHit hit;

        float distance = Vector3.Distance(position, previousPosition);
        Vector3 direction = (previousPosition - position).normalized;

        //Debug.DrawRay(position, direction, Color.green);

        if (Physics.SphereCast(position, thickness, direction, out hit, distance))
        {
            Debug.DrawLine(position, previousPosition, Color.red);

            Vector3 point = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            point += (new Vector3(hit.normal.x, hit.normal.y, hit.normal.z)) * -0.5f;
            
            script.data[Mathf.RoundToInt(point.x - 0.5f), Mathf.RoundToInt(point.y + 0.5f), Mathf.RoundToInt(point.z - 0.5f)] = 0;
            
            script.GenerateMesh();
        }
        else
        {
            Debug.DrawLine(position, previousPosition, Color.blue);
        }
    }
}
