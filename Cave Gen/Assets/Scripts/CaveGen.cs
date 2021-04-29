using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;

public class CaveGen : MonoBehaviour
{
    public GameObject block;

    public int mapSizeX = 10;
    public int mapSizeY = 10;
    public int mapSizeZ = 10;

    public float blockSize = 1.0f;

    private bool caveBuilt = false;
    float x, y, z;
    Vector3 spawnBlock;

    void Start()
    {
        spawnBlock = new Vector3(-(mapSizeX * blockSize / 2), mapSizeY * blockSize / 2, mapSizeZ * blockSize / 2);
    }

    void Update()
    {
        if (!caveBuilt)
        {
            GameObject wall = Instantiate(block, spawnBlock, block.transform.rotation);
            wall.transform.localScale = new Vector3(blockSize, blockSize, blockSize);
            wall.transform.SetParent(transform);

            spawnBlock.x += blockSize;
            x++;

            if (x >= mapSizeX)
            {
                spawnBlock.x -= blockSize * mapSizeX;
                spawnBlock.z -= blockSize;
                x = 0;
                z++;

                if (z >= mapSizeZ)
                {
                    spawnBlock.z += blockSize * mapSizeZ;
                    spawnBlock.y -= blockSize;
                    z = 0;
                    y++;

                    if (y >= mapSizeY)
                    {
                        caveBuilt = true;
                    }
                }
            }


            //Vector3 spawnBlock = new Vector3(-(mapSizeX * blockSize / 2), mapSizeY * blockSize / 2, mapSizeZ * blockSize / 2);
            //for (int y = 0; y < mapSizeX; y++)
            //{
            //    for (int z = 0; z < mapSizeX; z++)
            //    {
            //        for (int x = 0; x < mapSizeX; x++)
            //        {
            //            GameObject wall = Instantiate(block, spawnBlock, block.transform.rotation);
            //            wall.transform.localScale = new Vector3(blockSize, blockSize, blockSize);
            //            wall.transform.SetParent(transform);
            //
            //            spawnBlock.x += blockSize;
            //        }
            //        spawnBlock.x -= blockSize * mapSizeX;
            //        spawnBlock.z -= blockSize;
            //    }
            //    spawnBlock.z += blockSize * mapSizeZ;
            //    spawnBlock.y -= blockSize;
            //}

            //caveBuilt = true;
        }
    }
}
