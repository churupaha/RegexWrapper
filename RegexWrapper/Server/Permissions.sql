use master;
go

create asymmetric key RegexWrapper_v2 from executable file = 'C:\Users\Administrator\Desktop\RegexWrapper\RegexWrapper\bin\Release\RegexWrapper_v2.dll';
go

create login RegexWrapper_v2 from asymmetric key RegexWrapper_v2;
go

grant unsafe assembly to RegexWrapper_v2;
go
