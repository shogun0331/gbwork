using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;

public static class DefineSymbolManager
{
    public struct DefineSymbolData
    {
        public BuildTargetGroup buildTargetGroup;
        public string fullSymbolString;          
        public Regex symbolRegex;

        public DefineSymbolData(string symbol)
        {
            buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            fullSymbolString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            symbolRegex = new Regex(@"\b" + symbol + @"\b(;|$)");
        }
    }

    public static bool IsSymbolAlreadyDefined(string symbol)
    {
        DefineSymbolData dsd = new DefineSymbolData(symbol);

        return dsd.symbolRegex.IsMatch(dsd.fullSymbolString);
    }

    public static bool IsSymbolAlreadyDefined(string symbol, out DefineSymbolData dsd)
    {
        dsd = new DefineSymbolData(symbol);

        return dsd.symbolRegex.IsMatch(dsd.fullSymbolString);
    }

    public static void AddDefineSymbol(string symbol)
    {
        if (!IsSymbolAlreadyDefined(symbol, out var dsd))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(dsd.buildTargetGroup, $"{dsd.fullSymbolString};{symbol}");
        }
    }

    public static void RemoveDefineSymbol(string symbol)
    {
        
        if (IsSymbolAlreadyDefined(symbol, out var dsd))
        {
            string strResult = dsd.symbolRegex.Replace(dsd.fullSymbolString, "");

            PlayerSettings.SetScriptingDefineSymbolsForGroup(dsd.buildTargetGroup, strResult);
        }
    }
}
#endif