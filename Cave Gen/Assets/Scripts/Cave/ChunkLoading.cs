using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoading : MonoBehaviour
{
    public CaveGen script;

    public Transform player;

    List<CaveData> notLoadedChunks = new List<CaveData>();

    Vector2 currentChunk;
    Vector2 newChunk;

    void Start()
    {
        script.StartGen();
    }

    void Update()
    {
        // Gets the chunk the player is on
        GetCurrentChunk();

        // If player is near unloaded chunk, generate it
        foreach (CaveData data in notLoadedChunks)
        {
            float distance = Vector2.Distance(new Vector2(player.position.x, player.position.z), data.chunkPosition);

            if (distance < script.chunkDistance)
            {
                script.Generate((int)data.chunkPosition.x, (int)data.chunkPosition.y);
            }
        }

        // If player is not near loaded chunk, delete it
        foreach (GameObject chunk in script.chunkList)
        {
            CaveData data = chunk.GetComponent<CaveData>();

            float distance = Vector2.Distance(new Vector2(player.position.x, player.position.z), data.chunkPosition);

            if (distance > script.chunkDistance)
            {
                Destroy(chunk);
            }
        }
    }

    void GetCurrentChunk()
    {
        float minDistance = 100;

        foreach (GameObject chunk in script.chunkList)
        {
            CaveData data = chunk.GetComponent<CaveData>();

            float distance = Vector2.Distance(new Vector2(player.position.x, player.position.z), new Vector2(data.midPosition.x, data.midPosition.z));

            // Find nearest chunk midpoint
            if (distance < minDistance)
            {
                minDistance = distance;

                newChunk = data.chunkPosition;
            }
        }

        GetOutsideChunks();
    }

    void GetOutsideChunks()
    {
        // Check if player has moved from to a different chunk
        if (newChunk != currentChunk)
        {
            currentChunk = newChunk;
            notLoadedChunks.Clear();

            int chunkDistance = script.chunkDistance + 1;

            for (int x = -chunkDistance + (int)currentChunk.x; x <= chunkDistance + (int)currentChunk.x; x++)
            {
                for (int z = -chunkDistance + (int)currentChunk.y; z <= chunkDistance + (int)currentChunk.y; z++)
                {
                    if (x == -chunkDistance + (int)currentChunk.x || x == chunkDistance + (int)currentChunk.x || z == -chunkDistance + (int)currentChunk.y || z == chunkDistance + (int)currentChunk.y)
                    {
                        CaveData data;
                        data.VarSetup(x, z);

                        // Add chunks not loaded around gen chunks
                        notLoadedChunks.Add(data);
                    }
                }
            }
        }
    }
}
