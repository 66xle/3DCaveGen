using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoading : MonoBehaviour
{
    public CaveGen script;
    public Transform player;

    List<CaveData> notLoadedChunks = new List<CaveData>();
    List<CaveData> deleteData = new List<CaveData>();
    List<CaveData> createChunk = new List<CaveData>();

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
        ScanChunkRadius();

        CreateChunks();

        foreach (CaveData data in deleteData)
        {
            script.chunkList.Remove(data);
        }
    }

    void CreateChunks()
    {
        foreach (CaveData data in createChunk)
        {
            script.GenerateChunk((int)data.chunkPosition.x, (int)data.chunkPosition.y);
            createChunk.Remove(data);
            return; // Return so we can create a chunk every frame
        }
    }

    void ScanChunkRadius()
    {
        // Check if player has moved from to a different chunk
        if (newChunk != currentChunk)
        {
            notLoadedChunks.Clear();
            createChunk.Clear();

            currentChunk = newChunk; // Set player current chunk

            int chunkDistance = script.chunkDistance + 1;

            // Loop through chunks in radius of player
            for (int x = -chunkDistance + (int)currentChunk.x; x <= chunkDistance + (int)currentChunk.x; x++)
            {
                for (int z = -chunkDistance + (int)currentChunk.y; z <= chunkDistance + (int)currentChunk.y; z++)
                {
                    // If x/z is around the current generated chunks
                    if (x == -chunkDistance + (int)currentChunk.x || x == chunkDistance + (int)currentChunk.x || z == -chunkDistance + (int)currentChunk.y || z == chunkDistance + (int)currentChunk.y)
                    {
                        CaveData data = new CaveData(x, z);

                        // Add chunks not loaded around gen chunks
                        notLoadedChunks.Add(data);

                        foreach (CaveData d in script.chunkList)
                        {
                            // If chunk is outside player chunk radius, delete the chunk
                            if (d.chunkPosition == data.chunkPosition)
                            {
                                Destroy(d.chunkObject);
                                deleteData.Add(d);
                            }
                        }
                    }
                    else
                    {
                        bool chunkExists = false;

                        foreach (CaveData data in script.chunkList)
                        {
                            // If chunk does exist don't create chunk
                            if (data.chunkPosition == new Vector2(x, z))
                            {
                                chunkExists = true;
                                break;
                            }  
                        }

                        // Create chunk if chunk does not exist
                        if (!chunkExists)
                        {
                            createChunk.Add(new CaveData(x, z));
                        }
                    }
                }
            }
        }
    }
}
