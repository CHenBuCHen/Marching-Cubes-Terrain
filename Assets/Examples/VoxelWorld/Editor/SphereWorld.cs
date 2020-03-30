using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace MarchingCubes.Editor
{
    public class SphereWorld : WorldShapeBase
    {
        public override string name => "Sphere";

        public Vector3Int position;
        public int radius;

        public float polygon = 0;   //[0,1]  

        public override void OnGUI()
        {
            base.OnGUI();

            position = EditorGUILayout.Vector3IntField("Sphere Position", position);
            radius = EditorGUILayout.IntField("Sphere Radius", radius);
            polygon = EditorGUILayout.Slider("polygon", polygon, 0, 1);
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
                        Vector3Int current = new Vector3Int(x, y, z);
                        float distance = Vector3Int.Distance(current, position);
                        float _density = distance - radius;
                        if (_density > 1 || _density < -1)
                        {
                            _density = Mathf.Clamp(_density, -1, 1);
                        }
                        else
                        {
                            if (polygon > 0 && polygon < 1)
                                _density = ((int)(_density / polygon)) * polygon;
                        }
                        density[x, y, z] = _density;
                    }
                }
            }
        }
    }
}

