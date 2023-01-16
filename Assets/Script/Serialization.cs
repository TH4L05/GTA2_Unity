/// <author>Thoams Krahl</author>

using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

namespace ProjectGTA2_Unity
{
    public class Serialization : MonoBehaviour
    {
        public static void SaveToFile(object saveObj, string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(fileName, FileMode.Create);

            bf.Serialize(stream, saveObj);
            stream.Close();
            Debug.Log($"<color=cyan>File {fileName} = Saved</color>");
        }

        public static object LoadFromFile(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(fileName, FileMode.Open);
            object result = bf.Deserialize(stream);
            stream.Close();
            Debug.Log($"<color=cyan>File {fileName} = Loaded</color>");

            return result;
        }

        public static byte[] LoadFromFileByteArray(string fileName)
        {
            byte[] data = File.ReadAllBytes(fileName);
            return data;
        }

        public static void SaveFileByteArray(string fileName, byte[] data)
        {
            File.WriteAllBytes(fileName, data);
        }

        public static bool FileExistenceCheck(string fileName)
        {
            bool fileExists = File.Exists(fileName);
            if (!fileExists) Debug.LogError($"FILE {fileName} DOES NOT EXIST");
            return fileExists;
        }

        public static void DeleteFile(string fileName)
        {
            File.Delete(fileName);
        }

        public static void SaveToFileText(string text, string filename)
        {
            File.WriteAllText(filename, text);
            Debug.Log($"<color=cyan>File {filename} = Saved</color>");
        }

        public static List<string> LoadFromFileTextByLine(string filename)
        {
            var content = new List<string>();

            foreach (var line in File.ReadAllLines(filename))
            {
                content.Add(line);
            }

            Debug.Log($"<color=cyan>File {filename} = Loaded</color>");
            return content;
        }

        public static void CheckDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public static string[] GetFilesFromDirectory(string path, string searchpattern)
        {
            return Directory.GetFiles(path, searchpattern);
        }
    }
}

