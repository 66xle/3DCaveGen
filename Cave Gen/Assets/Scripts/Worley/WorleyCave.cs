using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorleyCave : MonoBehaviour
{
    const int HAS_CAVES_FLAG = 129;

    public int seed = 0;
    //public float frequency = 0.05f;

    public int minCaveHeight = 1;
    public int maxCaveHeight = 128;

    [Range(-1.0f, 1.0f)] public float noiseCutoff = -0.18f;     //Controls size of caves. Smaller values = larger caves
    [Range(-1.0f, 1.0f)] public float surfaceCutoff = -0.081f;  //Controls size of caves at the surface. Smaller values = more caves break through the surface

    public float warpAmplifier = 8.0f;              //Controls how much to warp caves. Lower values = straighter caves
    public float easeInDepth = 15.0f;               //Reduces number of caves at surface level, becoming more common until caves generate normally X number of blocks below the surface
    public float yCompression = 2.0f;               //Squishes caves on the Y axis. Lower values = taller caves and more steep drops
    public float xzCompression = 1.0f;              //Controls how much to warp caves. Lower values = straighter caves


    private WorleyUtil util;
    private FastNoise noisePerlin;

    private CaveGen script;

    public void Setup()
    {
        script = GetComponent<CaveGen>();

        util = new WorleyUtil(seed);
        util.SetFrequency(0.016f);

        noisePerlin = new FastNoise(seed);
        noisePerlin.SetFrequency(0.05f);
    }

    public void CarveWorleyCaves(int chunkX, int chunkZ)
    {
        int chunkMaxHeight = script.mapSizeY;
        float[,,] samples = SampleNoise(chunkX, chunkZ, chunkMaxHeight + 1);
        float oneQuarter = 0.25f;
        float oneHalf = 0.5f;
        byte block = 2; // 2 = null
        Vector3 localPos;

        int offsetX = chunkX * 16;
        int offsetZ = chunkZ * 16;

        // each chunk divided into 4 subchunks along X axis
        for (int x = 0; x < 4; x++)
        {
            // each chunk divided into 4 subchunks along Z axis
            for (int z = 0; z < 4; z++)
            {
                int depth = 0;

                //don't bother checking all the other logic if there's nothing to dig in this column
                if (samples[x, HAS_CAVES_FLAG, z] == 0 && samples[x + 1, HAS_CAVES_FLAG, z] == 0 && samples[x, HAS_CAVES_FLAG, z + 1] == 0 && samples[x + 1, HAS_CAVES_FLAG, z + 1] == 0)
                    continue;

                // each chunk divided into 128 subchunks along Y axis. Need lots of y sample points to not break things
                for (int y = (maxCaveHeight / 2) - 1; y >= 0; y--)
                {
                    // grab the 8 sample points needed from the noise values
                    float x0y0z0 = samples[x,y,z];
                    float x0y0z1 = samples[x,y,z + 1];
                    float x1y0z0 = samples[x + 1,y,z];
                    float x1y0z1 = samples[x + 1,y,z + 1];
                    float x0y1z0 = samples[x,y + 1,z];
                    float x0y1z1 = samples[x,y + 1,z + 1];
                    float x1y1z0 = samples[x + 1,y + 1,z];
                    float x1y1z1 = samples[x + 1,y + 1,z + 1];

                    // how much to increment noise along y value linear interpolation from start y and end y
                    float noiseStepY00 = (x0y1z0 - x0y0z0) * -oneHalf;
                    float noiseStepY01 = (x0y1z1 - x0y0z1) * -oneHalf;
                    float noiseStepY10 = (x1y1z0 - x1y0z0) * -oneHalf;
                    float noiseStepY11 = (x1y1z1 - x1y0z1) * -oneHalf;

                    // noise values of 4 corners at y=0
                    float noiseStartX0 = x0y0z0;
                    float noiseStartX1 = x0y0z1;
                    float noiseEndX0 = x1y0z0;
                    float noiseEndX1 = x1y0z1;

                    // loop through 2 blocks of the Y subchunk
                    for (int suby = 1; suby >= 0; suby--)
                    {
                        int localY = suby + y * 2;
                        float noiseStartZ = noiseStartX0;
                        float noiseEndZ = noiseStartX1;

                        // how much to increment X values, linear interpolation
                        float noiseStepX0 = (noiseEndX0 - noiseStartX0) * oneQuarter;
                        float noiseStepX1 = (noiseEndX1 - noiseStartX1) * oneQuarter;

                        for (int subx = 0; subx < 4; subx++)
                        {
                            int localX = subx + x * 4;

                            // how much to increment Z values, linear interpolation
                            float noiseStepZ = (noiseEndZ - noiseStartZ) * oneQuarter;

                            // Y and X already interpolated, just need to interpolate final 4 Z block to get final noise value
                            float noiseVal = noiseStartZ;

                            for (int subz = 0; subz < 4; subz++)
                            {
                                int localZ = subz + z * 4;
                                block = 2; // Null
                                localPos = new Vector3(localX + offsetX, localY, localZ + offsetZ);

                                if (depth == 0)
                                {
                                    if (subx == 0 && subz == 0)
                                    {
                                        block = script.data[(int)localPos.x, (int)localPos.y, (int)localPos.z];

                                        // If block is solid???
                                        if (block == 1)
                                            depth++;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else if (subx == 0 && subz == 0)
                                {
                                    // already hit surface, simply increment depth counter
                                    depth++;
                                }

                                float adjustedNoiseCutoff = noiseCutoff;
                                if (depth < easeInDepth)
                                {
                                    // higher threshold at surface, normal threshold below easeInDepth
                                    adjustedNoiseCutoff = Mathf.Lerp(noiseCutoff, surfaceCutoff, (easeInDepth - (float)depth / easeInDepth));
                                }

                                // increase cutoff as we get closer to the minCaveHeight so it's not all flat floors
                                if (localY < (minCaveHeight + 5))
                                {
                                    adjustedNoiseCutoff += ((minCaveHeight + 5) - localY) * 0.05f;
                                }

                                if (noiseVal > adjustedNoiseCutoff)
                                {
                                    if (block == 2)
                                        block = script.data[(int)localPos.x, (int)localPos.y, (int)localPos.z];

                                    DigBlock(localPos, block);
                                }

                                noiseVal += noiseStepZ;
                            }

                            noiseStartZ += noiseStepX0;
                            noiseEndZ += noiseStepX1;
                        }

                        noiseStartX0 += noiseStepY00;
                        noiseStartX1 += noiseStepY01;
                        noiseEndX0 += noiseStepY10;
                        noiseEndX1 += noiseStepY11;
                    }
                }
            }
        }
    }

    void DigBlock(Vector3 blockPos, byte block)
    {
        if (block == 1)
            script.data[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = 0;
    }

    float[,,] SampleNoise(int chunkX, int chunkZ, int maxSurfaceHeight)
    {
        int originalMaxHeight = 128;
        float[,,] noiseSamples = new float[5, 130, 5];
        float noise;

        for (int x = 0; x < 5; x++)
        {
            int realX = x * 4 + chunkX * 16;
            for (int z = 0; z < 5; z++)
            {
                int realZ = z * 4 + chunkZ * 16;
                bool columnHasCaveFlag = false;

                // loop from top down for y values so we can adjust noise above current y later on
                for (int y = 128; y >= 0; y--)
                {
                    float realY = y * 2;
                    if (realY > maxSurfaceHeight || realY > maxCaveHeight || realY < minCaveHeight)
                    {
                        // if outside of valid cave range set noise value below normal minimum of -1.0
                        noiseSamples[x, y, z] = -1.1f;
                    }
                    else
                    {
                        // Experiment making the cave system more chaotic the more you descend
                        float dispAmp = warpAmplifier * ((originalMaxHeight - y) / (originalMaxHeight * 0.85f));

                        float xDisp = noisePerlin.GetNoise(realX, realZ) * dispAmp;
                        float yDisp = noisePerlin.GetNoise(realX, realZ + 67.0f) * dispAmp;
                        float zDisp = noisePerlin.GetNoise(realX, realZ + 149.0f) * dispAmp;

                        // doubling the y frequency to get some more caves
                        noise = util.SingleCellular3Edge(realX * xzCompression + xDisp, realY * yCompression + yDisp, realZ * xzCompression + zDisp);
                        noiseSamples[x, y, z] = noise;

                        if (noise > noiseCutoff)
                        {
                            columnHasCaveFlag = true;
                            // if noise is below cutoff, adjust values of neighbors helps prevent caves fracturing during interpolation
                            if (x > 0)
                                noiseSamples[x - 1, y, z] = (noise * 0.2f) + (noiseSamples[x - 1, y, z] * 0.8f);
                            if (z > 0)
                                noiseSamples[x, y, z - 1] = (noise * 0.2f) + (noiseSamples[x, y, z - 1] * 0.8f);

                            // more heavily adjust y above 'air block' noise values to give players more head room
                            if (y < 128)
                            {
                                float noiseAbove = noiseSamples[x, y + 1, z];
                                if (noise > noiseAbove)
                                    noiseSamples[x, y + 1, z] = (noise * 0.8F) + (noiseAbove * 0.2F);

                                if (y < 127)
                                {
                                    float noiseTwoAbove = noiseSamples[x, y + 2, z];
                                    if (noise > noiseTwoAbove)
                                        noiseSamples[x, y + 2, z] = (noise * 0.35F) + (noiseTwoAbove * 0.65F);
                                }
                            }
                        }
                    }
                }
                noiseSamples[x, HAS_CAVES_FLAG, z] = columnHasCaveFlag ? 1 : 0;
            }
            
        }
        return noiseSamples;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
