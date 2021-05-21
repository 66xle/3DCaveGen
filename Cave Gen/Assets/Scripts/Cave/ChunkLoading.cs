using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoading : MonoBehaviour
{
    public CaveGen script;
    public Transform player;

    List<Vector2> createChunk = new List<Vector2>();

    Vector2 currentChunk = new Vector2(-9999, -9999);
    Vector2 newChunk;

    void Start()
    {
        script.StartGen();
    }

    void Update()
    {
        // Gets the chunk the player is on
        newChunk = script.GetCurrentChunkPosition(player.position);
        ScanChunkRadius();

        CreateChunks();
    }

    void CreateChunks()
    {
        foreach (Vector2 chunkPos in createChunk)
        {
            script.GenerateChunk((int)chunkPos.x, (int)chunkPos.y);
            createChunk.Remove(chunkPos);
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
                            createChunk.Add(new Vector2(x, z));
                        }
                    }
                    else
                    {
                        // Remove chunks

                        // Loop through generated chunks
                        foreach (CaveData d in script.chunkList)
                        {
                            // If chunk is outside player chunk radius, delete the chunk
                            if (d.chunkPosition == new Vector2(x, z))
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
