﻿create function [ext].[Regex_Matches]
(
	@handle bigint,
	@input nvarchar (max)
)
returns 
     table 
	 (
        [match]         int                 null,
        [group_name]    nvarchar (4000)     null,
        [group]         int                 null,
        [capture]       int                 null,
        [position]      int                 null,
        [length]        int                 null,
        [value]         nvarchar (max)      null
	)
as external name [RegexWrapper].[__Regex].[Matches]
