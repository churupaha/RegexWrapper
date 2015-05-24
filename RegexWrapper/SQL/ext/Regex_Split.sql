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
as external name [RegexWrapper_v2].[__Regex].[Regex_Split]
