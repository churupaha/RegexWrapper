create function [ext].[Regex_IsMatch]
(
	@handle bigint,
	@input nvarchar (max)
)
returns bit
as external name [RegexWrapper].[__Regex].[IsMatch]