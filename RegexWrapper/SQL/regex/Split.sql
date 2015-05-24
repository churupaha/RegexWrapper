create function [regex].[Split]
(
	@handle bigint,
	@input nvarchar (max)
)
returns 
     table 
	 (
        [part] nvarchar (max) null
	)
as external name [RegexWrapper_v2].[__Regex].[Split]
