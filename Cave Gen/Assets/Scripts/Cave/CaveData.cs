using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveData : MonoBehaviour
{
    public byte[,,] data;
    public Vector3 midPosition;
    public Vector2 chunkPosition;

    public void VarSetup(int chunkX, int chunkZ)
    {
        midPosition = new Vector3(chunkX * 16 + 8.0f, 0, chunkZ * 16 + 8.0f);
        chunkPosition = new Vector2(chunkX, chunkZ);
    }
}
