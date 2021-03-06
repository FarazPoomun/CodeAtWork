﻿Create table InterestCategory
(
InterestCategoryId int primary key identity(1,1),
Name varchar (50)
)

Insert into InterestCategory
values ('Software Development'), ('Data')

Create table InterestCategoryTopic
(
InterestCategoryTopicId int primary key identity(1,1),
InterestCategoryId int foreign key references InterestCategory (InterestCategoryId),
Name varchar (50)
)

declare  @CatId int =  (select InterestCategoryId from InterestCategory where name = 'Software Development')

Insert into InterestCategoryTopic
values (@CatId, '.Net'),
(@CatId, '.Net'),
(@CatId, 'JavaScript'),
(@CatId, 'Git'),
(@CatId, 'HTML'),
(@CatId, 'C#'),
(@CatId, 'ASP.net MVC'),
(@CatId, 'TypeScript'),
(@CatId, 'React'),
(@CatId, 'Angular'),
(@CatId, 'Linq'),
(@CatId, 'CSS'),
(@CatId, 'Angular'),
(@CatId, 'Microsoft SQL Server'),
(@CatId, 'MYSQL'),
(@CatId, 'T-SQL'),
(@CatId, 'MongoDB'),
(@CatId, 'Hadoop'),
(@CatId, 'JAVA'),
(@CatId, 'C++'),
(@CatId, 'PHP'),
(@CatId, 'SQL'),
(@CatId, 'JQuery')


 set @CatId =  (select InterestCategoryId from InterestCategory where name = 'Data')

 Insert into InterestCategoryTopic
values (@CatId, 'MongoDB'),
(@CatId, 'Microsoft SQL Server'),
(@CatId, 'Hadoop'),
(@CatId, 'MYSQL'),
(@CatId, 'T-SQL'),
(@CatId, 'PostgreSQL'),
(@CatId, 'Google Analytics')


Create table AppUser(
AppUserId int primary key identity (1,1),
Username varchar (max) not null,
Password varchar (max) not null
)

Insert into AppUser
values ('Admin',1)

Create Table UserSubscribedTopic(
UserSubscribedTopicId int primary key identity (1,1),
AppUserId int Foreign Key references AppUser,
InterestCategoryTopicId int Foreign key references InterestCategoryTopic
)

drop table VideoRepository

Create table VideoRepository
(
VideoId UNIQUEIDENTIFIER primary key default NEWID(),
VideoURL varchar(max) not null, 
IsLocal bit not null default (0),
VideoAuthor varchar (max),
VideoDescription varchar (max)
)

Insert into VideoRepository
Values (default, 'https://www.youtube.com/embed/tgbNymZ7vqY', 0, 'Muppets', 'Muppets Singing Rhapsody')


Create Table UserBookMarkedVideo(
UserBookMarkedVideo int primary key identity(1,1),
VideoId UNIQUEIDENTIFIER Foreign key references VideoRepository, 
AppUserId int Foreign key references AppUser,
CreatedTimeStamp Datetime default (Current_timestamp)
)