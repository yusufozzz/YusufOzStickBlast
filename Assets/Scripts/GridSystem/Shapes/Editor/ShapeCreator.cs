#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using GridSystem.Sticks;
using UnityEditor;
using UnityEngine;

namespace GridSystem.Shapes.Editor
{
    public class ShapeCreator : EditorWindow
    {
        private const int GridSize = 3;
        private const float CellSize = 30f;
        private const string ShapeSavePath = "Assets/Prefabs/Shapes";

        private readonly bool[,] _selectedDots = new bool[GridSize, GridSize];

        private GameObject _shapePrefab;
        private GameObject _stickPrefab;

        private float _spacing = 0.96f;
        private List<GameObject> _spawnedSticks;
        private string _shapeName = "NewShape";

        [MenuItem("Tools/Shape Creator")]
        public static void ShowWindow()
        {
            GetWindow<ShapeCreator>("Shape Creator");
        }

        private void OnGUI()
        {
            GUILayout.Label("5x5 Dot Grid", EditorStyles.boldLabel);
            _spacing = EditorGUILayout.FloatField("Spacing", _spacing);
            _shapeName = EditorGUILayout.TextField("Shape Name", _shapeName);
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

        private void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                AssetDatabase.Refresh();
            }
        }

        private void GenerateShape()
        {
            List<StickPoints> stickPoints = new();
            HashSet<Vector2Int> usedHorizontalCenters = new();
            HashSet<Vector2Int> usedVerticalCenters = new();

            for (int y = 0; y < GridSize; y++)
            {
                for (int x = 0; x < GridSize - 2; x++)
                {
                    if (_selectedDots[x, y] && _selectedDots[x + 1, y] && _selectedDots[x + 2, y])
                    {
                        Vector2Int center = new Vector2Int(x + 1, y);
                        if (usedHorizontalCenters.Contains(center)) continue;

                        usedHorizontalCenters.Add(center);
                        Vector2 worldPos = GetGridAlignedPosition(center.x, center.y);
                        stickPoints.Add(new StickPoints(worldPos, false));
                    }
                }
            }

            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize - 2; y++)
                {
                    if (_selectedDots[x, y] && _selectedDots[x, y + 1] && _selectedDots[x, y + 2])
                    {
                        Vector2Int center = new Vector2Int(x, y + 1);
                        if (usedVerticalCenters.Contains(center)) continue;

                        usedVerticalCenters.Add(center);
                        Vector2 worldPos = GetGridAlignedPosition(center.x, center.y);
                        stickPoints.Add(new StickPoints(worldPos, true));
                    }
                }
            }

            if (stickPoints.Count == 0)
            {
                Debug.LogWarning("No valid stick connections found.");
                return;
            }

            var structure = new ShapeStructure(stickPoints, GridSize, GridSize);
            GameObject shapeObj = (GameObject)PrefabUtility.InstantiatePrefab(_shapePrefab);
            string timestamp = DateTime.Now.ToString("HHmmss");
            string finalShapeName = string.IsNullOrEmpty(_shapeName) ? $"Shape_{timestamp}" : $"{_shapeName}_{timestamp}";
            shapeObj.name = finalShapeName;
            shapeObj.transform.position = Vector3.zero;

            var shape = shapeObj.GetComponent<Shape>();
            BuildFromStructure(shape, structure, _spacing);
            
            // Create the prefab variant
            EnsureDirectoryExists(ShapeSavePath);
            string variantPath = $"{ShapeSavePath}/{finalShapeName}.prefab";
            
            GameObject prefabVariant = PrefabUtility.SaveAsPrefabAsset(shapeObj, variantPath);
            if (prefabVariant != null)
            {
                Debug.Log($"Shape prefab variant saved to: {variantPath}");
                DestroyImmediate(shapeObj); // Destroy the temporary object
                Selection.activeObject = prefabVariant; // Select the new prefab in the Project window
            }
            else
            {
                Debug.LogError("Failed to save shape prefab variant!");
                Undo.RegisterCreatedObjectUndo(shapeObj, "Create Shape");
                Selection.activeGameObject = shapeObj;
            }
        }

        private Vector2 GetGridAlignedPosition(int x, int y)
        {
            // Convert grid coordinates to aligned positions (-spacing, 0, or spacing)
            float posX = x == 0 ? 0 : (x < 0 ? -_spacing : _spacing);
            float posY = y == 0 ? 0 : (y < 0 ? -_spacing : _spacing);
            
            // For coordinates greater than 1 or less than -1
            posX = x * _spacing;
            posY = y * _spacing;
            
            // Ensure values are exactly -spacing, 0, or spacing by rounding to these values
            posX = GetAlignedValue(posX);
            posY = GetAlignedValue(posY);
            
            return new Vector2(posX, posY);
        }
        
        private float GetAlignedValue(float value)
        {
            if (Mathf.Approximately(value, 0f))
                return 0f;
                
            // Determine number of spacing units
            int units = Mathf.RoundToInt(value / _spacing);
            return units * _spacing;
        }

        private void BuildFromStructure(Shape shape, ShapeStructure structure, float spacing)
        {
            List<StickPoints> stickPointsList = new();
            _spawnedSticks = new List<GameObject>();

            foreach (var stick in structure.StickPoints)
            {
                Vector2 alignedPos = new Vector2(
                    GetAlignedValue(stick.Position.x),
                    GetAlignedValue(stick.Position.y)
                );
                
                Vector3 pos = new Vector3(alignedPos.x, alignedPos.y, 0f);
                // Instantiate using PrefabUtility to maintain prefab connection
                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(_stickPrefab, shape.transform);
                obj.transform.localPosition = pos;
                obj.transform.localRotation = Quaternion.Euler(0f, 0f, stick.IsVertical ? 0f : 90f);
                _spawnedSticks.Add(obj);
            }

            Bounds totalBounds = CalculatePreciseBounds(shape);
            CenterPivot(shape, totalBounds.center);

            stickPointsList.Clear();
            foreach (var stickObj in _spawnedSticks)
            {
                bool isVertical = false;
                if (stickObj.TryGetComponent<Stick>(out var stickComponent))
                    isVertical = Mathf.Approximately(stickObj.transform.localEulerAngles.z, 0f);
                else
                    isVertical = Mathf.Approximately(stickObj.transform.localEulerAngles.z, 0f);

                Vector2 localPos = stickObj.transform.localPosition;
                // Make sure the position is grid-aligned even after centering
                Vector2 alignedPos = new Vector2(
                    GetAlignedValue(localPos.x),
                    GetAlignedValue(localPos.y)
                );
                stickPointsList.Add(new StickPoints(alignedPos, isVertical));
            }

            shape.SetStickPoints(stickPointsList);
        }

        private void CenterPivot(Shape shape, Vector3 pivotOffset)
        {
            // Adjust pivot offset to grid alignment
            Vector3 alignedOffset = new Vector3(
                GetAlignedValue(pivotOffset.x),
                GetAlignedValue(pivotOffset.y),
                0f
            );
            
            foreach (var stick in _spawnedSticks)
            {
                stick.transform.localPosition -= alignedOffset;
                
                // Ensure final position is grid-aligned
                Vector3 pos = stick.transform.localPosition;
                stick.transform.localPosition = new Vector3(
                    GetAlignedValue(pos.x),
                    GetAlignedValue(pos.y),
                    0f
                );
            }
        }

        private Bounds CalculatePreciseBounds(Shape shape)
        {
            if (_spawnedSticks.Count == 0)
                return new Bounds(Vector3.zero, Vector3.zero);

            Renderer firstRenderer = _spawnedSticks[0].GetComponentInChildren<Renderer>();
            if (firstRenderer == null)
                return CalculateBoundsFromTransforms();

            Bounds firstBounds = firstRenderer.bounds;
            Vector3 min = shape.transform.InverseTransformPoint(firstBounds.min);
            Vector3 max = shape.transform.InverseTransformPoint(firstBounds.max);
            Bounds localBounds = new Bounds();
            localBounds.SetMinMax(min, max);

            for (int i = 1; i < _spawnedSticks.Count; i++)
            {
                Renderer renderer = _spawnedSticks[i].GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    Bounds rendererBounds = renderer.bounds;
                    min = shape.transform.InverseTransformPoint(rendererBounds.min);
                    max = shape.transform.InverseTransformPoint(rendererBounds.max);
                    localBounds.Encapsulate(min);
                    localBounds.Encapsulate(max);
                }
            }

            return localBounds;
        }

        private Bounds CalculateBoundsFromTransforms()
        {
            if (_spawnedSticks.Count == 0)
                return new Bounds(Vector3.zero, Vector3.zero);

            Bounds bounds = new Bounds(_spawnedSticks[0].transform.localPosition, Vector3.zero);

            foreach (var stick in _spawnedSticks)
            {
                bounds.Encapsulate(stick.transform.localPosition);

                float stickLength = 1.0f;
                Vector3 size = stick.transform.localScale * stickLength;
                Vector3 halfSize = size * 0.5f;

                bounds.Encapsulate(stick.transform.localPosition + new Vector3(halfSize.x, halfSize.y, 0));
                bounds.Encapsulate(stick.transform.localPosition - new Vector3(halfSize.x, halfSize.y, 0));
            }

            return bounds;
        }
    }
}
#endif