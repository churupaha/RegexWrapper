create procedure ext.Regex_Free
(
	@handle bigint
)
as external name [RegexWrapper].[__Regex].[Free]
