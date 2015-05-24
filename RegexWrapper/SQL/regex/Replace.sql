create function [regex].[Replace]
(
	@handle bigint,
	@input nvarchar (max),
	@replacement nvarchar(max)
)
returns nvarchar (max)
as external name [RegexWrapper_v2].[__Regex].[Replace]

