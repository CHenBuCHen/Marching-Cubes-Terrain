using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using MarchingCubes.Examples;


namespace MarchingCubes.Editor
{
    public class VoxelWorldCreator : ScriptableWizard
    {
        static VoxelWorldCreator wizard;
        int selected = 0;
        WorldShapeBase[] shapes;
        string[] shapeNames;
        [MenuItem("tools/Create Voxel World")]
        static void OpenWindow()
        {
            if (wizard == null)
            {
                wizard = DisplayWizard<VoxelWorldCreator>("VoxelWorldCreator");
                wizard.shapes = new WorldShapeBase[]
                {
                    new PlaneWorld(),
                    new SphereWorld(),new HeightmapWorld()
                };

                int length = wizard.shapes.Length;
                wizard.shapeNames = new string[length];
                for (int i = 0; i < length; i++)
                {
                    wizard.shapeNames[i] = wizard.shapes[i].name;
                }
            }
        }

        void OnWizardCreate()
        {
            shapes[selected].CalculateDensity();
            GameObject terrain = new GameObject("Terrain");
            terrain.transform.position = Vector3.zero;
            VoxelWorld vw = terrain.AddComponent<VoxelWorld>();
            vw.CreateWorld(shapes[selected].worldWidth, shapes[selected].worldLength, shapes[selected].worldHeight,
                shapes[selected].chunkSize, shapes[selected].isolevel, shapes[selected].mat, shapes[selected].density);
        }


        protected override bool DrawWizardGUI()
        {
            selected = GUILayout.Toolbar(selected, shapeNames);
            shapes[selected].OnGUI();
            return true;
        }

    }
}

