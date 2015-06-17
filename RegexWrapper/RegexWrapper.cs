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

public class __Regex
{
    #region ������� ��� static Regex Split

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

    #endregion ������� ��� static Regex Split

    #region ������� ��� static Regex Matches

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
                        __Match()
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
        var m = (__Match)obj;

        match = m.Match;
        groupName = m.GroupName;
        group = m.Group;
        capture = m.Capture;
        position = m.Position;
        length = m.Length;
        value = m.Value;
    }

    #endregion ������� ��� static Regex Matches

    #region ������� ��� static Regex Match

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

    #endregion ������� ��� static Regex Match

    #region ������� ��� static Regex IsMatch

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

    #endregion ������� ��� static Regex IsMatch

    #region ������� ��� static Regex Replace

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

    #endregion ������� ��� static Regex Replace

    #region ������� ��� Split
    
    [SqlFunction(
        IsDeterministic = true,
        IsPrecise = true,
        FillRowMethodName = "__SplitFill", 
        TableDefinition = "[part] nvarchar(max)")]
    public static IEnumerable Split(SqlInt64 handle, [SqlFacet(MaxSize = -1)] SqlString input)
    {
        if (handle.IsNull)
            throw new ArgumentException("Invalid handle");
        
        if (input.IsNull)
            return new string[] { };

        Regex r = (Regex)RegexFromHandle(ref handle);

        return r.Split(input.Value);
    }

    public static void __SplitFill(
        object obj, 
        [SqlFacet(MaxSize = -1)] out SqlString part)
    {
        part = new SqlString((string)obj);
    }

    #endregion ������� ��� Split

    #region ������� ��� Matches

    [SqlFunction(
        IsDeterministic = true,
        IsPrecise = true, 
        FillRowMethodName = "__MatchesFill",
        TableDefinition = "[match] int, [group_name] nvarchar(4000), [group] int, [capture] int, [position] int, [length] int, [value] nvarchar(max)")]
    public static IEnumerable Matches(SqlInt64 handle, [SqlFacet(MaxSize = -1)] SqlString input)
    {
        if (handle.IsNull)
            throw new ArgumentException("Invalid handle");
        
        if (input.IsNull)
            yield break;

        Regex r = (Regex)RegexFromHandle(ref handle);

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

    public static void __MatchesFill(
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

    #endregion ������� ��� Matches

    #region ������� ��� Match

    [return: SqlFacet(MaxSize = -1)]
    [SqlFunction(IsDeterministic = true, IsPrecise = true)]
    public static SqlString Match(SqlInt64 handle, [SqlFacet(MaxSize = -1)] SqlString input)
    {
        if (handle.IsNull)
            throw new ArgumentException("Invalid handle");
        
        if (input.IsNull)
            return String.Empty;

        Regex r = (Regex)RegexFromHandle(ref handle);

        return r.Match(input.Value).Value;
    }

    #endregion ������� ��� Match
    
    #region ������� ��� IsMatch
    
    [SqlFunction(IsDeterministic = true, IsPrecise = true)]
    public static SqlBoolean IsMatch(SqlInt64 handle, [SqlFacet(MaxSize = -1)] SqlString input)
    {
        if (handle.IsNull)
            throw new ArgumentException("Invalid handle");
        
        if (input.IsNull)
            return false;

        Regex r = (Regex)RegexFromHandle(ref handle);

        return r.IsMatch(input.Value);
    }

    #endregion ������� ��� IsMatch

    #region ������� ��� Replace

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

        Regex r = (Regex)RegexFromHandle(ref handle);

        return r.Replace
        (
            input.Value, 
            replacement.Value
        );
    }
    
    #endregion ������� ��� Replace

    #region Alloc

    [SqlProcedure]
    public static void Alloc(SqlString pattern, SqlString options, out SqlInt64 handle)
    {
        if (pattern.IsNull)
            throw new ArgumentException("Parameter [pattern] can not be NULL");

        Regex r = new Regex(pattern.Value, __ParseRegexOptions(options));

        handle = GCHandle.ToIntPtr(GCHandle.Alloc(r, GCHandleType.Normal)).ToInt64();
    }

    #endregion Alloc

    #region Free

    [SqlProcedure]
    public static void Free(SqlInt64 handle)
    {
        if (handle.IsNull)
            throw new ArgumentException("Invalid handle");

        GCHandleFromInt64(ref handle).Free();
    }

    #endregion Free

    #region �������

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

    private static GCHandle GCHandleFromInt64(ref SqlInt64 handle)
    {
        return GCHandle.FromIntPtr(new IntPtr(handle.Value));
    }

    private static Regex RegexFromHandle(ref SqlInt64 handle)
    {
        return (Regex)GCHandleFromInt64(ref handle).Target;
    }

    #endregion �������
}

