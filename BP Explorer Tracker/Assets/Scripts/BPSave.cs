using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public class SaveSlotField
{
    public string FieldName;
    public string Type;
    public object Value;
}

public class SaveSlotArray
{
    public string ArrayName;
    public List<SaveSlotArrayEntry> Data = new List<SaveSlotArrayEntry>();

    public List<int> GetIntArray()
    {
        List<int> intArray = new List<int>();
        foreach(SaveSlotArrayEntry entry in Data)
        {
            if (entry.Type == "System.Int32")
            {
                intArray.Add((int)entry.Value);
            }
        }

        return intArray;
    }

    public List<string> GetStringArray()
    {
        List<string> stringArray = new List<string>();
        foreach(SaveSlotArrayEntry entry in Data)
        {
            if (entry.Type == "System.String")
            {
                stringArray.Add((string)entry.Value);
            }
        }

        return stringArray;
    }
}

public class SaveSlotArrayEntry
{
    public string Type;
    public object Value;
}

public class SaveSlot
{
    public string SlotName;

    public List<SaveSlotField> Objects = new List<SaveSlotField>();

    public List<SaveSlotArray> Arrays = new List<SaveSlotArray>();

    public SaveSlotField GetSaveField(string fieldName)
    {
        SaveSlotField field = Objects.Find(s => s.FieldName == fieldName);
        return field;
    }

    public SaveSlotArray GetSaveArray(string arrayName)
    {
        SaveSlotArray array = Arrays.Find(s => s.ArrayName == arrayName);
        return array;
    }
}

public class BPSave
{
    public List<SaveSlot> SaveSlots = new List<SaveSlot>();

    public SaveSlot GetSaveSlot(string slotName)
    {
        SaveSlot slot = SaveSlots.Find(s => s.SlotName == slotName);
        return slot;
    }
}


public static class BPSaveParser
{
    public static BPSave Parse(string saveData)
    {
        if (string.IsNullOrEmpty(saveData))
        {
            throw new ArgumentException("Save data is empty.");
        }

        // The save data format is not normal Json, but I want it in normal Json so it can be parsed much more easily
        saveData = NormalizeSaveData(saveData);

        JsonSaveParser parser = new JsonSaveParser(saveData);

        Dictionary<string, object> root = parser.ParseObject();

        BPSave result = new BPSave();

        foreach (KeyValuePair<string, object> slotPair in root)
        {
            Dictionary<string, object> slotData =
                slotPair.Value as Dictionary<string, object>;

            if (slotData == null)
                continue;

            Dictionary<string, object> valueData =
                GetDictionary(slotData, "value");

            if (valueData == null)
                continue;

            SaveSlot slot = new SaveSlot();
            slot.SlotName = slotPair.Key;

            // Getting all the objects first
            Dictionary<string, object> objects =
                GetDictionary(valueData, "objs");

            if (objects != null)
            {
                foreach (KeyValuePair<string, object> fieldPair in objects)
                {
                    Dictionary<string, object> fieldData =
                        fieldPair.Value as Dictionary<string, object>;

                    if (fieldData == null)
                        continue;

                    SaveSlotField field =
                        ParseField(fieldPair.Key, fieldData);

                    if (field != null)
                        slot.Objects.Add(field);
                }
            }

            // Getting all the arrays next
            Dictionary<string, object> arrays = GetDictionary(valueData, "arrays");
            if (arrays != null)
            {
                foreach (KeyValuePair<string, object> arrayPair in arrays)
                {
                    List<object> arrayData = arrayPair.Value as List<object>;

                    if (arrayData == null)
                        continue;

                    SaveSlotArray saveArray = new SaveSlotArray();
                    saveArray.ArrayName = arrayPair.Key;

                    foreach (object entryObject in arrayData)
                    {
                        Dictionary<string, object> entry = entryObject as Dictionary<string, object>;

                        if (entry == null)
                            continue;

                        SaveSlotArrayEntry parsedEntry = ParseArrayEntry(entry);

                        if (parsedEntry != null)
                        {
                            saveArray.Data.Add(parsedEntry);
                        }
                    }

                    slot.Arrays.Add(saveArray);
                }
            }

            result.SaveSlots.Add(slot);
        }

        return result;
    }

    private static SaveSlotField ParseField(string fieldName, Dictionary<string, object> data)
    {
        string type = GetString(data, "__type");

        if (string.IsNullOrEmpty(type))
        {
            return null;
        }

        object value = ConvertValue(type, data);

        return new SaveSlotField
        {
            FieldName = fieldName,
            Type = type,
            Value = value
        };
    }

    private static SaveSlotArrayEntry ParseArrayEntry(Dictionary<string, object> data)
    {
        string type = GetString(data, "__type");

        if (string.IsNullOrEmpty(type))
        {
            return null;
        }

        object value = ConvertValue(type, data);

        return new SaveSlotArrayEntry
        {
            Type = type,
            Value = value
        };
    }

    private static object ConvertValue(string type, Dictionary<string, object> data)
    {
        object value;
        bool gotValue = false;
        if (data.TryGetValue("value", out value))
        {
            gotValue = true;
        }

        switch (type)
        {
            case "System.Int32":
                return gotValue ? Convert.ToInt32(value) : 0;

            case "System.Int64":
                return gotValue ? Convert.ToInt64(value) : 0;

            case "System.Single":
                return gotValue ? Convert.ToSingle(value) : 0;

            case "System.Double":
                return gotValue ? Convert.ToDouble(value) : 0;

            case "System.Boolean":
                return gotValue ? Convert.ToBoolean(value) : false;

            case "System.String":
                return gotValue ? value as string : "";

            case "UnityEngine.Vector3,UnityEngine.CoreModule":
                return new Vector3(
                    GetFloat(data, "x"),
                    GetFloat(data, "y"),
                    GetFloat(data, "z")
                );

            default:
                Debug.LogWarning(
                    "Unknown save data type: " + type
                );

                // Preserve unknown values if possible.
                if (data.ContainsKey("value"))
                    return data["value"];

                return null;
        }
    }


    // Dictionary helpers
    private static Dictionary<string, object> GetDictionary(Dictionary<string, object> dictionary, string key)
    {
        object value;

        if (!dictionary.TryGetValue(key, out value))
            return null;

        return value as Dictionary<string, object>;
    }

    private static string GetString(Dictionary<string, object> dictionary, string key)
    {
        object value;

        if (!dictionary.TryGetValue(key, out value))
            return null;

        return value as string;
    }

    private static float GetFloat(
        Dictionary<string, object> dictionary,
        string key)
    {
        object value;

        if (!dictionary.TryGetValue(key, out value))
            return 0f;

        if (value is float)
            return (float)value;

        return Convert.ToSingle(value, CultureInfo.InvariantCulture);
    }


    // Save format normalization to JSON
    private static string NormalizeSaveData(string input)
    {
        // The game's save format is weird, with an entry looking something like this:
        //      "__type" : "System.Single"156699.016
        // IDK what format this is supposed to be, but normal Json formatting is more like:
        //      "__type" : "System.Single",
        //      "value" : 156699.016
        // Just to make it easier for me to parse I'm gonna convert things to json format.

        string[] numericTypes =
        {
            "System.Single",
            "System.Double",
            "System.Int32",
            "System.Int64"
        };

        // Number values
        foreach (string type in numericTypes)
        {
            string escapedType = Regex.Escape(type);

            input = Regex.Replace(
                input,
                "(\"__type\"\\s*:\\s*\"" + escapedType + "\")\\s*" + @"(-?\d+(?:\.\d+)?(?:[eE][+-]?\d+)?)",
                "$1,\"value\":$2"
            );
        }

        // Boolean values
        input = Regex.Replace(
            input,
            @"(""__type""\s*:\s*""System\.Boolean"")\s*(true|false)",
            "$1,\"value\":$2"
        );

        // string values
        input = Regex.Replace(
            input,
            @"(""__type""\s*:\s*""System\.String"")\s*""([^""]*)""",
            "$1,\"value\":\"$2\""
        );

        // Remove duplicate commas.
        input = Regex.Replace(
            input,
            @",\s*,+",
            ","
        );

        return input;
    }
}


// This is a class that will return a big nested tree of objects, with the top part being mapping 
// Because trying to use Unity's JsonUtility parsing was giving me a headache and this code works just fine.
public class JsonSaveParser
{
    private readonly string input;
    private int position;

    public JsonSaveParser(string input)
    {
        this.input = input;
        position = 0;
    }

    public Dictionary<string, object> ParseObject()
    {
        SkipWhitespace();

        Expect('{');

        Dictionary<string, object> result = new Dictionary<string, object>();

        SkipWhitespace();

        while (!EndOfInput() && Peek() != '}')
        {
            string key = ParseString();

            SkipWhitespace();
            Expect(':');
            SkipWhitespace();

            object value = ParseValue();
            result[key] = value;

            SkipWhitespace();

            if (Peek() == ',')
            {
                position++;
                SkipWhitespace();
            }
            else
            {
                break;
            }
        }

        Expect('}');

        return result;
    }

    private object ParseValue()
    {
        SkipWhitespace();

        if (EndOfInput())
        {
            throw Error("Unexpected end of input.");
        }

        char c = Peek();

        if (c == '{')
        {
            return ParseObject();
        }

        if (c == '[')
        {
            return ParseArray();
        }

        if (c == '"')
        {
            return ParseString();
        }

        if (c == 't' || c == 'f')
        {
            return ParseBoolean();
        }

        if (c == 'n')
        {
            ParseNull();
            return null;
        }

        if (c == '-' || char.IsDigit(c))
        {
            return ParseNumber();
        }

        throw Error(
            "Unexpected character '" + c + "'."
        );
    }

    private List<object> ParseArray()
    {
        Expect('[');

        List<object> result = new List<object>();

        SkipWhitespace();

        while (!EndOfInput() && Peek() != ']')
        {
            object value = ParseValue();

            result.Add(value);

            SkipWhitespace();

            if (Peek() == ',')
            {
                position++;
                SkipWhitespace();
            }
            else
            {
                break;
            }
        }

        Expect(']');

        return result;
    }

    private string ParseString()
    {
        Expect('"');

        StringBuilder result = new StringBuilder();

        while (!EndOfInput())
        {
            char c = input[position++];

            if (c == '"')
            {
                return result.ToString();
            }

            if (c == '\\')
            {
                if (EndOfInput())
                    break;

                char escaped = input[position++];

                switch (escaped)
                {
                    case '"':
                        result.Append('"');
                        break;

                    case '\\':
                        result.Append('\\');
                        break;

                    case '/':
                        result.Append('/');
                        break;

                    case 'b':
                        result.Append('\b');
                        break;

                    case 'f':
                        result.Append('\f');
                        break;

                    case 'n':
                        result.Append('\n');
                        break;

                    case 'r':
                        result.Append('\r');
                        break;

                    case 't':
                        result.Append('\t');
                        break;

                    case 'u':
                        result.Append(ParseUnicodeEscape());
                        break;

                    default:
                        result.Append(escaped);
                        break;
                }
            }
            else
            {
                result.Append(c);
            }
        }

        throw Error("Unterminated string.");
    }


    private char ParseUnicodeEscape()
    {
        if (position + 4 > input.Length)
        {
            throw Error("Invalid Unicode escape.");
        }

        string hex = input.Substring(position, 4);

        position += 4;

        return (char)Convert.ToInt32(hex, 16);
    }


    // ============================================================
    // Number
    // ============================================================

    private object ParseNumber()
    {
        int start = position;

        if (Peek() == '-')
        {
            position++;
        }

        while (!EndOfInput() && char.IsDigit(Peek()))
        {
            position++;
        }

        bool isFloat = false;

        if (!EndOfInput() && Peek() == '.')
        {
            isFloat = true;
            position++;

            while (!EndOfInput() && char.IsDigit(Peek()))
            {
                position++;
            }
        }

        if (!EndOfInput() && (Peek() == 'e' || Peek() == 'E'))
        {
            isFloat = true;
            position++;

            if (!EndOfInput() && (Peek() == '+' || Peek() == '-'))
            {
                position++;
            }

            while (!EndOfInput() && char.IsDigit(Peek()))
            {
                position++;
            }
        }

        string number = input.Substring(start, position - start);

        if (isFloat)
        {
            return float.Parse(number, CultureInfo.InvariantCulture);
        }

        return int.Parse(number, CultureInfo.InvariantCulture);
    }

    private bool ParseBoolean()
    {
        if (input.Substring(position, Math.Min(4, input.Length - position)) == "true")
        {
            position += 4;
            return true;
        }

        if (input.Substring(position, Math.Min(5, input.Length - position)) == "false")
        {
            position += 5;
            return false;
        }

        throw Error("Invalid boolean.");
    }


    private void ParseNull()
    {
        if (position + 4 > input.Length || input.Substring(position, 4) != "null")
        {
            throw Error("Invalid null.");
        }

        position += 4;
    }

    // Skip any whitespace and go on to the next bit of text
    private void SkipWhitespace()
    {
        while (!EndOfInput() && char.IsWhiteSpace(input[position]))
        {
            position++;
        }
    }

    // Quick way to see what the current character we're on is
    private char Peek()
    {
        if (EndOfInput())
        {
            return '\0';
        }

        return input[position];
    }

    // Quick way to check if the next character is one we expect it to be, and if so, skip it.
    // This is mostly to skip over characters like quotes and commas
    private void Expect(char expected)
    {
        SkipWhitespace();

        if (EndOfInput() || input[position] != expected)
        {
            throw Error("Expected '" + expected + "' but found '" + Peek() + "'.");
        }

        position++;
    }

    private bool EndOfInput()
    {
        return position >= input.Length;
    }

    private Exception Error(string message)
    {
        return new Exception(message + " Position: " + position);
    }
}