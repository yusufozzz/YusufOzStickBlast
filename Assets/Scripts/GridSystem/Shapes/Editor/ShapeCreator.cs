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
        private List<GameObject> _spawnedSticks;

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
                    _selectedDots[x, y] = GUILayout.Toggle(_selectedDots[x, y], "", "Button", GUILayout.Width(CellSize),
                        GUILayout.Height(CellSize));
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
            List<StickPoints> stickPoints = new();

            // Find horizontal sticks (3 consecutive horizontal dots)
            for (int y = 0; y < GridSize; y++)
            {
                for (int x = 0; x < GridSize - 2; x++)
                {
                    if (_selectedDots[x, y] && _selectedDots[x + 1, y] && _selectedDots[x + 2, y])
                    {
                        // Convert grid position to world position using spacing
                        Vector2 worldPos = new Vector2((x + 1) * _spacing, y * _spacing);
                        stickPoints.Add(new StickPoints(worldPos, false));
                    }
                }
            }

            // Find vertical sticks (3 consecutive vertical dots)
            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize - 2; y++)
                {
                    if (_selectedDots[x, y] && _selectedDots[x, y + 1] && _selectedDots[x, y + 2])
                    {
                        // Convert grid position to world position using spacing
                        Vector2 worldPos = new Vector2(x * _spacing, (y + 1) * _spacing);
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

            // Create the shape object
            GameObject shapeObj = (GameObject)PrefabUtility.InstantiatePrefab(_shapePrefab);
            shapeObj.name = $"Shape_{DateTime.Now:HHmmss}";
            shapeObj.transform.position = Vector3.zero;

            // Get the Shape component and set the stick prefab
            var shape = shapeObj.GetComponent<Shape>();

            // Build the shape with the structure
            BuildFromStructure(shape, structure, _spacing);

            Undo.RegisterCreatedObjectUndo(shapeObj, "Create Shape");
            Selection.activeGameObject = shapeObj;
        }

        /// <summary>
        /// Builds the shape from a given structure and spacing - moved from Shape.cs to ShapeCreator
        /// </summary>
        private void BuildFromStructure(Shape shape, ShapeStructure structure, float spacing)
        {
            List<StickPoints> stickPointsList = new List<StickPoints>();

            _spawnedSticks = new List<GameObject>();
            // Create new sticks based on the structure
            foreach (var stick in structure.StickPoints)
            {
                // Use the position directly as it's already in world-space coordinates
                Vector3 pos = new Vector3(stick.Position.x, stick.Position.y, 0f);
                GameObject obj = Instantiate(_stickPrefab, shape.transform);
                obj.transform.localPosition = pos;
                obj.transform.localRotation = Quaternion.Euler(0f, 0f, stick.IsVertical ? 0f : 90f);
                _spawnedSticks.Add(obj);

                // Save the actual local position in stick points
                stickPointsList.Add(new StickPoints(obj.transform.localPosition, stick.IsVertical));
            }

            // Calculate bounds and center the pivot
            Bounds totalBounds = CalculatePreciseBounds(shape);
            CenterPivot(shape, totalBounds.center);

            // Update the stick points with the new centered positions
            stickPointsList.Clear();
            foreach (var stickObj in _spawnedSticks)
            {
                // Get stick component to determine if it's vertical
                bool isVertical = false;
                if (stickObj.TryGetComponent<Stick>(out var stickComponent))
                {
                    // Determine if vertical based on rotation (assuming 90 degrees is horizontal)
                    isVertical = Mathf.Approximately(stickObj.transform.localEulerAngles.z, 0f);
                }
                else
                {
                    // Fallback if no stick component - check the local rotation
                    isVertical = Mathf.Approximately(stickObj.transform.localEulerAngles.z, 0f);
                }

                // Store the actual local position
                Vector2 localPos = stickObj.transform.localPosition;
                stickPointsList.Add(new StickPoints(localPos, isVertical));
            }

            // Set the stick points on the shape
            shape.SetStickPoints(stickPointsList);
        }

        /// <summary>
        /// Centers the pivot point of the shape - moved from Shape.cs
        /// </summary>
        private void CenterPivot(Shape shape, Vector3 pivotOffset)
        {
            var sticks = _spawnedSticks;
            foreach (var stick in sticks)
            {
                stick.transform.localPosition -= pivotOffset;
            }
        }

        /// <summary>
        /// Calculates precise bounds from all stick renderers - moved from Shape.cs
        /// </summary>
        private Bounds CalculatePreciseBounds(Shape shape)
        {
            var sticks = _spawnedSticks;

            if (sticks.Count == 0)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }

            // Initialize bounds with the first renderer's bounds
            Renderer firstRenderer = sticks[0].GetComponentInChildren<Renderer>();
            if (firstRenderer == null)
            {
                // Fallback to transform positions if no renderers are found
                return CalculateBoundsFromTransforms(shape);
            }

            // Convert the world space bounds to local space
            Bounds firstBounds = firstRenderer.bounds;
            Vector3 min = shape.transform.InverseTransformPoint(firstBounds.min);
            Vector3 max = shape.transform.InverseTransformPoint(firstBounds.max);
            Bounds localBounds = new Bounds();
            localBounds.SetMinMax(min, max);

            // Include all other renderers
            for (int i = 1; i < sticks.Count; i++)
            {
                Renderer renderer = sticks[i].GetComponentInChildren<Renderer>();
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

        /// <summary>
        /// Calculates bounds based on transforms when renderers are not available - moved from Shape.cs
        /// </summary>
        private Bounds CalculateBoundsFromTransforms(Shape shape)
        {
            var sticks = _spawnedSticks;

            if (sticks.Count == 0)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }

            Bounds bounds = new Bounds(sticks[0].transform.localPosition, Vector3.zero);

            foreach (var stick in sticks)
            {
                bounds.Encapsulate(stick.transform.localPosition);

                // Approximate the stick size based on scale
                // This is a fallback, but Renderer-based calculation is more accurate
                float stickLength = 1.0f; // Assuming the stick's length is 1 unit
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