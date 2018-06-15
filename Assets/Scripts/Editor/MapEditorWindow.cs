using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class MapEditorWindow : EditorWindow
{
    private RoomStats _stats;

    [MenuItem("Window/Utils/Map Editor Helper")]

    public static void ShowWindow()
    {
        var window = GetWindow(typeof(MapEditorWindow)) as MapEditorWindow;
        window.RefreshRoomStats();
    }

    void OnGUI()
    {
        GUILayout.Label("Room stats", EditorStyles.boldLabel);
        if (GUILayout.Button("Refresh"))
        {
            RefreshRoomStats();
        }

        GUILayout.Label("Left connectors: " + _stats.LeftConnectors, EditorStyles.label);
        GUILayout.Label("Right connectors: " + _stats.RightConnectors, EditorStyles.label);
        GUILayout.Label("Top connectors: " + _stats.TopConnectors, EditorStyles.label);
        GUILayout.Label("Bottom connectors: " + _stats.BottomConnectors, EditorStyles.label);

        GUILayout.Label("Specific Left connectors:", EditorStyles.boldLabel);
        PrintDictionary(_stats.LeftByCoord, _stats.RightByCoord);
        GUILayout.Label("Specific Right connectors:", EditorStyles.boldLabel);
        PrintDictionary(_stats.RightByCoord, _stats.LeftByCoord);
        GUILayout.Label("Specific Top connectors:", EditorStyles.boldLabel);
        PrintDictionary(_stats.TopByCoord, _stats.BottomByCoord);
        GUILayout.Label("Specific Bottom connectors:", EditorStyles.boldLabel);
        PrintDictionary(_stats.BottomByCoord, _stats.TopByCoord);
    }

    private void PrintDictionary(Dictionary<int, int> dict, Dictionary<int, int> oppositeDict)
    {
        var style = new GUIStyle(EditorStyles.label);
        foreach (var key in dict.Keys)
        {
            if (!oppositeDict.ContainsKey(key))
            {
                style.normal.textColor = Color.red;
            }
            else
            {
                style.normal.textColor = Color.gray;
            }

            GUILayout.Label("  " + key + " -> " + dict[key], style);
        }
    }

    private class RoomStats
    {
        public int LeftConnectors;
        public int RightConnectors;
        public int TopConnectors;
        public int BottomConnectors;

        // Key is coord, second is count
        public Dictionary<int, int> BottomByCoord = new Dictionary<int, int>();
        public Dictionary<int, int> TopByCoord = new Dictionary<int, int>();
        public Dictionary<int, int> LeftByCoord = new Dictionary<int, int>();
        public Dictionary<int, int> RightByCoord = new Dictionary<int, int>();
    }

    private void RefreshRoomStats()
    {
        var rooms = GetRoomsInScene();
        _stats = new RoomStats();
        foreach (var room in rooms)
        {
            _stats.LeftConnectors += room.HasConnectorToDirection(Direction.Left) ? 1 : 0;
            _stats.RightConnectors += room.HasConnectorToDirection(Direction.Right) ? 1 : 0;
            _stats.TopConnectors += room.HasConnectorToDirection(Direction.Up) ? 1 : 0;
            _stats.BottomConnectors += room.HasConnectorToDirection(Direction.Down) ? 1 : 0;

            var connectorInfoList = room.GetConnectorInfo();
            foreach (var info in connectorInfoList)
            {
                switch (info.Direction) {
                    case Direction.Down:
                        _stats.BottomByCoord.SetOrIncrement(info.LocalPos);
                        break;
                    case Direction.Up:
                        _stats.TopByCoord.SetOrIncrement(info.LocalPos);
                        break;
                    case Direction.Left:
                        _stats.LeftByCoord.SetOrIncrement(info.LocalPos);
                        break;
                    case Direction.Right:
                        _stats.RightByCoord.SetOrIncrement(info.LocalPos);
                        break;
                }
            }
        }
    }

    private List<Room> GetRoomsInScene()
    {
        return GameObject.FindObjectsOfType<Room>().ToList();
    }
}
