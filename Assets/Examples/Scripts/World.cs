using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace MarchingCubes.Examples
{
    public abstract class World : MonoBehaviour
    {
        /// <summary>
        /// The chunk's size. This represents the width, height and depth in Unity units.
        /// </summary>
        [SerializeField] protected int chunkSize = 16;

        /// <summary>
        /// The chunk's prefab that will be instantiated
        /// </summary>
        [SerializeField] protected GameObject chunkPrefab;

        /// <summary>
        /// The density level where a surface will be created. Densities below this will be inside the surface (solid),
        /// and densities above this will be outside the surface (air)
        /// </summary>
        [SerializeField] protected float isolevel;

        /// <summary>
        /// The chunk's size. This represents the width, height and depth in Unity units.
        /// </summary>
        public int ChunkSize => chunkSize;

        /// <summary>
        /// The chunk's prefab that will be instantiated
        /// </summary>
        public GameObject ChunkPrefab => chunkPrefab;

        /// <summary>
        /// The density level where a surface will be created. Densities below this will be inside the surface (solid),
        /// and densities above this will be outside the surface (air)
        /// </summary>
        public float Isolevel => isolevel;

        /// <summary>
        /// All the chunks of this world
        /// </summary>
        public Dictionary<int3, Chunk> Chunks { get; set; }

        protected virtual void Start()
        {
            Chunks = new Dictionary<int3, Chunk>();
        }

        /// <summary>
        /// Tries to get a chunk at a world position
        /// </summary>
        /// <param name="worldPosition">World position of the chunk (can be inside the chunk, doesn't have to be the chunk's origin)</param>
        /// <param name="chunk">The chunk at that position (if any)</param>
        /// <returns>Does a chunk exist at that world position</returns>
        public bool TryGetChunk(int3 worldPosition, out Chunk chunk)
        {
            int3 localPosition = worldPosition -
                new int3(Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y),
                Mathf.RoundToInt(transform.position.z));

            int3 chunkCoordinate = LocalPositionToCoordinate(localPosition);
            return Chunks.TryGetValue(chunkCoordinate, out chunk);
        }

        /// <summary>
        /// Gets the density at a world position
        /// </summary>
        /// <param name="worldPosition">The world position of the density to get</param>
        /// <returns>The density at that world position (0 if it doesn't exist)</returns>
        public float GetDensity(int3 worldPosition)
        {
            int3 localPosition = worldPosition -
                new int3(Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y),
                Mathf.RoundToInt(transform.position.z));

            if (TryGetChunk(localPosition, out Chunk chunk))
            {
                return chunk.GetDensity(localPosition.Mod(ChunkSize));
            }

            return 0;
        }

        /// <summary>
        /// Sets the density at a world position
        /// </summary>
        /// <param name="density">The new density</param>
        /// <param name="worldPosition">The density's world position</param>
        public void SetDensity(float density, int3 worldPosition)
        {
            int3 localPosition = worldPosition -
                new int3(Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y),
                Mathf.RoundToInt(transform.position.z));

            List<int3> modifiedChunkPositions = new List<int3>();
            for (int i = 0; i < 8; i++)
            {       //好像是查询 当前点 是不是处于 chunk 与chunk间的间缝, 因为chunk之间有个一个单位的共享空间
                int3 chunkPos = chunkSize * LocalPositionToCoordinate(localPosition - LookupTables.CubeCorners[i]);
                if (modifiedChunkPositions.Contains(chunkPos)) { continue; }

                if (TryGetChunk(chunkPos, out Chunk chunk))
                {
                    int3 localPos = (localPosition - chunkPos).Mod(ChunkSize + 1);
                    chunk.SetDensity(density, localPos);
                    modifiedChunkPositions.Add(chunkPos);

                }
            }
        }

        /// <summary>
        /// Converts a world position to a chunk coordinate
        /// 将世界位置转换为块坐标
        /// </summary>
        /// <param name="worldPosition">The world-position that should be converted</param>
        /// <returns>The world position converted to a chunk coordinate</returns>
        public int3 WorldPositionToCoordinate(float3 worldPosition)
        {
            float3 localPosition = worldPosition -
                new float3(transform.position.x,
                transform.position.y,
                transform.position.z);

            return localPosition.FloorToMultipleOfX(ChunkSize) / ChunkSize;
        }

        /// <summary>
        /// Converts a world position to  coordinate  in chunk
        /// 将世界位置转换为块坐标
        /// </summary>
        /// <param name="worldPosition">The world-position that should be converted</param>
        /// <returns>The world position converted to a chunk coordinate</returns>
        public int3 WorldPositionToCoordinateInChunk(int3 worldPosition)
        {
            int3 localPosition = worldPosition -
                new int3(Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y),
                Mathf.RoundToInt(transform.position.z));

            return localPosition.Mod(ChunkSize);
        }

        /// <summary>
        /// Converts a local position to a chunk coordinate
        /// 将本地位置转换为块坐标
        /// </summary>
        /// <param name="worldPosition">The world-position that should be converted</param>
        /// <returns>The world position converted to a chunk coordinate</returns>
        public int3 LocalPositionToCoordinate(float3 localPosition)
        {
            return localPosition.FloorToMultipleOfX(ChunkSize) / ChunkSize;
        }
    }
}