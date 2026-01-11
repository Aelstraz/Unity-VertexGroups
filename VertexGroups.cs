using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to create and manage vertex groups for a MeshFilter's mesh.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class VertexGroups : MonoBehaviour
{
    [SerializeField]
    public List<KeyValuePair<string, int[]>> groups = new List<KeyValuePair<string, int[]>>();

    public void AddVertexGroup(string groupName, int[] indices)
    {
        groups.Add(new KeyValuePair<string, int[]>(groupName, indices));
    }

    public void AddVertexGroup(string groupName, List<int> indices)
    {
        groups.Add(new KeyValuePair<string, int[]>(groupName, indices.ToArray()));
    }

    public void UpdateVertexGroup(string groupName, int[] indices)
    {
        int index = -1;
        for (int i = 0; i < groups.Count; i++)
        {
            if (groups[i].Key == groupName)
            {
                index = i;
                break;
            }
        }
        if (index == -1)
        {
            Debug.LogError("Group name not found.");
            return;
        }
        groups.RemoveAt(index);
        groups.Add(new KeyValuePair<string, int[]>(groupName, indices));
    }

    public void UpdateVertexGroup(string groupName, List<int> indices)
    {
        UpdateVertexGroup(groupName, indices.ToArray());
    }

    public void RemoveVertexGroup(string groupName)
    {
        int index = -1;
        for (int i = 0; i < groups.Count; i++)
        {
            if (groups[i].Key == groupName)
            {
                index = i;
                break;
            }
        }
        if (index == -1)
        {
            Debug.LogError("Group name not found.");
            return;
        }

        groups.RemoveAt(index);
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
        int index = -1;
        for (int i = 0; i < groups.Count; i++)
        {
            if (groups[i].Key == groupName)
            {
                index = i;
                break;
            }
        }
        return index != -1;
    }

    public int[] GetVertexGroup(string groupName)
    {
        int index = -1;
        for (int i = 0; i < groups.Count; i++)
        {
            if (groups[i].Key == groupName)
            {
                index = i;
                break;
            }
        }
        if (index == -1)
        {
            Debug.LogError("Group name not found.");
            return null;
        }
        return groups[index].Value;
    }

    public KeyValuePair<string, int[]> GetVertexGroup(int groupIndex)
    {
        if (groupIndex < 0 || groupIndex >= groups.Count)
        {
            Debug.LogError("Group index out of range.");
            return new KeyValuePair<string, int[]>(null, null);
        }
        return groups[groupIndex];
    }
}

[System.Serializable]
public class KeyValuePair<TKey, TValue>
{
    [SerializeField]
    public TKey Key;
    [SerializeField]
    public TValue Value;

    public KeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}
