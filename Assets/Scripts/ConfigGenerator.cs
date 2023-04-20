using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public static class ConfigGenerator
{
    private static string GenerateConfig() => JsonConvert.SerializeObject(
        ContentData.Default,
        Formatting.None,
        new SerializedValueConverter());

    [MenuItem("Tools/Read Generated Config")]
    public static void ReadGeneratedConfig()
    {
        var content = GenerateConfig();
        var obj = JsonConvert.DeserializeObject<ContentData>(content, new SerializedValueConverter());
        Debug.Log(obj);
    }

    [MenuItem("Tools/Generate Config")]
    public static void PrintGeneratedConfig()
    {
        var config = GenerateConfig();
        System.IO.File.WriteAllText("./config.json", config);
        Debug.Log(config);
    }
}