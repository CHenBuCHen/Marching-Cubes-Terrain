using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MarchingCubes.Examples;
using Unity.Mathematics;

namespace MarchingCubes.Editor
{
    public class BrushBase
    {
        public float size = 1;
        public float power = 0.1f;


        RaycastHit hit;
        VoxelChunk targetOnRay;

        public virtual void OnGUI()
        {
            size = EditorGUILayout.Slider("size", size, 0, 10);
            power = EditorGUILayout.Slider("power", power, -0.2f, 0.2f);
        }

        public virtual void OnSceneGUI()
        {
            Vector2 guiPosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
            Physics.Raycast(ray);
            if (Physics.Raycast(ray, out hit, 1000))
            {
                VoxelChunk chunk = hit.collider.GetComponent<VoxelChunk>();

                if (chunk != targetOnRay)
                {
                    targetOnRay = chunk;
                }
            }

            EditorWindow view = EditorWindow.GetWindow<SceneView>();
            view.Repaint();
            if (Application.isPlaying == false)
            {
                if (targetOnRay != null)
                {
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
                    Handles.color = new Color(1, 0, 0, 0.3f);
                    Handles.SphereHandleCap(0, hit.point, Quaternion.identity, size, EventType.Repaint);
                    switch (Event.current.GetTypeForControl(0))
                    {
                        case EventType.MouseDrag:
                            if (Event.current.button == 0)
                            {
                                EditTerrain(hit.point, power, size);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

        }


        private void EditTerrain(Vector3 point, float deformSpeed, float range)
        {
            int buildModifier = deformSpeed > 0 ? 1 : -1;
            deformSpeed = Mathf.Abs(deformSpeed);

            int hitX = Mathf.RoundToInt(point.x);
            int hitY = Mathf.RoundToInt(point.y);
            int hitZ = Mathf.RoundToInt(point.z);

            int intRange = Mathf.CeilToInt(range);

            for (int x = -intRange; x <= intRange; x++)
            {
                for (int y = -intRange; y <= intRange; y++)
                {
                    for (int z = -intRange; z <= intRange; z++)
                    {
                        int offsetX = hitX - x;
                        int offsetY = hitY - y;
                        int offsetZ = hitZ - z;

                        var offsetPoint = new int3(offsetX, offsetY, offsetZ);
                        float distance = math.distance(offsetPoint, point);
                        if (distance > range)
                        {
                            continue;
                        }

                        float modificationAmount = deformSpeed / distance * buildModifier;

                        float oldDensity = targetOnRay.world.GetDensity(offsetPoint);
                        float newDensity = Mathf.Clamp(oldDensity - modificationAmount, -1, 1);

                        targetOnRay.world.SetDensity(newDensity, offsetPoint);
                    }
                }
            }
        }
    }
}

