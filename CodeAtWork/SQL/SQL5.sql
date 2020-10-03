Create Table Path(
PathId int primary key identity(1,1),
Name varchar(max),
Level int)

Create Table PathVideo(
PathVideoId int primary key identity(1,1),
PathId int foreign key references Path on delete cascade, 
VideoId uniqueidentifier foreign key references videorepository on delete cascade,
[Sequence] int
)

Create table UserPath(
UserPathId int primary key identity(1,1),
PathId int foreign key references Path on delete cascade
)