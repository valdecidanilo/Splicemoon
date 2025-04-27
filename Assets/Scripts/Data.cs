using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Models;
using UnityEngine;
using Logger = LenixSO.Logger.Logger;


public static class Data
{
    public static void SaveSplicemon(SpliceMon data, string filename)
    {
        var folderPath = Path.Combine(Application.persistentDataPath, "SpliceMonData");
    
        // Cria o diretório se não existir
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    
        var fullPath = Path.Combine(folderPath, filename + ".splice");
        Logger.Log(fullPath);
        // Serializa o objeto
        var formatter = new BinaryFormatter();
        var stream = new FileStream(fullPath, FileMode.Create);
        
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SpliceMon LoadSplicemon(string filename)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, "SpliceMonData", filename + ".splice");
    
        if (File.Exists(fullPath))
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(fullPath, FileMode.Open);
        
            var data = formatter.Deserialize(stream) as SpliceMon;
            stream.Close();
        
            var newMon = ScriptableObject.CreateInstance<SpliceMon>();
            if (data != null) data.ApplyTo(newMon);
            return newMon;
        }
    
        return null;
    }
}