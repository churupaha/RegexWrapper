create procedure regex.Free
(
	@handle bigint
)
as external name [RegexWrapper_v2].[__Regex].[Free]
