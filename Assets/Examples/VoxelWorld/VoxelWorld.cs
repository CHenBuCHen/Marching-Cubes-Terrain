using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace MarchingCubes.Examples
{
    public class VoxelWorld : World, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] protected int width;
        [SerializeField, HideInInspector] protected int length;
        [SerializeField, HideInInspector] protected int height;

        [SerializeField, HideInInspector] protected int chunkCountX;
        [SerializeField, HideInInspector] protected int chunkCountY;
        [SerializeField, HideInInspector] protected int chunkCountZ;

        [SerializeField, HideInInspector] protected VoxelChunk[] chunksArray;

        public int Width => width;
        public int Lehgth => length;
        public int Height => height;
        public int ChunkCountX => chunkCountX;
        public int ChunkCountY => chunkCountY;
        public int ChunkCountZ => chunkCountZ;

        protected override void Start() //this function is empty,but necessary. Because in base.Start(); Chunks = new Dictionary
        {                               //Dictionary can not serialize. I use VoxelChunk[] and ISerializationCallbackReceiver
            //base.Start(); 
        }

        public void OnBeforeSerialize()
        {
            // I think this is no necessary;
            // only need OnAfterDeserialize();
        }

        public void OnAfterDeserialize()
        {
            Chunks = new Dictionary<int3, Chunk>();
            for (int z = 0; z < ChunkCountZ; z++)
            {
                for (int y = 0; y < ChunkCountY; y++)
                {
                    for (int x = 0; x < ChunkCountX; x++)
                    {
                        Chunks.Add(new int3(x, y, z), chunksArray[x + y * ChunkCountX + z * ChunkCountX * ChunkCountY]);
                    }
                }
            }
        }



        public void CreateWorld(int width, int length, int height, int chunkSize, float isolevel, Material mat, float[,,] density)
        {
            this.width = width;
            this.length = length;
            this.height = height;
            this.chunkSize = chunkSize;
            this.isolevel = isolevel;

            chunkCountX = Mathf.CeilToInt((float)(width - 1) / ChunkSize);
            chunkCountY = Mathf.CeilToInt((float)(height - 1) / ChunkSize);
            chunkCountZ = Mathf.CeilToInt((float)(length - 1) / ChunkSize);

            chunksArray = new VoxelChunk[ChunkCountX * ChunkCountY * ChunkCountZ];


            for (int x = 0; x < chunkCountX; x++)      
            {
                for (int y = 0; y < chunkCountY; y++)
                {
                    for (int z = 0; z < chunkCountZ; z++)
                    {
                        CreateChunk(new int3(x, y, z), mat, density);
                    }
                }
            }

            OnAfterDeserialize();
        }

        public void CreateChunk(int3 chunkCoordinate, Material mat,float[,,] density)
        {
            GameObject go = new GameObject($"Chunk_{chunkCoordinate.x}_{chunkCoordinate.y}_{chunkCoordinate.z}");
            go.transform.SetParent(this.transform);
            go.transform.localPosition = (chunkCoordinate * ChunkSize).ToVectorInt();
            VoxelChunk chunk = go.AddComponent<VoxelChunk>();

            int3 offset = chunkCoordinate * ChunkSize;
            float[] chunkDensity = new float[(ChunkSize + 1) * (ChunkSize + 1) * (ChunkSize + 1)];
            for (int x = 0; x < ChunkSize + 1; x++)
            {
                for (int y = 0; y < ChunkSize + 1; y++)
                {
                    for (int z = 0; z < ChunkSize + 1; z++)
                    {       //density = float[widht,height,length] for example density = float[18,16,16]
                        try //chunksize = 16 ,so need chunkCountX = 2,chunkCountY = 1,chunkCountZ = 1
                        {   //when ask data density[19,16,16] indexOutOfRange ,
                            chunkDensity[x * (ChunkSize + 1) * (ChunkSize + 1) + y * (ChunkSize + 1) + z] = density[x + offset.x, y + offset.y, z + offset.z];
                        }
                        catch (System.Exception ex)
                        {
                            chunkDensity[x * (ChunkSize + 1) * (ChunkSize + 1) + y * (ChunkSize + 1) + z] = 1;
                        }
                    }
                }
            }


            chunk.serializedDensity = chunkDensity;
            chunk.world = this;
            chunk.Initialize(ChunkSize, Isolevel, chunkCoordinate);
            chunk.GetComponent<MeshRenderer>().material = mat;
            chunksArray[chunkCoordinate.x + chunkCoordinate.y * chunkCountX + chunkCoordinate.z * chunkCountX * chunkCountY] = chunk;
        }



#if UNITY_EDITOR

        public void VariablesCheck()
        {
            if (chunksArray != null)
            {
                for (int i = 0; i < chunksArray.Length; i++)
                {
                    chunksArray[i].VariablesCheck();
                    chunksArray[i].StartDensityCalculation();
                }
            }
        }



        public void Dispose()
        {
            if (chunksArray != null)
            {
                for (int i = 0; i < chunksArray.Length; i++)
                {
                    chunksArray[i].Dispose();
                }
            }
        }

        public void UpdateInEditorMode()
        {
            if (chunksArray != null)
            {
                for (int i = 0; i < chunksArray.Length; i++)
                {
                    chunksArray[i].UpdateInEditorMode();
                }
            }
        }
#endif
    }
}

