using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
{
    #region Обертка для Regex Split
    
    [SqlFunction(
        IsDeterministic = true,
        IsPrecise = true,
        FillRowMethodName = "__Regex_SplitFill", 
        TableDefinition = "[part] nvarchar(max)")]
    public static IEnumerable Regex_Split(
        [SqlFacet(MaxSize = -1)] SqlString input,
        SqlString pattern,
        SqlString options)
    {
        if (pattern.IsNull)
            throw new ArgumentException("Parameter [pattern] can not be NULL");

        if (input.IsNull)
            return new string[] { };

        return 
            (IEnumerable)Regex.Split
            (
                input.Value, 
                pattern.Value,
                __ParseRegexOptions(options)
            );
    }

    public static void __Regex_SplitFill(
        object obj, 
        [SqlFacet(MaxSize = -1)] out SqlString part)
    {
        part = new SqlString((string)obj);
    }

    #endregion Обертка для Regex Split

    #region Обертка для Regex Matches

    [SqlFunction(
        IsDeterministic = true,
        IsPrecise = true, 
        FillRowMethodName = "__Regex_MatchesFill",
        TableDefinition = "[match] int, [group_name] nvarchar(4000), [group] int, [capture] int, [position] int, [length] int, [value] nvarchar(max)")]
    public static IEnumerable Regex_Matches(
        [SqlFacet(MaxSize = -1)] SqlString input, 
        SqlString pattern, 
        SqlString options)
    {
        if (pattern.IsNull)
            throw new ArgumentException("Parameter [pattern] can not be NULL");

        if (input.IsNull)
            yield break;

        int mNum = 0;
        foreach (Match m in Regex.Matches(input.Value, pattern.Value, __ParseRegexOptions(options)))
        {
            int gNum = 0;
            foreach (Group g in m.Groups)
            {
                int cNum = 0;
                foreach (Capture c in g.Captures)
                {

                    yield return new
                        __MatchWrapper()
                        {
                            
                            Match = mNum,
                            Group = gNum,
                            Capture = cNum,
                            Position = c.Index,
                            Length = c.Length,
                            Value = c.Value
                        };
                
                    cNum++;
                }
            
                gNum++;
            }
        
            mNum++;
        }
    }

    public static void __Regex_MatchesFill(
        object obj, out SqlInt32 match, out SqlString groupName, out SqlInt32 group,
        out SqlInt32 capture, out SqlInt32 position, out SqlInt32 length, 
        [SqlFacet(MaxSize = -1)] out SqlString value)
    {
        var m = (__MatchWrapper)obj;

        match = m.Match;
        groupName = m.GroupName;
        group = m.Group;
        capture = m.Capture;
        position = m.Position;
        length = m.Length;
        value = m.Value;
    }

    #endregion Обертка для Regex Matches

    #region Обертка для Regex Match

    [return: SqlFacet(MaxSize = -1)]
    [SqlFunction(IsDeterministic = true, IsPrecise = true)]
    public static SqlString Regex_Match(
        [SqlFacet(MaxSize = -1)] SqlString input,
        SqlString pattern,
        SqlString options)
    {
        if (pattern.IsNull)
            throw new ArgumentException("Parameter [pattern] can not be NULL");

        if (input.IsNull)
            return String.Empty;

        return
            Regex.Match
            (
                input.Value,
                pattern.Value,
                __ParseRegexOptions(options)
            ).Value;
    }

    #endregion Обертка для Regex IsMatch
    
    #region Обертка для Regex IsMatch
    
    [SqlFunction(IsDeterministic = true, IsPrecise = true)]
    public static SqlBoolean Regex_IsMatch(
        [SqlFacet(MaxSize = -1)] SqlString input, 
        SqlString pattern, 
        SqlString options)
    {
        if (pattern.IsNull)
            throw new ArgumentException("Parameter [pattern] can not be NULL");

        if (input.IsNull)
            return false;

        return
            Regex.IsMatch
            (
                input.Value, 
                pattern.Value,
                __ParseRegexOptions(options)
            );
    }

    #endregion Обертка для Regex IsMatch

    #region Обертка для Regex Replace

    [return: SqlFacet(MaxSize = -1)]
    [SqlFunction(IsDeterministic = true, IsPrecise = true)]
    public static SqlString Regex_Replace(
        [SqlFacet(MaxSize = -1)] SqlString input, 
        SqlString pattern,
        [SqlFacet(MaxSize = -1)] SqlString replacement, 
        SqlString options)
    {
        if (pattern.IsNull)
            throw new ArgumentException("Parameter [pattern] can not be NULL");

        if (replacement.IsNull)
            throw new ArgumentException("Parameter [replacement] can not be NULL");

        if (input.IsNull)
            return new SqlString(null);

        return
            Regex.Replace
            (
                input.Value, 
                pattern.Value, 
                replacement.Value,
                __ParseRegexOptions(options)
            );
    }
    
    #endregion Обертка для Regex Replace

    #region Хелперы

    private static RegexOptions __ParseRegexOptions(SqlString options)
    {
        return
            options.IsNull || string.IsNullOrEmpty(options.Value) || options.Value == "None"
                    ?
                RegexOptions.None 
                    : 
                Regex
                    .Split(options.Value, @"[\s\n]*\|[\s\n]*")
                    .ToList()
                    .Select(s => Enum.Parse(typeof(RegexOptions), s))
                    .Cast<RegexOptions>()
                    .Aggregate((a, n) => a | n);
    }

    private class __MatchWrapper
    {
        public int Match { get; set; }
        public string GroupName { get; set; }
        public int Group { get; set; }
        public int Capture { get; set; }
        public int Position { get; set; }
        public int Length { get; set; }
        public string Value { get; set; }
    }

    #endregion Хелперы

}
