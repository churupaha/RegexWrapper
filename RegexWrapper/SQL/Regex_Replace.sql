create function [ext].[Regex_Replace]
(
	@handle bigint,
	@input nvarchar (max),
	@replacement nvarchar(max)
)
returns nvarchar (max)
as external name [RegexWrapper].[__Regex].[Replace]

