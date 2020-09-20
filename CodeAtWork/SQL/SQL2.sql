Create table UserChannel(
UserChannelId int primary key identity(1,1),
ChannelName varchar(max) not null,
AppUserId int foreign key references Appuser,
)


Create table ChannelVideo(
UserChannelVideoId int primary key identity(1,1),
UserChannelId int foreign key references UserChannel not null,
VideoId uniqueIdentifier foreign key references VideoRepository not null,
)