create function [ext].[Regex_Match]
(
	@handle bigint,
	@input nvarchar (max)
)
returns nvarchar(max)
as external name [RegexWrapper].[__Regex].[Match]