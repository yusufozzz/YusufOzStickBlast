#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using GridSystem.Sticks;
using UnityEditor;
using UnityEngine;

namespace GridSystem.Shapes.Editor
{
    public class ShapeCreator : EditorWindow
    {
        private const int GridSize = 5;
        private const float CellSize = 30f;

        private readonly bool[,] _selectedDots = new bool[GridSize, GridSize];

        private GameObject _shapePrefab;
        private GameObject _stickPrefab;

        private float _spacing = 1.288f;

        [MenuItem("Tools/Shape Creator")]
        public static void ShowWindow()
        {
            GetWindow<ShapeCreator>("Shape Creator");
        }

        private void OnGUI()
        {
            GUILayout.Label("5x5 Dot Grid", EditorStyles.boldLabel);

            _spacing = EditorGUILayout.FloatField("Spacing", _spacing);

            DrawDotGrid();

            GUILayout.Space(10);

            if (GUILayout.Button("Generate Shape"))
            {
                LoadPrefabs();
                if (_shapePrefab == null || _stickPrefab == null)
                {
                    Debug.LogError("Missing Shape or Stick prefab.");
                    return;
                }

                GenerateShape();
            }
        }

        private void DrawDotGrid()
        {
            for (int y = GridSize - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < GridSize; x++)
                {
                    _selectedDots[x, y] = GUILayout.Toggle(_selectedDots[x, y], "", "Button", GUILayout.Width(CellSize), GUILayout.Height(CellSize));
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void LoadPrefabs()
        {
            if (_shapePrefab == null)
                _shapePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Shapes/Shape.prefab");

            if (_stickPrefab == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:Prefab");
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (obj != null && obj.GetComponent<Stick>() != null)
                    {
                        _stickPrefab = obj;
                        break;
                    }
                }
            }
        }

        private void GenerateShape()
        {
            List<StickTransform> stickTransforms = new();

            for (int y = 0; y < GridSize; y++)
            {
                for (int x = 0; x < GridSize - 2; x++)
                {
                    if (_selectedDots[x, y] && _selectedDots[x + 1, y] && _selectedDots[x + 2, y])
                        stickTransforms.Add(new StickTransform(new Vector2Int(x + 1, y), false));
                }
            }

            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize - 2; y++)
                {
                    if (_selectedDots[x, y] && _selectedDots[x, y + 1] && _selectedDots[x, y + 2])
                        stickTransforms.Add(new StickTransform(new Vector2Int(x, y + 1), true));
                }
            }

            if (stickTransforms.Count == 0)
            {
                Debug.LogWarning("No valid stick connections found.");
                return;
            }

            var structure = new ShapeStructure(stickTransforms, GridSize, GridSize);

            GameObject shapeObj = (GameObject)PrefabUtility.InstantiatePrefab(_shapePrefab);
            shapeObj.name = $"Shape_{DateTime.Now:HHmmss}";
            shapeObj.transform.position = Vector3.zero;

            var shape = shapeObj.GetComponent<Shape>();
            shape.GetType().GetField("stickPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(shape, _stickPrefab);

            shape.BuildFromStructure(structure, _spacing);


            Undo.RegisterCreatedObjectUndo(shapeObj, "Create Shape");
            Selection.activeGameObject = shapeObj;
        }
    }
}
#endif
