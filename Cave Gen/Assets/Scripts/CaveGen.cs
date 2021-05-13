using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;
using System.Collections.Generic;
using UnityEngine;

public class CaveGen : MonoBehaviour
{
    class MeshData
    {
        public List<Vector3> newVertices;
        public List<int> newTriangles;
        public List<Vector2> newUV;

        public MeshData()
        {
            newVertices = new List<Vector3>();
            newTriangles = new List<int>();
            newUV = new List<Vector2>();
        }
    }

    [Header("Cave Values")]
    public byte[,,] data;
    public int maxHeight = 10;
    public int chunkSizeX;
    public int chunkSizeZ;
    public Material material;

    private int mapSizeX;
    private int mapSizeY;
    private int mapSizeZ;

    [Header("Perlin Values")]
    public double frequency = 1.0;
    public double lacunarity = 2.375;
    public double persistence = 0.5;
    public int octaves = 3;
    public int seed = 0;

    [Header("Voronoi Values")]
    public float displacement = 1;
    public bool distance = false;

    [Header("Noise Values")]
    public float noiseDivider = 15.0f;
    public float maxThreshold = 0.5f;
    public float minThreshold = -0.5f;

    ModuleBase perlin;
    ModuleBase rigged;
    ModuleBase billow;
    ModuleBase voronoi;
    ModuleBase cylinders;
    ModuleBase spheres;
    ModuleBase add;

    // Mesh Stuff
    private List<MeshData> meshData = new List<MeshData>();
    private List<GameObject> meshObject = new List<GameObject>();

    private int index = 0;
    private int vertexCount = 0;

    private float tUnit = 0.25f;
    private Vector2 tStone = new Vector2(1, 0);

    private int faceCount;

    // Bool
    [SerializeField] public bool swapData = false;
    private bool updateMesh = false;

    private float startTime;
    private WorleyCave worley;

    void Start()
    {
        worley = GetComponent<WorleyCave>();
        worley.Setup();

        mapSizeX = chunkSizeX * 16;
        mapSizeZ = chunkSizeZ * 16;
        mapSizeY = maxHeight;

        data = new byte[mapSizeX, mapSizeY, mapSizeZ];

        Generate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            SwapData();

        if (Input.GetKeyDown(KeyCode.G))
        {
            updateMesh = false;

            mapSizeX = chunkSizeX * 16;
            mapSizeZ = chunkSizeZ * 16;
            mapSizeY = maxHeight;

            data = new byte[mapSizeX, mapSizeY, mapSizeZ];

            DestoryMesh();
            Generate();
        }
    }

    void DestoryMesh()
    {
        foreach (GameObject go in meshObject)
        {
            Destroy(go);
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

    
    #region CubeVertices

    void CubeTop(int x, int y, int z, byte block)
    {
        CheckVertex();

        meshData[index].newVertices.Add(new Vector3(x, y, z + 1));
        meshData[index].newVertices.Add(new Vector3(x + 1, y, z + 1));
        meshData[index].newVertices.Add(new Vector3(x + 1, y, z));
        meshData[index].newVertices.Add(new Vector3(x, y, z));

        Vector2 texturePos = new Vector2();

        texturePos = tStone;

        Cube(texturePos);
    }

    void CubeNorth(int x, int y, int z, byte block)
    {
        CheckVertex();

        meshData[index].newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        meshData[index].newVertices.Add(new Vector3(x + 1, y    , z + 1));
        meshData[index].newVertices.Add(new Vector3(x    , y    , z + 1));
        meshData[index].newVertices.Add(new Vector3(x    , y - 1, z + 1));

        Vector2 texturePos = new Vector2();

        texturePos = tStone;

        Cube(texturePos);
    }
    void CubeEast(int x, int y, int z, byte block)
    {
        CheckVertex();

        meshData[index].newVertices.Add(new Vector3(x + 1, y - 1, z));
        meshData[index].newVertices.Add(new Vector3(x + 1, y, z));
        meshData[index].newVertices.Add(new Vector3(x + 1, y, z + 1));
        meshData[index].newVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        Vector2 texturePos;

        texturePos = tStone;

        Cube(texturePos);
    }

    void CubeSouth(int x, int y, int z, byte block)
    {
        CheckVertex();

        meshData[index].newVertices.Add(new Vector3(x, y - 1, z));
        meshData[index].newVertices.Add(new Vector3(x, y, z));
        meshData[index].newVertices.Add(new Vector3(x + 1, y, z));
        meshData[index].newVertices.Add(new Vector3(x + 1, y - 1, z));

        Vector2 texturePos;

        texturePos = tStone;

        Cube(texturePos);
    }

    void CubeWest(int x, int y, int z, byte block)
    {
        CheckVertex();

        meshData[index].newVertices.Add(new Vector3(x, y - 1, z + 1));
        meshData[index].newVertices.Add(new Vector3(x, y, z + 1));
        meshData[index].newVertices.Add(new Vector3(x, y, z));
        meshData[index].newVertices.Add(new Vector3(x, y - 1, z));

        Vector2 texturePos;

        texturePos = tStone;

        Cube(texturePos);
    }

    void CubeBottom(int x, int y, int z, byte block)
    {
        CheckVertex();

        meshData[index].newVertices.Add(new Vector3(x, y - 1, z));
        meshData[index].newVertices.Add(new Vector3(x + 1, y - 1, z));
        meshData[index].newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        meshData[index].newVertices.Add(new Vector3(x, y - 1, z + 1));

        Vector2 texturePos;

        texturePos = tStone;

        Cube(texturePos);
    }

    void Cube(Vector2 texturePos)
    {
        meshData[index].newTriangles.Add(faceCount * 4);     //1
        meshData[index].newTriangles.Add(faceCount * 4 + 1); //2
        meshData[index].newTriangles.Add(faceCount * 4 + 2); //3
        meshData[index].newTriangles.Add(faceCount * 4);     //1
        meshData[index].newTriangles.Add(faceCount * 4 + 2); //3
        meshData[index].newTriangles.Add(faceCount * 4 + 3); //4

        meshData[index].newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
        meshData[index].newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
        meshData[index].newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
        meshData[index].newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y));

        faceCount++;
    }

    void CheckVertex()
    {
        vertexCount += 4;

        if (vertexCount >= 65536)
        {
            meshData.Add(new MeshData());

            vertexCount = 0;
            faceCount = 0;
            index++;
        }
    }

    #endregion

    void SwapData()
    {
        startTime = Time.realtimeSinceStartup;

        if (swapData)
            swapData = false;
        else
            swapData = true;

        updateMesh = false;

        //DestoryMesh();
        //CreateMesh();
    }

    void Generate()
    {
        startTime = Time.realtimeSinceStartup;

        //perlin = new Perlin(frequency, lacunarity, persistence, octaves, seed, QualityMode.High);
        //rigged = new RiggedMultifractal(frequency, lacunarity, octaves, seed, QualityMode.High);
        //voronoi = new Voronoi(frequency, displacement, seed, distance);
        //billow = new Billow(frequency, lacunarity, persistence, octaves, seed, QualityMode.High);
        //cylinders = new Cylinders(frequency);
        //spheres = new Spheres(frequency);

        //add = new Add(rigged, perlin);
        //add = new Blend(spheres, perlin, billow);
        //add = new Multiply(rigged, perlin);


        for (int y = 0; y < mapSizeY; y++)
        {
            for (int z = 0; z < mapSizeZ; z++)
            {
                for (int x = 0; x < mapSizeX; x++)
                {
                    data[x, y, z] = 1;

                    //float value = (float)add.GetValue(x / noiseDivider, y / noiseDivider, z / noiseDivider);
                    //
                    //if (value >= minThreshold && value <= maxThreshold || y == 0)
                    //{
                    //    data[x, y, z] = 1;
                    //}
                }
            }
        }

        
        for (int x = 0; x < chunkSizeX; x++)
        {
            for (int z = 0; z < chunkSizeZ; z++)
            {
                worley.CarveWorleyCaves(x, z);
                CreateMesh(x, z);
            }
        }

        updateMesh = true;

        Debug.Log("Loaded in " + (Time.realtimeSinceStartup - startTime) + " Seconds.");
    }

    // Add Verts, Triangles and UVs to mesh
    public void CreateMesh(int chunkX, int chunkZ)
    {
        meshData.Add(new MeshData());

        byte block;

        if (swapData)
            block = 1;
        else
            block = 0;

        for (int x = chunkX * 16; x < chunkX * 16 + 16; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                for (int z = chunkZ * 16; z < chunkZ * 16 + 16; z++)
                {
                    // If block is solid(1)/air(0)
                    if (Block(x, y, z) != block)
                    {
                        if (Block(x, y + 1, z) == block || y == mapSizeY - 1)
                        {
                            // Block above is air
                            CubeTop(x, y, z, Block(x, y, z));
                        }

                        if (Block(x, y - 1, z) == block || y == 0)
                        {
                            //Block below is air
                            CubeBottom(x, y, z, Block(x, y, z));
                        }

                        if (Block(x + 1, y, z) == block || x == mapSizeX - 1)
                        {
                            //Block east is air
                            CubeEast(x, y, z, Block(x, y, z));

                        }

                        if (Block(x - 1, y, z) == block || x == 0)
                        {
                            //Block west is air
                            CubeWest(x, y, z, Block(x, y, z));

                        }

                        if (Block(x, y, z + 1) == block || z == mapSizeZ - 1)
                        {
                            //Block north is air
                            CubeNorth(x, y, z, Block(x, y, z));

                        }

                        if (Block(x, y, z - 1) == block || z == 0)
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
        for (int i = 0; i < meshData.Count; i++)
        {
            MeshData data = meshData[i];

            Mesh mesh;

            // When generating new cave
            if (!updateMesh)
            {
                GameObject go = new GameObject("Mesh");

                go.transform.SetParent(transform);

                MeshRenderer renderer = go.AddComponent<MeshRenderer>();
                renderer.material = material;

                mesh = go.AddComponent<MeshFilter>().mesh;

                meshObject.Add(go);
            }
            else
            {
                mesh = transform.GetChild(i).GetComponent<MeshFilter>().mesh;
            }

            mesh.Clear();
            mesh.vertices = data.newVertices.ToArray();
            mesh.triangles = data.newTriangles.ToArray();
            mesh.uv = data.newUV.ToArray();
            mesh.Optimize();
            mesh.RecalculateNormals();
        }

        meshData.Clear();

        vertexCount = 0;
        index = 0;
        faceCount = 0;
    }
}
