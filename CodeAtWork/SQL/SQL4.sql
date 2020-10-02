Create table VideoTopic (
VideoTopicId int primary key identity(1,1),
VideoId uniqueidentifier foreign key references videoRepository on delete cascade, 
InterestCategoryTopicId int foreign key references InterestCategoryTopic on delete cascade
)

alter table VideoRepository
add Level int default (1)

update VideoRepository
set Level =  1