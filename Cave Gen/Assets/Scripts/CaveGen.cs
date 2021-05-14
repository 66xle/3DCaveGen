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
    public byte[,,] chunkData;
    public int maxHeight = 10;
    public int chunkDistance;
    public Material material;

    //[Header("Perlin Values")]
    //public double frequency = 1.0;
    //public double lacunarity = 2.375;
    //public double persistence = 0.5;
    //public int octaves = 3;
    //public int seed = 0;
    //
    //[Header("Voronoi Values")]
    //public float displacement = 1;
    //public bool distance = false;
    //
    //[Header("Noise Values")]
    //public float noiseDivider = 15.0f;
    //public float maxThreshold = 0.5f;
    //public float minThreshold = -0.5f;

    //ModuleBase perlin;
    //ModuleBase rigged;
    //ModuleBase billow;
    //ModuleBase voronoi;
    //ModuleBase cylinders;
    //ModuleBase spheres;
    //ModuleBase add;

    // Mesh Stuff
    private List<MeshData> meshData = new List<MeshData>();
    private List<GameObject> chunkList = new List<GameObject>();

    private int index = 0;
    private int vertexCount = 0;

    private float tUnit = 0.25f;
    private Vector2 tStone = new Vector2(1, 0);

    private int faceCount;

    // Bool
    [SerializeField] public bool swapData = false;

    private float startTime;
    private WorleyCave worley;

    void Start()
    {
        worley = GetComponent<WorleyCave>();
        worley.Setup();

        Generate();
    }

    void Update()
    {
        foreach (GameObject go in chunkList)
        {
            CaveData data = go.GetComponent<CaveData>();
            Debug.DrawLine(data.midPosition, new Vector3(data.midPosition.x, maxHeight + 20.0f, data.midPosition.z), Color.red);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            DestroyChunk();
            Generate();
        }
    }

    void DestroyChunk()
    {
        foreach (GameObject go in chunkList)
        {
            Destroy(go);
        }
    }

    public byte Block(int x, int y, int z)
    {
        if (x >= 16 || x < 0 || y >= maxHeight || y < 0 || z >= 16 || z < 0)
        {
            return 1;
        }

        return chunkData[x, y, z];
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

        for (int chunkX = -chunkDistance; chunkX <= chunkDistance; chunkX++)
        {
            for (int chunkZ = -chunkDistance; chunkZ <= chunkDistance; chunkZ++)
            {
                chunkData = new byte[16, maxHeight, 16];

                // Fill chunk with solid blocks
                for (int y = 0; y < maxHeight; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        for (int x = 0; x < 16; x++)
                        {
                            chunkData[x, y, z] = 1;
                        }
                    }
                }

                worley.CarveWorleyCaves(chunkX, chunkZ);

                // Debugging
                if (swapData)
                {
                    for (int y = 0; y < maxHeight; y++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            for (int x = 0; x < 16; x++)
                            {
                                if (chunkData[x, y, z] == 1)
                                    chunkData[x, y, z] = 0;
                                else if (chunkData[x, y, z] == 0)
                                    chunkData[x, y, z] = 1;
                            }
                        }
                    }
                }

                CreateMesh();
                UpdateMesh(chunkX, chunkZ);
            }
        }

        Debug.Log("Loaded in " + (Time.realtimeSinceStartup - startTime) + " Seconds.");
    }

    // Add Verts, Triangles and UVs to mesh
    public void CreateMesh()
    {
        meshData.Add(new MeshData());

        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    // If block is solid(1)/air(0)
                    if (Block(x, y, z) != 0)
                    {
                        if (Block(x, y + 1, z) == 0 || y == maxHeight - 1)
                        {
                            // Block above is air
                            CubeTop(x, y, z, Block(x, y, z));
                        }

                        if (Block(x, y - 1, z) == 0 || y == 0)
                        {
                            //Block below is air
                            CubeBottom(x, y, z, Block(x, y, z));
                        }

                        if (Block(x + 1, y, z) == 0 || x == 15)
                        {
                            //Block east is air
                            CubeEast(x, y, z, Block(x, y, z));

                        }

                        if (Block(x - 1, y, z) == 0 || x == 0)
                        {
                            //Block west is air
                            CubeWest(x, y, z, Block(x, y, z));

                        }

                        if (Block(x, y, z + 1) == 0 || z == 15)
                        {
                            //Block north is air
                            CubeNorth(x, y, z, Block(x, y, z));

                        }

                        if (Block(x, y, z - 1) == 0 || z == 0)
                        {
                            //Block south is air
                            CubeSouth(x, y, z, Block(x, y, z));
                        }
                    }
                }
            }
        }
    }

    void UpdateMesh(int chunkX, int chunkZ)
    {
        for (int i = 0; i < meshData.Count; i++)
        {
            MeshData data = meshData[i];

            Mesh mesh;

            GameObject go = new GameObject("Chunk");

            go.transform.SetParent(transform);
            go.transform.position = new Vector3(chunkX * 16, 0, chunkZ * 16);

            // Store data into chunk
            CaveData script = go.AddComponent<CaveData>();
            script.data = chunkData;
            script.midPosition = new Vector3(go.transform.position.x + 8.0f, 0, go.transform.position.z + 8.0f);

            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.material = material;

            mesh = go.AddComponent<MeshFilter>().mesh;
            MeshCollider mc = go.AddComponent<MeshCollider>();

            chunkList.Add(go);


            mesh.Clear();
            mesh.vertices = data.newVertices.ToArray();
            mesh.triangles = data.newTriangles.ToArray();
            mesh.uv = data.newUV.ToArray();
            mesh.Optimize();
            mesh.RecalculateNormals();

            mc.sharedMesh = mesh;
        }

        meshData.Clear();

        vertexCount = 0;
        index = 0;
        faceCount = 0;
    }
}
