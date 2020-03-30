using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MarchingCubes.Editor
{
    public class WorldShapeBase
    {
        public virtual string name => "null";
        
        public int worldWidth = 16;
        public int worldLength = 16;
        public int worldHeight = 16;
        public int chunkSize = 16;
        public float isolevel = 0;
        public Material mat;

        public float[,,] density;

        public WorldShapeBase()
        {
            //CalculateDensity();
        }


        public virtual void OnGUI()
        {
            worldWidth = EditorGUILayout.IntField("World Width", worldWidth);
            worldLength = EditorGUILayout.IntField("World Length", worldLength);
            worldHeight = EditorGUILayout.IntField("World  Height", worldHeight);

            chunkSize = EditorGUILayout.IntField("Chunk Size", chunkSize);
            isolevel = EditorGUILayout.FloatField("isolevel", isolevel);
            mat = EditorGUILayout.ObjectField(new GUIContent("Material"), mat, typeof(Material), false) as Material;


        }

        public virtual void CalculateDensity()
        {
            density = new float[worldWidth + 1, worldHeight + 1, worldLength + 1];
        }

    }
}


