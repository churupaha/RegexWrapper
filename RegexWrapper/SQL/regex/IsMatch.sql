create function [regex].[IsMatch]
(
	@handle bigint,
	@input nvarchar (max)
)
returns bit
as external name [RegexWrapper_v2].[__Regex].[IsMatch]