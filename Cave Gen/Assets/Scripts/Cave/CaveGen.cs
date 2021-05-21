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

    // Mesh Stuff
    private List<MeshData> meshData = new List<MeshData>();
    public List<CaveData> chunkList = new List<CaveData>();

    private int index = 0;
    private int vertexCount = 0;
    private float tUnit = 0.25f;
    private Vector2 tStone = new Vector2(1, 0);
    private int faceCount;


    [Tooltip("Gen cave inside out")] public bool swapData = false;

    private float startTime;
    private WorleyCave worley;

    void Awake()
    {
        worley = GetComponent<WorleyCave>();
        worley.Setup();
    }

    void Update()
    {
        // Shows middle of every chunk
        //foreach (CaveData data in chunkList)
        //{
        //    Debug.DrawLine(data.midPosition, new Vector3(data.midPosition.x, maxHeight + 20.0f, data.midPosition.z), Color.red);
        //}

        // Generate Cave again
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    DestroyChunk();
        //    StartGen();
        //}
    }

    void DestroyChunk()
    {
        // Destory all chunks
        foreach (CaveData data in chunkList)
        {
            Destroy(data.chunkObject);
            chunkList.Remove(data);
        }
    }

    public byte Block(int x, int y, int z)
    {
        // Outside of data array
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

    /// <summary>
    /// Create Chunks depending on chunk distance
    /// </summary>
    public void StartGen()
    {
        startTime = Time.realtimeSinceStartup;

        for (int chunkX = -chunkDistance; chunkX <= chunkDistance; chunkX++)
        {
            for (int chunkZ = -chunkDistance; chunkZ <= chunkDistance; chunkZ++)
            {
                GenerateChunk(chunkX, chunkZ);
            }
        }

        Debug.Log("Loaded in " + (Time.realtimeSinceStartup - startTime) + " Seconds.");
    }

    public void GenerateChunk(int chunkX, int chunkZ)
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

        // Swap Air/Block to Block/Air
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

    // Add Verts, Triangles and UVs to mesh
    void CreateMesh()
    {
        meshData.Add(new MeshData());

        // Loop through chunk data
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    // If block is solid
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

            // Create new gameobject
            GameObject chunk = new GameObject("Chunk");

            chunk.transform.SetParent(transform);
            chunk.transform.position = new Vector3(chunkX * 16, 0, chunkZ * 16);

            MeshRenderer renderer = chunk.AddComponent<MeshRenderer>();
            renderer.material = material;


            // Set mesh stuff
            mesh = chunk.AddComponent<MeshFilter>().mesh;
            mesh.Clear();
            mesh.vertices = data.newVertices.ToArray();
            mesh.triangles = data.newTriangles.ToArray();
            mesh.uv = data.newUV.ToArray();
            mesh.Optimize();
            mesh.RecalculateNormals();

            // Set colldier
            MeshCollider mc = chunk.AddComponent<MeshCollider>();
            mc.sharedMesh = mesh;

            // Store data into chunk
            CaveData caveData = new CaveData(chunkX, chunkZ, chunkData, chunk);
            chunkList.Add(caveData);
        }

        meshData.Clear();

        vertexCount = 0;
        index = 0;
        faceCount = 0;
    }

    void RecalculateMesh(CaveData d)
    {
        for (int i = 0; i < meshData.Count; i++)
        {
            MeshData data = meshData[i];
            Mesh mesh;

            // Set mesh stuff
            mesh = d.chunkObject.GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            mesh.vertices = data.newVertices.ToArray();
            mesh.triangles = data.newTriangles.ToArray();
            mesh.uv = data.newUV.ToArray();
            mesh.Optimize();
            mesh.RecalculateNormals();

            // Set colldier
            MeshCollider mc = d.chunkObject.GetComponent<MeshCollider>();
            mc.sharedMesh = mesh;
        }

        meshData.Clear();

        vertexCount = 0;
        index = 0;
        faceCount = 0;
    }

    /// <summary>
    /// Get the nearest chunk position
    /// </summary>
    /// <param name="position">World position</param>
    /// <returns></returns>
    public Vector2 GetChunkPosition(Vector3 position)
    {
        float minDistance = 100;
        Vector2 newChunk = new Vector2();

        foreach (CaveData data in chunkList)
        {
            float distance = Vector2.Distance(new Vector2(position.x, position.z), new Vector2(data.midPosition.x, data.midPosition.z));

            // Find nearest chunk midpoint
            if (distance < minDistance)
            {
                minDistance = distance;

                newChunk = data.chunkPosition;
            }
        }

        return newChunk;
    }

    public Vector2 ChunkToWorldPosition(Vector2 chunkPosition)
    {
        return new Vector2(chunkPosition.x * 16 + 8.0f, chunkPosition.y * 16 + 8.0f);
    }

    public Vector3 GetMidPosition(Vector2 chunkPosition)
    {
        return new Vector3(chunkPosition.x * 16 + 8.0f, 0, chunkPosition.y * 16 + 8.0f);
    }

    /// <summary>
    /// Change data in a chunk
    /// </summary>
    /// <param name="dataPosition">The data is in world position</param>
    /// <param name="chunkPosition"></param>
    /// <param name="block">Solid = 1, Air = 0</param>
    public void SetData(Vector3 dataPosition, byte data)
    {
        Vector2 chunkPosition = GetChunkPosition(dataPosition);
        Vector3 newDataPosition = new Vector3(Mathf.RoundToInt(dataPosition.x - 0.5f) - chunkPosition.x * 16, Mathf.RoundToInt(dataPosition.y + 0.5f), Mathf.RoundToInt(dataPosition.z - 0.5f) - chunkPosition.y * 16);

        foreach (CaveData d in chunkList)
        {
            if (d.chunkPosition == chunkPosition)
            {
                d.chunkData[(int)newDataPosition.x, (int)newDataPosition.y, (int)newDataPosition.z] = data;

                chunkData = d.chunkData;
                CreateMesh();
                RecalculateMesh(d);

                break;
            }
        }
    }
}
