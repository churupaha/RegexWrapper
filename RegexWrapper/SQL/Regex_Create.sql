create procedure ext.Regex_Create
(
	@pattern nvarchar (4000), 
	@options nvarchar (4000), 
	@handle bigint output
)
as external name [RegexWrapper].[__Regex].[Create]
