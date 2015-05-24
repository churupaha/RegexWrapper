create function [regex].[Match]
(
	@handle bigint,
	@input nvarchar (max)
)
returns nvarchar(max)
as external name [RegexWrapper_v2].[__Regex].[Match]