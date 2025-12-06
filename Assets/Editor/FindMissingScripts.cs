using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FindMissingScripts : EditorWindow
{
    private static int missingCount = 0;
    private static List<string> missingObjects = new List<string>();

    [MenuItem("Tools/Find Missing Scripts")]
    public static void ShowWindow()
    {
        GetWindow<FindMissingScripts>("Find Missing Scripts");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in Scenes"))
        {
            FindInScenes();
        }

        if (GUILayout.Button("Find Missing Scripts in Prefabs"))
        {
            FindInPrefabs();
        }

        if (GUILayout.Button("Find Missing Scripts in All Assets"))
        {
            FindInAllAssets();
        }

        if (missingCount > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox($"Found {missingCount} missing script(s)!", MessageType.Warning);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Objects with missing scripts:", EditorStyles.boldLabel);
            
            foreach (string obj in missingObjects)
            {
                EditorGUILayout.LabelField(obj);
            }
        }
    }

    private static void FindInScenes()
    {
        missingCount = 0;
        missingObjects.Clear();
        
        string[] allScenes = AssetDatabase.FindAssets("t:Scene");
        foreach (string sceneGUID in allScenes)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            
            foreach (GameObject go in allObjects)
            {
                if (go.scene.name == null || go.scene.name == "") continue;
                
                Component[] components = go.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null)
                    {
                        missingCount++;
                        string path = GetGameObjectPath(go);
                        missingObjects.Add($"Scene: {go.scene.name} - GameObject: {path}");
                        Debug.LogWarning($"Missing script found on: {path} in scene {go.scene.name}", go);
                    }
                }
            }
        }
        
        Debug.Log($"Finished searching scenes. Found {missingCount} missing script(s).");
    }

    private static void FindInPrefabs()
    {
        missingCount = 0;
        missingObjects.Clear();
        
        string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab");
        foreach (string prefabGUID in allPrefabs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (prefab != null)
            {
                CheckGameObjectForMissingScripts(prefab, prefabPath);
            }
        }
        
        Debug.Log($"Finished searching prefabs. Found {missingCount} missing script(s).");
    }

    private static void FindInAllAssets()
    {
        missingCount = 0;
        missingObjects.Clear();
        
        FindInScenes();
        FindInPrefabs();
        
        Debug.Log($"Finished searching all assets. Total missing script(s): {missingCount}");
    }

    private static void CheckGameObjectForMissingScripts(GameObject go, string assetPath)
    {
        Component[] components = go.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                missingCount++;
                string path = GetGameObjectPath(go);
                missingObjects.Add($"Prefab: {assetPath} - GameObject: {path}");
                Debug.LogWarning($"Missing script found on: {path} in prefab {assetPath}", go);
            }
        }

        // Check children recursively
        foreach (Transform child in go.transform)
        {
            CheckGameObjectForMissingScripts(child.gameObject, assetPath);
        }
    }

    private static string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return path;
    }
}
