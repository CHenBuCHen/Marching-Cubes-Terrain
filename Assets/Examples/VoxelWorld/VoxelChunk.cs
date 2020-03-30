using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


namespace MarchingCubes.Examples
{
    /// <summary>
    /// 可编辑的地块 通过serializedDensity 保存数据
    /// </summary>
    public class VoxelChunk : Chunk
    {
        [HideInInspector] public float[] serializedDensity;//float[,,] can not Serialize. I use float[]

        public VoxelWorld world;

        protected override void Awake()
        {
            VariablesCheck();
            StartDensityCalculation();
            StartMeshGeneration();
        }

        public override void StartDensityCalculation()
        {
            for (int i = 0; i < serializedDensity.Length; i++)
            {
                _densities[i] = serializedDensity[i];
            }

        }

        public override void SetDensity(float density, int x, int y, int z)
        {
            base.SetDensity(density, x, y, z);

#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                serializedDensity[x * (ChunkSize + 1) * (ChunkSize + 1) + y * (ChunkSize + 1) + z] = density;
            }
#endif
        }
#if UNITY_EDITOR
        public void UpdateInEditorMode()
        {
            Update();
        }
#endif
    }
}

