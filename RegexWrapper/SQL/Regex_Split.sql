create function [ext].[Regex_Split]
(
	@handle bigint,
	@input nvarchar (max)
)
returns 
     table 
	 (
        [part] nvarchar (max) null
	)
as external name [RegexWrapper].[__Regex].[Split]
