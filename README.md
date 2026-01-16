# Unity-VertexGroups
Create and modify vertex groups for a mesh, with an in-editor 3D view to select vertices.

![alt text](https://github.com/Aelstraz/Unity-VertexGroups/blob/main/Screenshot.png?raw=true)

### Importing/Installing:
Simply add the entire folder into your projects Assets folder, and attach the VertexGroups component to a GameObject with a MeshFilter.

### Usage:
After assigning vertex groups, they can be retrieved as followed ways:

    int index = vertexGroups.GetGroupIndex(yourGroupName);
    vertexGroups.GetVertexGroup(index);

### Twitter: [@aelstraz](https://twitter.com/Aelstraz)
