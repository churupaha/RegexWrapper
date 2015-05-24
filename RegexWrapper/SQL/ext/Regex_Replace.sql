create function [ext].[Regex_Replace]
(
	@input nvarchar (max), 
	@pattern nvarchar (4000), 
	@replacement nvarchar (max), 
	@options nvarchar (4000)
)
returns nvarchar (max)
as external name [RegexWrapper_v2].[__Regex].[Regex_Replace]

