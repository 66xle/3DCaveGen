using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;

public class CaveGen : MonoBehaviour
{
    // Default worm lateral speed.
    const double WORM_LATERAL_SPEED = (2.0 / 8192.0);

    // Default length of a worm segment, in screen units.
    const double WORM_SEGMENT_LENGTH = (1.0 / 64.0);

    // Default segment count for each worm.
    const int WORM_SEGMENT_COUNT = 112;

    // Default worm speed.
    const double WORM_SPEED = (3.0 / 2048.0);

    // Default worm thickness.
    const double WORM_THICKNESS = (4.0 / 256.0);

    // Default "twistiness" of the worms.
    const double WORM_TWISTINESS = (4.0 / 256.0);

    public int mapSizeX;
    public int mapSizeY;
    public int mapSizeZ;

    public float x;
    public float y;
    public float z;

    public Transform worm;

    [Header("Worm Values")]
    public int seed = 0;


    ModuleBase module;
    private float value;

    private Vector3 headNoisePos;
    Vector2 headScreenPos;
    float lateralSpeed;
    int segmentCount;
    float segmentLength;
    float speed;
    float thickness;
    float twistness;

    // Start is called before the first frame update
    void Start()
    {
        module = new Perlin(1.0, 2.375, 0.5, 3, seed, QualityMode.Low);

        headNoisePos = new Vector3((7.0f / 2048.0f), (1163.0f / 2048.0f), (409.0f / 2048.0f));
        speed = (float)WORM_SPEED;
        lateralSpeed = (float)WORM_LATERAL_SPEED;
    }

    // Update is called once per frame
    void Update()
    {
        value = (float)module.GetValue(headNoisePos.x, headNoisePos.y, headNoisePos.z);

        headScreenPos.x -= (Mathf.Cos(value * 2.0f * Mathf.PI) * speed);
        headScreenPos.y -= (Mathf.Sin(value * 2.0f * Mathf.PI) * speed);

        headNoisePos.x -= speed * 2;
        headNoisePos.y += lateralSpeed;
        headNoisePos.z += lateralSpeed;

        worm.position = headNoisePos;

        if (Input.GetKeyDown(KeyCode.Mouse0))
            Draw();
    }

    void Draw()
    {

    }
}
