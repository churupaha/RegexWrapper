create function [ext].[Regex_IsMatch]
(
	@input nvarchar (max), 
	@pattern nvarchar (4000), 
	@options nvarchar (4000)
)
returns bit
as external name [RegexWrapper].[UserDefinedFunctions].[Regex_IsMatch]