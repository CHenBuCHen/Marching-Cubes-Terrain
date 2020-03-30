using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MarchingCubes.Editor
{
    public class HeightmapWorld : WorldShapeBase
    {
        public override string name => "HeightmapWorld";


        Texture2D heightmap;
        int heightOffset;
        float amplitude;

        public override void OnGUI()
        {
            base.OnGUI();

            EditorGUI.BeginChangeCheck();
            heightmap = EditorGUILayout.ObjectField(new GUIContent("heightmap"), heightmap, typeof(Texture2D)) as Texture2D;

            if (EditorGUI.EndChangeCheck())
            {
                worldWidth = heightmap.width;
                worldLength = heightmap.height;
            }
            amplitude = EditorGUILayout.FloatField("amplitude", amplitude);

            heightOffset = EditorGUILayout.IntField("heightOffset", heightOffset);

        }

        public override void CalculateDensity()
        {
            base.CalculateDensity();


            for (int x = 0; x <= worldWidth; x++)
            {
                for (int y = 0; y <= worldHeight; y++)
                {
                    for (int z = 0; z <= worldLength; z++)
                    {
                        //  amplitude = 30, 34, 40,60,68
                        density[x, y, z] = Mathf.Clamp(y - heightmap.GetPixel(x, z).grayscale * amplitude - heightOffset, -1, 1);
                    }
                }
            }
        }
    }
}

