using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveData
{
    public byte[,,] chunkData;
    public Vector3 midPosition;
    public Vector2 chunkPosition;
    public GameObject chunkObject;

    public CaveData(int chunkX, int chunkZ, byte[,,] data, GameObject go)
    {
        midPosition = new Vector3(chunkX * 16 + 8.0f, 0, chunkZ * 16 + 8.0f);
        chunkPosition = new Vector2(chunkX, chunkZ);
        chunkObject = go;
        chunkData = data;
    }
}
