using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoading : MonoBehaviour
{
    public CaveGen script;

    public Transform player;

    List<CaveData> notLoadedChunks = new List<CaveData>();

    List<CaveData> deleteData = new List<CaveData>();

    Vector2 currentChunk = new Vector2(-9999, -9999);
    Vector2 newChunk;

    void Start()
    {
        script.StartGen();
    }

    void Update()
    {
        // Gets the chunk the player is on
        newChunk = script.GetChunkPosition(player.position);
        GetOutsideChunks();

        // If player is not near loaded chunk, delete it
        foreach (CaveData data in script.chunkList)
        {
            float distance = Vector2.Distance(currentChunk, data.chunkPosition);
        
            if (distance > script.chunkDistance)
            {
                Destroy(data.chunkObject);
                deleteData.Add(data);
            }
        }

        foreach (CaveData data in deleteData)
        {
            script.chunkList.Remove(data);
        }
    }

    void GetOutsideChunks()
    {
        // Check if player has moved from to a different chunk
        if (newChunk != currentChunk)
        {
            notLoadedChunks.Clear();

            currentChunk = newChunk;

            int chunkDistance = script.chunkDistance + 1;

            for (int x = -chunkDistance + (int)currentChunk.x; x <= chunkDistance + (int)currentChunk.x; x++)
            {
                for (int z = -chunkDistance + (int)currentChunk.y; z <= chunkDistance + (int)currentChunk.y; z++)
                {
                    if (x == -chunkDistance + (int)currentChunk.x || x == chunkDistance + (int)currentChunk.x || z == -chunkDistance + (int)currentChunk.y || z == chunkDistance + (int)currentChunk.y)
                    {
                        CaveData data = new CaveData(x, z);

                        // Add chunks not loaded around gen chunks
                        notLoadedChunks.Add(data);
                    }
                    else
                    {
                        bool chunkExists = false;

                        foreach (CaveData data in script.chunkList)
                        {
                            if (data.chunkPosition == new Vector2(x, z))
                            {
                                chunkExists = true;
                                break;
                            }  
                        }

                        if (!chunkExists)
                            script.Generate(x, z);
                    }
                }
            }
        }
    }
}
