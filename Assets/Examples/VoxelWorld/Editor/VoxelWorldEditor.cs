using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MarchingCubes.Examples;
using Unity.Mathematics;

namespace MarchingCubes.Editor
{
    [CustomEditor(typeof(VoxelWorld))]
    public class VoxelWorldEditor : UnityEditor.Editor
    {
        public float power = 0.1f;

        VoxelWorld world;
        VoxelChunk targetOnRay;
        RaycastHit hit;

        int selected = 0;

        BrushBase brush = new BrushBase();
        private void OnEnable()
        {
            world = target as VoxelWorld;
            if (Application.isPlaying == false)
                world.VariablesCheck();
        }

        private void OnDisable()
        {
            //if (Application.isPlaying == false)
                world.Dispose();
        }

        public override void OnInspectorGUI()
        {

            EditorGUI.BeginChangeCheck();
            if (Application.isPlaying)
                EditorGUILayout.LabelField("can not use in Play Mode!");

            selected = GUILayout.Toolbar(selected, new string[] { "Point", "Brush" });

            switch (selected)
            {
                case 0:
                    EditorGUILayout.LabelField("click vertex to change density.");
                    power = EditorGUILayout.Slider("power", power, -1, 1);
                    break;
                case 1:
                    brush.OnGUI();
                    break;
                default:
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                //force-refresh Scene View
                EditorWindow view = EditorWindow.GetWindow<SceneView>();
                view.Repaint();
            }

        }





        private void DrawBrushGUI()
        {
            brush.OnSceneGUI();
        }


        void OnSceneGUI()
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlID);


            Vector2 guiPosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
            Physics.Raycast(ray);
            if (Physics.Raycast(ray, out hit, 1000))
            {
                VoxelChunk chunk = hit.collider.GetComponent<VoxelChunk>();

                if(chunk != targetOnRay)
                {
                    targetOnRay = chunk;
                }
            }

            world.UpdateInEditorMode();

            DrawWireCube();

            switch (selected)
            {
                case 0:
                    DrawPointGUI();
                    break;
                case 1:
                    DrawBrushGUI();
                    break;
                default:
                    break;
            }


        }

        void DrawWireCube()
        {
            Vector3 size = world.ChunkSize * Vector3.one;

            if (targetOnRay)
            {
                Handles.DrawWireCube(targetOnRay.transform.position + size / 2, size);
            }

        }




        private void DrawPointGUI()
        {
            if (targetOnRay != null)
            {
                int3 worldPosition = new int3(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.y), Mathf.FloorToInt(hit.point.z));
                int3 posCoord = world.WorldPositionToCoordinateInChunk(worldPosition);

                Vector3 worldPos = targetOnRay.transform.position + posCoord.ToVectorInt();
                DrawCube(worldPos);
                
                GetAndShowDensities(targetOnRay, posCoord);
            }


        }

        private void DrawCube(Vector3 worldPos)
        {
            Vector3 vertex1 = worldPos;
            Vector3 vertex2 = worldPos + new Vector3(1, 0, 0);
            Vector3 vertex3 = worldPos + new Vector3(1, 1, 0);
            Vector3 vertex4 = worldPos + new Vector3(0, 1, 0);
            Vector3 vertex5 = worldPos + new Vector3(0, 0, 1);
            Vector3 vertex6 = worldPos + new Vector3(1, 0, 1);
            Vector3 vertex7 = worldPos + new Vector3(1, 1, 1);
            Vector3 vertex8 = worldPos + new Vector3(0, 1, 1);

            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            Handles.color = Color.red;
            Handles.DrawWireCube(worldPos + Vector3.one * 0.5f, Vector3.one);

            Handles.zTest = UnityEngine.Rendering.CompareFunction.GreaterEqual;
            Color color = new Color(1f, 1f, 1f, 0.4f);
            Handles.color = Color.white;

            Handles.DrawSolidRectangleWithOutline(
                new Vector3[] { vertex1, vertex2, vertex3, vertex4 },
                color, color);
            Handles.DrawSolidRectangleWithOutline(
                new Vector3[] { vertex2, vertex3, vertex7, vertex6 },
                color, color);
            Handles.DrawSolidRectangleWithOutline(
                new Vector3[] { vertex5, vertex6, vertex7, vertex8 },
                color, color);
            Handles.DrawSolidRectangleWithOutline(
                new Vector3[] { vertex5, vertex8, vertex4, vertex1 },
                color, color);
            Handles.DrawSolidRectangleWithOutline(
                new Vector3[] { vertex3, vertex4, vertex7, vertex8 },
                color, color);
            Handles.DrawSolidRectangleWithOutline(
                new Vector3[] { vertex1, vertex2, vertex6, vertex5 },
                color, color);
                                              
        }

        private void GetAndShowDensities(VoxelChunk chunk, int3 localPosition)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Disabled;
            for (int i = 0; i < 8; i++)
            {
                float density = 0;
                int3 voxelCorner = localPosition + LookupTables.CubeCorners[i];
                Vector3 worldPos = voxelCorner.ToVectorInt() + chunk.transform.position;
                int densityIndex = voxelCorner.x * (chunk.ChunkSize + 1) * (chunk.ChunkSize + 1) + voxelCorner.y * (chunk.ChunkSize + 1) + voxelCorner.z;

                if (densityIndex >= 0 && densityIndex < chunk.serializedDensity.Length)
                    density = chunk.serializedDensity[densityIndex];

                if(Application.isPlaying == false)
                {
                    Color _temp = Handles.color;
                    Handles.color = Handles.xAxisColor;
                    if (Handles.Button(worldPos, Quaternion.identity, 0.05f, 0.1f, Handles.DotHandleCap))
                    {
                        density = Mathf.Clamp(density + power, -1, 1);
                        world.SetDensity(density, new int3(Mathf.RoundToInt(worldPos.x),
                                                    Mathf.RoundToInt(worldPos.y),
                                                    Mathf.RoundToInt(worldPos.z)));
                        //targetOnRay.SetDensity(density, voxelCorner);
                        //if (densityIndex >= 0 && densityIndex < chunk.serializedDensity.Length)
                        //    chunk.serializedDensity[densityIndex] = density;
                    }
                    Handles.color = _temp;
                }


                Handles.Label(worldPos, density.ToString("f2"));

            }
        }
    }
}

