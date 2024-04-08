using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PerlinNoiseTest))]
public class PerlinNoiseTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector options
        DrawDefaultInspector();

        PerlinNoiseTest script = (PerlinNoiseTest)target;

        // Add a button to the inspector for generating a random texture
        if (GUILayout.Button("Generate Random Texture"))
        {
            script.GenerateRandomTexture();
        }

        if (GUILayout.Button("Save Texture"))
        {
            script.SaveTextureAsPNG();
        }
    }
}
