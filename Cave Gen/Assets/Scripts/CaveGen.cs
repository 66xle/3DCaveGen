using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;

public class CaveGen : MonoBehaviour
{
    public byte[,,] data;
    public int mapSizeX = 10;
    public int mapSizeY = 10;
    public int mapSizeZ = 10;

    [Header("Perlin Values")]
    public double frequency = 1.0;
    public double lacunarity = 2.375;
    public double persistence = 0.5;
    public int octaves = 3;
    public int seed = 0;

    [Header("Worm Values")]
    public float lateralSpeed = 2.0f;
    public float speed = 3.0f;

    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();

    private float tUnit = 0.25f;
    private Vector2 tStone = new Vector2(1, 0);
    private Vector2 tGrass = new Vector2(0, 1);

    private Mesh mesh;
    private MeshCollider col;

    private int faceCount;

    private LayerMask layerMask = (0 << 1);

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        col = GetComponent<MeshCollider>();

        mapSizeX++;
        mapSizeY++;
        mapSizeZ++;

        data = new byte[mapSizeX, mapSizeY, mapSizeZ];

        for (int y = 0; y < mapSizeY; y++)
        {
            for (int z = 0; z < mapSizeZ; z++)
            {
                for (int x = 0; x < mapSizeX; x++)
                {
                    if (x > 0 && x < mapSizeX - 1 && y > 0 && y < mapSizeY - 1 && z > 0 && z < mapSizeZ - 1)
                    {
                        data[x, y, z] = 1;
                    }
                }
            }
        }

        GenerateMesh();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            mapSizeX++;
            mapSizeY++;
            mapSizeZ++;

            data = new byte[mapSizeX, mapSizeY, mapSizeZ];

            for (int y = 0; y < mapSizeY; y++)
            {
                for (int z = 0; z < mapSizeZ; z++)
                {
                    for (int x = 0; x < mapSizeX; x++)
                    {
                        if (x > 0 && x < mapSizeX - 1 && y > 0 && y < mapSizeY - 1 && z > 0 && z < mapSizeZ - 1)
                        {
                            data[x, y, z] = 1;
                        }
                    }
                }
            }

            GenerateMesh();
        }
    }

    void Worm()
    {
        Vector3 wormPos = new Vector3(mapSizeX / 2, mapSizeY / 2, mapSizeZ / 2);

        Vector3 noisePos = new Vector3((7.0f / 2048.0f), (1163.0f / 2048.0f), (409.0f / 2048.0f));

        ModuleBase perlin = new Perlin(frequency, lacunarity, persistence, octaves, seed, QualityMode.Low);
        ModuleBase perlin2 = new Perlin(frequency, lacunarity, persistence, octaves, seed + 1, QualityMode.Low);

        float LATERALSPEED = lateralSpeed / 8192.0f;
        float SPEED = speed / 2048.0f;

        for (int i = 0; i < 100; i++)
        {
            float noiseValue = (float)perlin.GetValue(noisePos);
            float noiseValue2 = (float)perlin2.GetValue(noisePos);

            wormPos.x -= (Mathf.Cos(noiseValue) * Mathf.Sin(noiseValue2) * SPEED * 1000.0f);
            wormPos.y -= (Mathf.Sin(noiseValue) * Mathf.Sin(noiseValue2) * SPEED * 1000.0f);
            wormPos.z -= (Mathf.Cos(noiseValue2) * SPEED * 1000.0f);

            Debug.Log(wormPos);

            wormPos.x = Mathf.Clamp(wormPos.x, 0, mapSizeX - 1);
            wormPos.y = Mathf.Clamp(wormPos.y, 0, mapSizeY - 1);
            wormPos.z = Mathf.Clamp(wormPos.z, 0, mapSizeZ - 1);

            noisePos += new Vector3(-(SPEED * 2), LATERALSPEED, LATERALSPEED);


            data[(int)wormPos.x, (int)wormPos.y, (int)wormPos.z] = 0;
        }
    }

    public byte Block(int x, int y, int z)
    {
        if (x >= mapSizeX || x < 0 || y >= mapSizeY || y < 0 || z >= mapSizeZ || z < 0)
        {
            return (byte)1;
        }

        return data[x, y, z];
    }

    
    #region CubeVerticies

    void CubeTop(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x,     y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z    ));
        newVertices.Add(new Vector3(x,     y, z    ));

        Vector2 texturePos = new Vector2();

        texturePos = tStone;

        Cube(texturePos);
    }

    void CubeNorth(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x + 1, y    , z + 1));
        newVertices.Add(new Vector3(x    , y    , z + 1));
        newVertices.Add(new Vector3(x    , y - 1, z + 1));

        Vector2 texturePos = new Vector2();

        texturePos = tStone;

        Cube(texturePos);
    }
    void CubeEast(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x + 1, y - 1, z    ));
        newVertices.Add(new Vector3(x + 1, y    , z    ));
        newVertices.Add(new Vector3(x + 1, y    , z + 1));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        Vector2 texturePos;

        texturePos = tStone;

        Cube(texturePos);
    }

    void CubeSouth(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x    , y - 1, z));
        newVertices.Add(new Vector3(x    , y    , z));
        newVertices.Add(new Vector3(x + 1, y    , z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));

        Vector2 texturePos;

        texturePos = tStone;

        Cube(texturePos);
    }

    void CubeWest(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y    , z + 1));
        newVertices.Add(new Vector3(x, y    , z    ));
        newVertices.Add(new Vector3(x, y - 1, z    ));

        Vector2 texturePos;

        texturePos = tStone;

        Cube(texturePos);
    }

    void CubeBottom(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x    , y - 1, z     ));
        newVertices.Add(new Vector3(x + 1, y - 1, z     ));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x    , y - 1, z + 1));

        Vector2 texturePos;

        texturePos = tStone;

        Cube(texturePos);
    }

    void Cube(Vector2 texturePos)
    {
        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 1); //2
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4 + 3); //4

        newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
        newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
        newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
        newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y));

        faceCount++;
    }

    #endregion

    public void GenerateMesh()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                for (int z = 0; z < mapSizeZ; z++)
                {
                    // If block is solid
                    if (Block(x, y, z) != 0)
                    {
                        
                        if (Block(x, y + 1, z) == 0)
                        {
                            // Block above is air
                            CubeTop(x, y, z, Block(x, y, z));
                        }
                        if (Block(x, y - 1, z) == 0)
                        {
                            //Block below is air
                            CubeBottom(x, y, z, Block(x, y, z));

                        }

                        if (Block(x + 1, y, z) == 0)
                        {
                            //Block east is air
                            CubeEast(x, y, z, Block(x, y, z));

                        }

                        if (Block(x - 1, y, z) == 0)
                        {
                            //Block west is air
                            CubeWest(x, y, z, Block(x, y, z));

                        }

                        if (Block(x, y, z + 1) == 0)
                        {
                            //Block north is air
                            CubeNorth(x, y, z, Block(x, y, z));

                        }

                        if (Block(x, y, z - 1) == 0)
                        {
                            //Block south is air
                            CubeSouth(x, y, z, Block(x, y, z));

                        }
                    }
                }
            }
        }

        UpdateMesh();
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        col.sharedMesh = null;
        col.sharedMesh = mesh;

        newVertices.Clear();
        newUV.Clear();
        newTriangles.Clear();

        faceCount = 0;
    }
}
