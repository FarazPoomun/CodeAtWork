
Create table UserChannel(
UserChannelId int primary key identity(1,1),
ChannelName varchar(max) not null,
AppUserId int foreign key references Appuser,
IsShared bit default(0)
)


Create table ChannelVideo(
ChannelVideoId int primary key identity(1,1),
UserChannelId int foreign key references UserChannel not null,
VideoId uniqueIdentifier foreign key references VideoRepository not null,
)

