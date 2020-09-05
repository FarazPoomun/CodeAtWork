Create table InterestCategory
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