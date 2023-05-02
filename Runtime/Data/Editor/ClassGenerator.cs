using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace DarkNaku.Foundation
{
    public class ClassGenerator 
    {
        public static void Generate(Dictionary<string, List<FieldInfo>> schemas)
        {
            var root = ClassConstants.ROOT_CLASS_FORMAT;

            var classes = GenerateClasses(schemas);
            var fields = GetFields(classes);

            root = root.Replace("[FIELD]", fields);

            File.WriteAllText(Application.dataPath + "/DB.cs", root);

            AssetDatabase.Refresh();
        }

        private static string GetFields(Dictionary<string, string> classes)
        {
            var index = 0;
            var sb = new StringBuilder();

            foreach (var keyValue in classes)
            {
                sb.Append("".PadLeft(ClassConstants.IndentLevel2));
                sb.AppendFormat("[ProtoMember({0})] public List<{1}> {2} {{ get; private set; }}\n", index, keyValue.Value, keyValue.Key);
                
                index++;
            }

            return sb.ToString();
        }

        private static Dictionary<string, string> GenerateClasses(Dictionary<string, List<FieldInfo>> schemas)
        {
            var classes = new Dictionary<string, string>();

            foreach (KeyValuePair<string, List<FieldInfo>> keyValue in schemas)
            {
                classes.Add(keyValue.Key, GenerateClass(keyValue.Key, keyValue.Value));
            }

            return classes;
        }

        private static string GenerateClass(string key, List<FieldInfo> fieldInfos)
        {
            var sb = new StringBuilder();
            var className = string.Format(ClassConstants.DATA_CLASS_NAME_FORMAT, key);
            var fileName = string.Format(ClassConstants.CLASS_FILE_NAME_FORMAT, className);

            // Add the auto generated comment at the top of the file
            sb.Append(ClassConstants.COMMENT);
            sb.Append("\n");

            // Append all the using statements
            sb.Append(ClassConstants.HEADER);
            sb.Append("\n");

            // Append the class declaration
            sb.Append("".PadLeft(ClassConstants.IndentLevel1));
            sb.AppendFormat(ClassConstants.CLASS_DECLARATION_FORMAT, className);
            sb.Append("\n");
            sb.Append("{".PadLeft(ClassConstants.IndentLevel1 + 1));
            sb.Append("\n");

            GenerateFields(ref sb, ref fieldInfos);

            // Append the close class brace
            sb.Append("}".PadLeft(ClassConstants.IndentLevel1 + 1));
            sb.Append("\n");

            // Append the close namespace brace
            sb.Append("}");
            sb.Append("\n");

            File.WriteAllText(Application.dataPath + "/" + className + ".cs", sb.ToString());

            return className;
        }

        private static void GenerateFields(ref StringBuilder sb, ref List<FieldInfo> fieldInfos)
        {
            var fieldNames = new HashSet<string>();

            sb.Append("".PadLeft(ClassConstants.IndentLevel2));
            sb.Append("[ProtoMember(0)] public string GUID { get; private set; }\n");

            for (int i = 0; i < fieldInfos.Count; i++)
            {
                if (fieldNames.Contains(fieldInfos[i].Name))
                {
                    Debug.LogErrorFormat("[ClassGenerator] GenerateFields : Field's name is duplicated - {0}", fieldInfos[i].Name);
                    continue;
                }

                sb.Append("".PadLeft(ClassConstants.IndentLevel2));
                sb.AppendFormat("[ProtoMember({0})] public {1} {2} {{ get; private set; }}\n", i + 1, GetTypeString(fieldInfos[i]), fieldInfos[i].Name);

                fieldNames.Add(fieldInfos[i].Name);
            }
        }

        private static string GetTypeString(FieldInfo fieldInfo)
        {
            var format = fieldInfo.IsList ? "List<{0}>" : "{0}";
            
            switch (fieldInfo.FieldType)
            {
                case FIELD_TYPE.BOOL:
                    return string.Format(format, "bool");
                case FIELD_TYPE.DOUBLE:
                    return string.Format(format, "bouble");
                case FIELD_TYPE.FLOAT:
                    return string.Format(format, "float");
                case FIELD_TYPE.INT:
                    return string.Format(format, "int");
                case FIELD_TYPE.LONG:
                    return string.Format(format, "long");
                case FIELD_TYPE.STRING:
                    return string.Format(format, "string");
                case FIELD_TYPE.UINT:
                    return string.Format(format, "uint");
                case FIELD_TYPE.ULONG:
                    return string.Format(format, "ulong");
                case FIELD_TYPE.UNDEFINED:
                    return string.Format(format, fieldInfo.FieldTypeNmae);
                default:
                    return "";
            }
        }
    }
}