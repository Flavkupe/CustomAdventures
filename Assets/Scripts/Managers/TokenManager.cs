using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class TokenManager : Singleton<TokenManager>
{
    Dictionary<string, Func<string>> mapping = new Dictionary<string, Func<string>>();

    public void SetToken(string key, Func<string> valueCallback)
    {
        if (!mapping.ContainsKey(key))
        {
            Log.Warning($"Key already exists: {key}");
        }

        mapping[$"%({key.ToLower()})"] = valueCallback;
    }

    public string ReplaceTokens(string input)
    {
        var final = input;
        var matches = Regex.Matches(input, "%\\((\\w+)\\)");
        foreach (var match in matches)
        {
            var token = match.ToString();
            var val = GetValue(token);
            final = final.Replace(token, val);
        }

        return final;
    }

    public string GetValue(string key)
    {
        key = key.ToLower();
        if (!mapping.ContainsKey(key))
        {
            Log.Warning($"Unknown key: {key}");
            return string.Empty;
        }

        try
        {
            return mapping[key]();
        }
        catch (Exception ex)
        {
            Log.Warning($"Failed to get key: {key}");
            Log.Error(ex);
            return string.Empty;
        }
    }
}

