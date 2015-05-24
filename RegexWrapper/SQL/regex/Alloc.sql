create procedure regex.Alloc
(
	@pattern nvarchar (4000), 
	@options nvarchar (4000), 
	@handle bigint output
)
as external name [RegexWrapper_v2].[__Regex].[Alloc]
