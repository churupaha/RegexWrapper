create function [ext].[Regex_Match]
(
	@input nvarchar (max), 
	@pattern nvarchar (4000), 
	@options nvarchar (4000)
)
returns nvarchar(max)
as external name [RegexWrapper].[UserDefinedFunctions].[Regex_Match]