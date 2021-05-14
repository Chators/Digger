create table dbo.tSoftware
(
    Id int identity(1, 1) not null,
    Name nvarchar(50) not null,
    Description nvarchar(255) not null,
    AverageExecutionTime int null,

    constraint PK_tSoftware primary key(Id)
);
