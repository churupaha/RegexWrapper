create function [ext].[Regex_Matches]
(
	@input nvarchar (max), 
	@pattern nvarchar (4000), 
	@options nvarchar (4000)
)
returns 
     table 
	 (
        [match]    int            null,
        [group]    int            null,
        [capture]  int            null,
        [position] int            null,
        [length]   int            null,
        [value]    nvarchar (max) null
	)
as external name [RegexWrapper].[UserDefinedFunctions].[Regex_Matches]
