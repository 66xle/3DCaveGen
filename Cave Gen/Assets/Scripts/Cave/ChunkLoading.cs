using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoading : MonoBehaviour
{
    public CaveGen script;
    public Transform player;

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
              createChunk.Clear();

            currentChunk = newChunk; // Set player current chunk

            int chunkDistance = script.chunkDistance + 1;

            // Loop through chunks in radius of player
            for (int x = -chunkDistance + (int)currentChunk.x; x <= chunkDistance + (int)currentChunk.x; x++)
            {
                for (int z = -chunkDistance + (int)currentChunk.y; z <= chunkDistance + (int)currentChunk.y; z++)
                {
                    int distance = (int)Vector2.Distance(currentChunk, new Vector2(x, z));
                    
                    if (distance <= script.chunkDistance)
                    {
                        // Create Chunks

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
                    else
                    {
                        // Remove chunks

                        CaveData data = new CaveData(x, z);

                        // Loop through generated chunks
                        foreach (CaveData d in script.chunkList)
                        {
                            // If chunk is outside player chunk radius, delete the chunk
                            if (d.chunkPosition == data.chunkPosition)
                            {
                                Destroy(d.chunkObject);
                                script.chunkList.Remove(d);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
