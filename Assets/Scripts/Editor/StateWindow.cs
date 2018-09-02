using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    public class StateWindow : EditorWindow
    {
        [MenuItem("Window/Utils/State Monitor")]

        public static void ShowWindow()
        {
            GetWindow(typeof(StateWindow));
        }

        void OnGUI()
        {
            if (Game.Dungeon == null)
            {
                return;
            }

            var context = Game.Dungeon.GetGameContext();
            var blueStyle = new GUIStyle(EditorStyles.label);
            blueStyle.normal.textColor = Color.blue;

            EditorGUILayout.LabelField("Actions", blueStyle);
            foreach (var val in Enum.GetValues(typeof(DungeonActionType)))
            {
                EditorGUILayout.LabelField(Enum.GetName(typeof(DungeonActionType), val) + ": ", context.CanPerformAction((DungeonActionType)val).ToString());
            }

            foreach (var controller in Game.Dungeon.GetStateControllers())
            {
                EditorGUILayout.LabelField(controller.Name + ": ", controller.StateName);
            }
        }

        void OnInspectorUpdate()
        {
            if (Game.Dungeon == null)
            {
                return;
            }

            Repaint();
        }
    }
}
