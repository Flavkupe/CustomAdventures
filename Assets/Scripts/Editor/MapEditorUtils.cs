using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class MapEditorUtils
{
    [MenuItem("Assets/Utils/Map Gen/Align Map Rooms")]
    private static void AlignRooms()
    {
        var parent = new GameObject("Utils-AlignRooms");

        var xPos = 0.0f;
        var yPos = 0.0f;
        var selectedAsset = Selection.GetFiltered<Room>(SelectionMode.DeepAssets);
        foreach (var room in selectedAsset)
        {
            // Debug.Log("Asset name: " + room.name + "   Type: " + room.GetType());
            var clone = PrefabUtility.InstantiatePrefab(room) as Room;
            clone.transform.SetParent(parent.transform);
            clone.transform.localPosition = new Vector3(xPos, yPos);
            xPos += clone.Dims;
            if (xPos >= clone.Dims * 5)
            {
                xPos = 0.0f;
                yPos += clone.Dims;
            }
        }
    }
}

