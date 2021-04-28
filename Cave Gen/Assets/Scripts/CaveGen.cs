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

    public Transform worm;

    [Header("Worm Values")]
    public int seed = 0;
    public float lateralSpeed = (2.0f / 8192.0f);
    public int segmentCount = 112;
    public float segmentLength = (1.0f / 64.0f);
    public float speed = (3.0f / 2048.0f);
    public float thickness = (4.0f / 256.0f);
    public float twistness = (4.0f / 256.0f);

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

    // Update is called once per frame
    void Update()
    {
        valueA = (float)module.GetValue(headNoisePos.x, headNoisePos.y, headNoisePos.z);
        valueB = (float)module2.GetValue(headNoisePos.x, headNoisePos.y, headNoisePos.z);

        // Direction the worm moves in (opposite direction)
        headScreenPos.x -= (Mathf.Cos(valueA * 2.0f * Mathf.PI) * Mathf.Sin(valueB * 2.0f * Mathf.PI) * speed);
        headScreenPos.y -= (Mathf.Sin(valueA * 2.0f * Mathf.PI) * Mathf.Sin(valueB * 2.0f * Mathf.PI) * speed);
        headScreenPos.z -= (Mathf.Cos(valueB * 2.0f * Mathf.PI) * speed);

        headNoisePos.x -= speed * 2;
        headNoisePos.y += lateralSpeed;
        headNoisePos.z += lateralSpeed;

        if (!Input.GetKey(KeyCode.Mouse0))
            Draw();

    }

    void Draw()
    {
        foreach(GameObject wormSegment in wormSegments)
        {
            Destroy(wormSegment);
        }
        wormSegments.Clear();

        Vector3 curSegmentScreenPos = headScreenPos;
        Vector3 offsetPos;

        Vector3 curNoisePos;


        for (int curSegment = 0; curSegment < segmentCount; curSegment++)
        {
            curNoisePos.x = headNoisePos.x + (curSegment * twistness);
            curNoisePos.y = headNoisePos.y;
            curNoisePos.z = headNoisePos.z;
            float noiseValueA = (float)module.GetValue(curNoisePos.x, curNoisePos.y, curNoisePos.z);
            float noiseValueB = (float)module.GetValue(curNoisePos.x, curNoisePos.y, curNoisePos.z);

            offsetPos.x = Mathf.Cos(noiseValueA * 2.0f * Mathf.PI) * Mathf.Sin(noiseValueB * 2.0f * Mathf.PI);
            offsetPos.y = Mathf.Sin(noiseValueA * 2.0f * Mathf.PI) * Mathf.Sin(noiseValueB * 2.0f * Mathf.PI);
            offsetPos.z = Mathf.Cos(noiseValueB * 2.0f * Mathf.PI);

            Vector3 currentPos = curSegmentScreenPos + offsetPos;

            GameObject wormSegment = Instantiate(worm.gameObject, currentPos, worm.rotation);
            wormSegments.Add(wormSegment);


            ++curSegment;
            curSegmentScreenPos += offsetPos;
        }
    }
}
