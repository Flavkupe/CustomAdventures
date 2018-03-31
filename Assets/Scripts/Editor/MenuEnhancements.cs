using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class MenuEnhancements {

    [MenuItem("Assets/Utils/Create Sprite Map")]
    private static void CreateSpriteMap()
    {
        var selected = Selection.activeObject as Texture2D;
        if (selected != null && Selection.objects != null)
        {
            var allObjects = Selection.objects.ToList().OfType<Texture2D>().ToList();
            if (allObjects.Count > 0)
            {
                Debug.Log(string.Format("Creating map with {0} objects", allObjects.Count));
                var xMax = selected.width * 8;
                var yMax = selected.height * Mathf.CeilToInt(allObjects.Count / 8.0f);
                var xOffset = 0;
                var yOffset = 0;
                var texture = new Texture2D(xMax, yMax);
                foreach (var item in allObjects)
                {
                    // Debug.Log(string.Format("Writing at {0}, {1}", xOffset, yOffset));
                    var pixels = item.GetPixels32();
                    texture.SetPixels32(xOffset, yOffset, selected.width, selected.height, pixels);
                    xOffset += selected.width;
                    if (xOffset >= xMax)
                    {
                        yOffset += selected.height;
                        xOffset = 0;
                    }
                }

                var bytes = texture.EncodeToPNG();

                var assetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(selected));
                var filePath = assetPath + "/NewFile.png";
                Debug.Log(string.Format("Writing to {0}", filePath));
                File.WriteAllBytes(filePath, bytes);

                AssetDatabase.Refresh();
            }
        }
    }
}
