using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]  
public class MapGeneratorInspector : Editor
{
    private MapGenerator mapGenerator;

    private void OnEnable()
    {
        mapGenerator = (MapGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Generate New Map"))
            {
                mapGenerator.GenerateNewMap();
            }
            if (GUILayout.Button("Repair Map"))
            {
                mapGenerator.RepairMap();
            }
        }
    }
}
