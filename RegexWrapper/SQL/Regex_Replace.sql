create function [ext].[Regex_Replace]
(
	@input nvarchar (max), 
	@pattern nvarchar (4000), 
	@replacement nvarchar (max), 
	@options nvarchar (4000)
)
returns nvarchar (max)
as external name [RegexWrapper].[UserDefinedFunctions].[Regex_Replace]

