using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Component to create and manage vertex groups for a MeshFilter's mesh.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class VertexGroups : MonoBehaviour
{
    public Dictionary<string, int[]> groups = new Dictionary<string, int[]>();

    public void AddVertexGroup(string groupName, int[] indices)
    {
        groups.Add(groupName, indices);
    }

    public void AddVertexGroup(string groupName, List<int> indices)
    {
        groups.Add(groupName, indices.ToArray());
    }

    public void UpdateVertexGroup(string groupName, int[] indices)
    {
        if (!groups.ContainsKey(groupName))
        {
            Debug.LogError("Group name not found.");
            return;
        }
        groups[groupName] = indices;
    }

    public void UpdateVertexGroup(string groupName, List<int> indices)
    {
        UpdateVertexGroup(groupName, indices.ToArray());
    }

    public void RemoveVertexGroup(string groupName)
    {
        if (!groups.ContainsKey(groupName))
        {
            Debug.LogError("Group name not found.");
            return;
        }

        groups.Remove(groupName);
    }

    public void ClearVertexGroups()
    {
        groups.Clear();
    }

    public int GetGroupCount()
    {
        return groups.Count;
    }

    public bool HasGroup(string groupName)
    {
        return groups.ContainsKey(groupName);
    }

    public int[] GetVertexGroup(string groupName)
    {
        if (!groups.ContainsKey(groupName))
        {
            Debug.LogError("Group name not found.");
            return null;
        }
        return groups[groupName];
    }

    public (string, int[]) GetVertexGroup(int groupIndex)
    {
        if (groupIndex < 0 || groupIndex >= groups.Count)
        {
            Debug.LogError("Group index out of range.");
            return (null, null);
        }
        string groupName = groups.Keys.ElementAt(groupIndex);
        return (groupName, groups[groupName]);
    }
}
