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
using System.Runtime.InteropServices;
using System.Security.Permissions;

public partial class __Regex
{
    #region Обертка для Split
    
    [SqlFunction(
        IsDeterministic = true,
        IsPrecise = true,
        FillRowMethodName = "__Regex_SplitFill", 
        TableDefinition = "[part] nvarchar(max)")]
    public static IEnumerable Split(SqlInt64 handle, [SqlFacet(MaxSize = -1)] SqlString input)
    {
        if (handle.IsNull)
            throw new ArgumentException("Invalid handle");
        
        if (input.IsNull)
            return new string[] { };
        
        Regex r = (Regex)GCHandle.FromIntPtr(new IntPtr(handle.Value)).Target;

        return r.Split(input.Value);
    }

    public static void __Regex_SplitFill(
        object obj, 
        [SqlFacet(MaxSize = -1)] out SqlString part)
    {
        part = new SqlString((string)obj);
    }

    #endregion Обертка для Split

    #region Обертка для Matches

    [SqlFunction(
        IsDeterministic = true,
        IsPrecise = true, 
        FillRowMethodName = "__Regex_MatchesFill",
        TableDefinition = "[match] int, [group_name] nvarchar(4000), [group] int, [capture] int, [position] int, [length] int, [value] nvarchar(max)")]
    public static IEnumerable Matches(SqlInt64 handle, [SqlFacet(MaxSize = -1)] SqlString input)
    {
        if (handle.IsNull)
            throw new ArgumentException("Invalid handle");
        
        if (input.IsNull)
            yield break;

        Regex r = (Regex)GCHandle.FromIntPtr(new IntPtr(handle.Value)).Target;

        int mNum = 0;
        foreach (Match m in r.Matches(input.Value))
        {
            int gNum = 0;
            foreach (Group g in m.Groups)
            {
                string gName = r.GroupNameFromNumber(gNum);
                
                int cNum = 0;
                foreach (Capture c in g.Captures)
                {

                    yield return new
                        __Match()
                        {
                            
                            Match = mNum,
                            GroupName = gName,
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
        var m = (__Match)obj;

        match = m.Match;
        groupName = m.GroupName;
        group = m.Group;
        capture = m.Capture;
        position = m.Position;
        length = m.Length;
        value = m.Value;
    }

    #endregion Обертка для Matches

    #region Обертка для Match

    [return: SqlFacet(MaxSize = -1)]
    [SqlFunction(IsDeterministic = true, IsPrecise = true)]
    public static SqlString Match(SqlInt64 handle, [SqlFacet(MaxSize = -1)] SqlString input)
    {
        if (handle.IsNull)
            throw new ArgumentException("Invalid handle");
        
        if (input.IsNull)
            return String.Empty;

        Regex r = (Regex)GCHandle.FromIntPtr(new IntPtr(handle.Value)).Target;

        return r.Match(input.Value).Value;
    }

    #endregion Обертка для Match
    
    #region Обертка для IsMatch
    
    [SqlFunction(IsDeterministic = true, IsPrecise = true)]
    public static SqlBoolean IsMatch(SqlInt64 handle, [SqlFacet(MaxSize = -1)] SqlString input)
    {
        if (handle.IsNull)
            throw new ArgumentException("Invalid handle");
        
        if (input.IsNull)
            return false;

        Regex r = (Regex)GCHandle.FromIntPtr(new IntPtr(handle.Value)).Target;

        return r.IsMatch(input.Value);
    }

    #endregion Обертка для IsMatch

    #region Обертка для Replace

    [return: SqlFacet(MaxSize = -1)]
    [SqlFunction(IsDeterministic = true, IsPrecise = true)]
    public static SqlString Replace(
        SqlInt64 handle,
        [SqlFacet(MaxSize = -1)] SqlString input, 
        [SqlFacet(MaxSize = -1)] SqlString replacement)
    {
        if (handle.IsNull)
            throw new ArgumentException("Invalid handle"); 
        
        if (replacement.IsNull)
            throw new ArgumentException("Parameter [replacement] can not be NULL");

        if (input.IsNull)
            return new SqlString(null);

        Regex r = (Regex)GCHandle.FromIntPtr(new IntPtr(handle.Value)).Target;

        return r.Replace
        (
            input.Value, 
            replacement.Value
        );
    }
    
    #endregion Обертка для Replace

    #region Create

    [SqlProcedure]
    public static void Create(SqlString pattern, SqlString options, out SqlInt64 handle)
    {
        if (pattern.IsNull)
            throw new ArgumentException("Parameter [pattern] can not be NULL");

        Regex r = new Regex(pattern.Value, __ParseRegexOptions(options));

        handle = GCHandle.ToIntPtr(GCHandle.Alloc(r, GCHandleType.Normal)).ToInt64();
    }

    #endregion Create

    #region Free

    [SqlProcedure]
    public static void Free(SqlInt64 handle)
    {
        if (handle.IsNull)
            throw new ArgumentException("Invalid handle");

        GCHandle.FromIntPtr(new IntPtr(handle.Value)).Free();
    }

    #endregion Free

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

    private class __Match
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
