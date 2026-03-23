using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Xna.Framework;

namespace Monolith.IO
{
    /// <summary>
    /// Filetype enum.
    /// </summary>
    public enum FileFormat
    {
        Json,
        Binary
    }

    public class FileSaver
    {
        /// <summary>
        /// The location of the appdata
        /// </summary>
        public static readonly string ApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        /// <summary>
        /// Universal file saving function.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="fullPath"></param>
        /// <param name="format"></param>
        public static void SaveData(object info, string fullPath, FileFormat format)
        {
            switch (format)
            {
                case FileFormat.Json:
                    SaveDataToJson(info, fullPath);
                    break;
                case FileFormat.Binary:
                    SaveDataToBinary(info, fullPath);
                    break;
            }
        }

        /// <summary>
        /// Universal file loading function.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fullPath"></param>
        /// <param name="format"></param>
        public static void LoadData(object target, string fullPath, FileFormat format)
        {
            switch (format)
            {
                case FileFormat.Json:
                    LoadDataFromJson(target, fullPath);
                    break;
                case FileFormat.Binary:
                    LoadDataFromBinary(target, fullPath);
                    break;
            }
        }

        /// <summary>
        /// Saves a JSON file containing the data of a class
        /// </summary>
        public static void SaveDataToJson(object info, string fullPath, bool compactFormat = false)
        {
            var fields = info.GetType()
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Where(f =>
                {
                    var value = f.GetValue(info);
                    if (value == null) return false;

                    var t = f.FieldType;
                    return t.IsPrimitive || t == typeof(string) || t == typeof(Vector2);
                })
                .ToDictionary(f => f.Name, f =>
                {
                    var value = f.GetValue(info);

                    if (f.FieldType == typeof(Vector2))
                    {
                        var vec = (Vector2)value;
                        return new { X = vec.X, Y = vec.Y };
                    }

                    return value;
                });

            var options = new JsonSerializerOptions
            {
                WriteIndented = !compactFormat
            };

            string fileName = Path.GetFileName(fullPath);
            if (string.IsNullOrEmpty(fileName))
                fileName = "data.json";

            string saveDirectory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(saveDirectory))
                System.IO.Directory.CreateDirectory(saveDirectory);

            string json = JsonSerializer.Serialize(fields, options);
            File.WriteAllText(fullPath, json);
        }

        /// <summary>
        /// Loads the data from a file and sets the file's value to the class
        /// </summary>
        public static void LoadDataFromJson(object target, string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found at: {filePath}");
                return;
            }

            var json = File.ReadAllText(filePath);

            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            if (dict == null) return;

            var fields = target.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (!dict.ContainsKey(field.Name)) continue;

                var jsonValue = dict[field.Name];

                try
                {
                    if (field.FieldType == typeof(int)) field.SetValue(target, jsonValue.GetInt32());
                    else if (field.FieldType == typeof(float)) field.SetValue(target, jsonValue.GetSingle());
                    else if (field.FieldType == typeof(double)) field.SetValue(target, jsonValue.GetDouble());
                    else if (field.FieldType == typeof(bool)) field.SetValue(target, jsonValue.GetBoolean());
                    else if (field.FieldType == typeof(string)) field.SetValue(target, jsonValue.GetString());
                    else if (field.FieldType == typeof(Vector2))
                    {
                        var x = jsonValue.GetProperty("X").GetSingle();
                        var y = jsonValue.GetProperty("Y").GetSingle();
                        field.SetValue(target, new Vector2(x, y));
                    }
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine($"Failed to load field {field.Name}: {ex.Message}");
                }
            }
        }

        public static void SaveDataToBinary(object info, string fullPath)
        {
            var fields = info.GetType()
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Where(f =>
                {
                    var value = f.GetValue(info);
                    if (value == null) return false;

                    var t = f.FieldType;
                    return t.IsPrimitive || t == typeof(string) || t == typeof(Vector2);
                })
                .ToList();

            string directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            using var bw = new BinaryWriter(File.Open(fullPath, FileMode.Create));

            bw.Write(fields.Count);

            foreach (var field in fields)
            {
                bw.Write(field.Name);

                var value = field.GetValue(info);

                if (field.FieldType == typeof(int)) bw.Write((int)value);
                else if (field.FieldType == typeof(float)) bw.Write((float)value);
                else if (field.FieldType == typeof(double)) bw.Write((double)value);
                else if (field.FieldType == typeof(bool)) bw.Write((bool)value);
                else if (field.FieldType == typeof(string)) bw.Write((string)value ?? "");
                else if (field.FieldType == typeof(Vector2))
                {
                    Vector2 v = (Vector2)value;
                    bw.Write(v.X);
                    bw.Write(v.Y);
                }
            }
        }

        public static void LoadDataFromBinary(object target, string fullPath)
        {
            if (!File.Exists(fullPath)) return;

            using var br = new BinaryReader(File.Open(fullPath, FileMode.Open));

            int fieldCount = br.ReadInt32();

            var fields = target.GetType()
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .ToDictionary(f => f.Name, f => f);

            for (int i = 0; i < fieldCount; i++)
            {
                string name = br.ReadString();

                if (!fields.TryGetValue(name, out var field))
                    continue;

                if (field.FieldType == typeof(int)) field.SetValue(target, br.ReadInt32());
                else if (field.FieldType == typeof(float)) field.SetValue(target, br.ReadSingle());
                else if (field.FieldType == typeof(double)) field.SetValue(target, br.ReadDouble());
                else if (field.FieldType == typeof(bool)) field.SetValue(target, br.ReadBoolean());
                else if (field.FieldType == typeof(string)) field.SetValue(target, br.ReadString());
                else if (field.FieldType == typeof(Vector2))
                {
                    float x = br.ReadSingle();
                    float y = br.ReadSingle();
                    field.SetValue(target, new Vector2(x, y));
                }
            }
        }
    }
}
