using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MarchingCubes.Editor
{
    public class PlaneWorld : WorldShapeBase
    {
        public override string name => "Plane";
        public int planeHeight = 1;
        public override void OnGUI()
        {
            base.OnGUI();
            planeHeight = EditorGUILayout.IntField("Plane Height", planeHeight);
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
                        if (y >= planeHeight)
                            density[x, y, z] = 1;
                        else
                            density[x, y, z] = -1;
                    }
                }                
            }
        }
    }
}

