using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ProceduralTerrainMenu))]
public class TerrainEditorButtons : Editor
{
    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        ProceduralTerrainMenu myScript = (ProceduralTerrainMenu) target;
        
        if(GUILayout.Button("Reset"))
        {
            myScript.Reset();
        }

        if(GUILayout.Button("Generate"))
        {
            myScript.Reset();
            myScript.ApplyProcedural();
        }
    }
}