using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for VertexGroup component to allow editing vertex groups in the Unity Editor.
/// </summary>
[CustomEditor(typeof(VertexGroups))]
public class VertexGroupsEditor : Editor
{
    private static float handleSize = 0.02f;
    private List<int> currentGroupIndices = new List<int>();
    private string currentGroupKey = null;
    private bool isEditing = false;
    private Tool previousTool = Tool.None;
    private Vector2 startMousePosition;
    private Rect mouseDragRect = new Rect();
    private bool isDragging = false;
    private string newGroupName = "";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VertexGroups vertexGroup = (VertexGroups)target;

        // Edit mode toggle
        if (!isEditing && GUILayout.Button("Start Editing"))
        {
            isEditing = true;
            previousTool = Tools.current;
            Tools.current = Tool.None;

            if (vertexGroup.GetGroupCount() > 0)
            {
                SelectVertexGroup(vertexGroup, 0);
            }
        }
        if (isEditing)
        {
            if (GUILayout.Button("Stop Editing"))
            {
                isEditing = false;
                Tools.current = previousTool;
                currentGroupKey = null;
                currentGroupIndices.Clear();
            }

            handleSize = EditorGUILayout.Slider("Handle Size", handleSize, 0.001f, 0.1f);

            if (GUILayout.Button("Create New Group"))
            {
                string newName = "Group ";
                int index = 0;

                // Ensure unique group name
                while (vertexGroup.HasGroup(newName + index))
                {
                    index++;
                }

                vertexGroup.AddVertexGroup(newName + index, new int[] { });
                SelectVertexGroup(vertexGroup, vertexGroup.GetGroupCount() - 1);
            }
        }

        GUILayout.Label("Vertex Groups (Count: " + vertexGroup.GetGroupCount() + "):");

        // List vertex groups
        for (int i = 0; i < vertexGroup.GetGroupCount(); i++)
        {
            DrawVertexGroup(vertexGroup, i);
        }

        // Edit current group
        if (isEditing && vertexGroup.GetGroupCount() > 0 && currentGroupKey != null)
        {
            GUILayout.Label("Group Name: ");
            newGroupName = GUILayout.TextField(newGroupName);

            if (GUILayout.Button("Save Current Group"))
            {
                if (newGroupName == currentGroupKey)
                {
                    vertexGroup.UpdateVertexGroup(currentGroupKey, currentGroupIndices);
                }
                else
                {
                    if (vertexGroup.HasGroup(newGroupName))
                    {
                        Debug.LogError("A group with that name already exists.");
                    }
                    else
                    {
                        vertexGroup.RemoveVertexGroup(currentGroupKey);
                        vertexGroup.AddVertexGroup(newGroupName, currentGroupIndices);
                        currentGroupKey = newGroupName;
                    }
                }
            }
            if (GUILayout.Button("Delete Current Group"))
            {
                vertexGroup.RemoveVertexGroup(currentGroupKey);
                SelectVertexGroup(vertexGroup, 0);
            }
        }

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawVertexGroup(VertexGroups vertexGroups, int i)
    {
        (string, int[]) group = vertexGroups.GetVertexGroup(i);
        string buttonLabel = group.Item1 + " (" + group.Item2.Length + " vertices)";

        GUIStyle selectedButtonStyle = new GUIStyle(GUI.skin.button);
        selectedButtonStyle.normal.textColor = Color.green;
        selectedButtonStyle.active.textColor = Color.green;
        selectedButtonStyle.hover.textColor = Color.green;

        if (!isEditing)
        {
            if (currentGroupKey == group.Item1)
            {
                GUILayout.Label(buttonLabel, selectedButtonStyle);
            }
            else
            {
                GUILayout.Label(buttonLabel);
            }
        }
        else if (isEditing)
        {
            if (currentGroupKey == group.Item1)
            {
                if (GUILayout.Button(buttonLabel, selectedButtonStyle))
                {
                    SelectVertexGroup(vertexGroups, i);
                }
            }
            else
            {
                if (GUILayout.Button(buttonLabel))
                {
                    SelectVertexGroup(vertexGroups, i);
                }
            }
        }
    }

    private void SelectVertexGroup(VertexGroups vertexGroups, int groupIndex)
    {
        currentGroupIndices.Clear();
        if (groupIndex < 0 || vertexGroups.GetGroupCount() <= 0 || groupIndex >= vertexGroups.GetGroupCount())
        {
            currentGroupKey = null;
            return;
        }
        (string, int[]) group = vertexGroups.GetVertexGroup(groupIndex);
        currentGroupIndices = group.Item2.ToList();
        currentGroupKey = group.Item1;
        newGroupName = currentGroupKey;
    }

    public void OnSceneGUI()
    {
        VertexGroups vertexGroups = (VertexGroups)target;
        if (vertexGroups == null)
        {
            return;
        }

        MeshFilter meshFilter = vertexGroups.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            return;
        }

        if (isEditing && currentGroupKey != null)
        {
            // Prevent de-selection of game object while in edit mode
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            // Handle mouse events
            Vector2 currentMousePosition = Event.current.mousePosition;
            bool finishedDragging = false;
            bool hasClicked = false;

            //left mouse button down
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                startMousePosition = Event.current.mousePosition;
                isDragging = false;
                hasClicked = true;
            }
            //left mouse button up
            else if (isDragging && Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                finishedDragging = true;
                isDragging = false;
            }
            //left mouse button drag
            else if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                isDragging = true;
                mouseDragRect = new Rect(
                    Mathf.Min(startMousePosition.x, currentMousePosition.x),
                    Mathf.Min(startMousePosition.y, currentMousePosition.y),
                    Mathf.Abs(startMousePosition.x - currentMousePosition.x),
                    Mathf.Abs(startMousePosition.y - currentMousePosition.y)
                );
            }

            Vector3[] vertices = meshFilter.sharedMesh.vertices;
            int[] triangles = meshFilter.sharedMesh.triangles;
            int t1, t2, t3;
            Vector3 v1, v2, v3;

            // Draw mesh triangles
            for (int i = 0; i < triangles.Length; i += 3)
            {
                t1 = triangles[i];
                t2 = triangles[i + 1];
                t3 = triangles[i + 2];
                v1 = meshFilter.transform.TransformPoint(vertices[t1]);
                v2 = meshFilter.transform.TransformPoint(vertices[t2]);
                v3 = meshFilter.transform.TransformPoint(vertices[t3]);
                DrawTriangle(v1, v2, v3);
            }

            // Draw vertex handles
            for (int i = 0; i < vertices.Length; i++)
            {
                DrawVertexHandle(meshFilter.transform.TransformPoint(vertices[i]), i, finishedDragging, hasClicked);
            }

            // Draw drag selection rectangle
            if (isDragging)
            {
                Handles.color = Color.blue;
                Handles.BeginGUI();
                GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
                GUI.backgroundColor = new Color(0f, 0.2f, 1f, 0.5f);
                boxStyle.normal.background = Texture2D.whiteTexture;
                GUI.Box(mouseDragRect, "", boxStyle);
                Handles.EndGUI();
            }
        }
    }

    private void DrawTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        Handles.color = Color.gray;
        Handles.DrawLine(v1, v2);
        Handles.DrawLine(v2, v3);
        Handles.DrawLine(v3, v1);
    }

    private void DrawVertexHandle(Vector3 position, int index, bool finishedDragging, bool hasClicked)
    {
        Handles.color = Color.red;
        if (currentGroupKey != null && currentGroupIndices.Contains(index))
        {
            Handles.color = Color.green;
        }
        Handles.DrawSolidDisc(position, -Camera.current.transform.forward, handleSize);

        Handles.color = Color.black;
        Handles.DrawWireDisc(position, -Camera.current.transform.forward, handleSize, 3f);

        // Handle click selection
        Rect handleRect = HandleUtility.WorldPointToSizedRect(position, new GUIContent(), GUI.skin.button);

        if (finishedDragging && mouseDragRect.Overlaps(handleRect))
        {
            if (currentGroupIndices.Contains(index))
            {
                currentGroupIndices.Remove(index);
            }
            else
            {
                currentGroupIndices.Add(index);
            }
        }
        else if (hasClicked && handleRect.Contains(Event.current.mousePosition))
        {
            if (currentGroupIndices.Contains(index))
            {
                currentGroupIndices.Remove(index);
            }
            else
            {
                currentGroupIndices.Add(index);
            }
        }
    }
}
