using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class MaterialConverter : EditorWindow
{
    [MenuItem("Tools/Convert Materials to URP")]
    public static void ConvertMaterials()
    {
        // Encuentra todos los materiales en el proyecto
        string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/POLYGON city pack" });
        
        int convertedCount = 0;
        
        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            
            if (material != null)
            {
                // Verifica si el material usa un shader Built-in
                if (material.shader.name.Contains("Standard") || 
                    material.shader.name.Contains("Legacy") ||
                    material.shader.name.Contains("Diffuse") ||
                    material.shader.name.Contains("Specular"))
                {
                    // Guarda las propiedades antes de cambiar el shader
                    Texture mainTexture = material.mainTexture;
                    Color color = material.HasProperty("_Color") ? material.color : Color.white;
                    
                    // Cambia al shader URP Lit
                    material.shader = Shader.Find("Universal Render Pipeline/Lit");
                    
                    // Restaura las propiedades
                    if (mainTexture != null)
                    {
                        material.mainTexture = mainTexture;
                    }
                    material.color = color;
                    
                    EditorUtility.SetDirty(material);
                    convertedCount++;
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log($"Conversión completada: {convertedCount} materiales convertidos a URP.");
    }
}
