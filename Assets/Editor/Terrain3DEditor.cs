﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Name.Terrain;

namespace NameEditor.Terrain
{
    [CustomEditor(typeof(Terrain3D))]
    public class Terrain3DEditor : Editor
    {
        private string[] tabs =
        {
            "Elevation",
            "Paint",
            "Properties"
        };

        private int selectedTab = 0;

        private bool isEditing = false;

        private Terrain3DElevation elevation = new Terrain3DElevation();
        private Terrain3DPaint paint = new Terrain3DPaint();

        public override void OnInspectorGUI()
        {
            Terrain3D terrain3D = (Terrain3D)target;

            selectedTab = GUILayout.Toolbar(selectedTab, tabs);

            switch (selectedTab)
            {
                case 0:
                    elevation.DrawInspector();
                    break;

                case 1:
                    paint.DrawInspector();
                    break;

                case 2:
                    DrawProperties();
                    break;

                default:
                    break;
            }

            GUILayout.BeginHorizontal();

            if (isEditing)
            {
                if (GUILayout.Button("Disable Editing"))
                {
                    isEditing = false;
                }
            }
            else
            {
                if (GUILayout.Button("Enable Editing"))
                {
                    isEditing = true;
                }
            }

            if (GUILayout.Button("Force Refresh"))
            {

            }

            GUILayout.EndHorizontal();
        }

        private void OnEnable()
        {
            Terrain3D terrain3D = (Terrain3D)target;
            terrain3D.Initialize();
            terrain3D.Refresh();
            Tools.hidden = true;
        }

        private void OnDisable()
        {
            Terrain3D terrain3D = (Terrain3D)target;
            terrain3D.Refresh();
            Tools.hidden = false;
        }

        private void OnSceneGUI()
        {
            Terrain3D terrain3D = (Terrain3D)target;

            Event e = Event.current;

            if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
            {
                int controlId = GUIUtility.GetControlID(FocusType.Passive);
                GUIUtility.hotControl = controlId;

                // Cast a ray through a screen point and return the hit point
                Camera cam = Camera.current;
                if (!cam)
                {
                    return;
                }

                SceneView sceneView = SceneView.currentDrawingSceneView;
                if (!sceneView)
                {
                    return;
                }

                Vector3 mousePosition = e.mousePosition;
                mousePosition.y = sceneView.camera.pixelHeight - e.mousePosition.y;


                Ray ray = cam.ScreenPointToRay(mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Debug.Log("Test");

                    Debug.DrawLine(ray.origin, hit.point);

                    // Transform the hit point from world space to local space
                    Vector3 localHit = terrain3D.transform.InverseTransformPoint(hit.point);
                    TerrainChunk chunk = terrain3D.terrainChunk;
            
                    int hitX = (int)(localHit.x / chunk.multiplier);
                    int hitY = (int)(localHit.y / chunk.multiplier);
                    int hitZ = (int)(localHit.z / chunk.multiplier);

                    chunk.voxels[hitX + chunk.size * hitY + chunk.size2 * hitZ].value += 0.5f * Time.deltaTime;
                    chunk.voxels[(hitX + chunk.size * hitY + chunk.size2 * hitZ) + chunk.size].value += 0.1f * Time.deltaTime;

                    Refresh();
                }

                e.Use();
            }
        }

        [MenuItem("GameObject/Create Other/Terrain3D")]
        private static void Create()
        {
            GameObject terrain3DGameObject = new GameObject("Terrain3D");
            Terrain3D terrain3D = (Terrain3D)terrain3DGameObject.AddComponent(typeof(Terrain3D));
            terrain3D.Initialize();
            terrain3D.Refresh();
            Terrain3DObject terrain3DObject = CreateInstance<Terrain3DObject>();
            AssetDatabase.CreateAsset(terrain3DObject, "Assets/Terrain3D.asset");
            AssetDatabase.SaveAssets();
        }

        private void DrawProperties()
        {
            Terrain3D terrain3D = (Terrain3D)target;

            terrain3D.chunks = EditorGUILayout.IntField("Chunks", terrain3D.chunks);
            terrain3D.chunkSize = EditorGUILayout.IntField("Chunk Size", terrain3D.chunkSize);
            terrain3D.resolution = EditorGUILayout.IntField("Resolution", terrain3D.resolution);
        }

        private void Initialize()
        {
            Terrain3D terrain3D = (Terrain3D)target;
            terrain3D.Initialize();
        }

        private void Refresh()
        {
            Terrain3D terrain3D = (Terrain3D)target;
            terrain3D.Refresh();
        }
    }
}
