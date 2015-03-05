create function [ext].[Regex_Split]
(
	@input nvarchar (max), 
	@pattern nvarchar (4000), 
	@options nvarchar (4000)
)
returns 
     table 
	 (
        [part] nvarchar (max) null
	)
as external name [RegexWrapper].[UserDefinedFunctions].[Regex_Split]
