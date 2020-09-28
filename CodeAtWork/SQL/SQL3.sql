DECLARE @ConstraintName nvarchar(200)
SELECT 
    @ConstraintName = KCU.CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS RC 
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU
    ON KCU.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG  
    AND KCU.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA 
    AND KCU.CONSTRAINT_NAME = RC.CONSTRAINT_NAME
WHERE
    KCU.TABLE_NAME = 'ChannelVideo' AND
    KCU.COLUMN_NAME = 'UserChannelId'

IF @ConstraintName IS NOT NULL EXEC('alter table ChannelVideo drop  CONSTRAINT ' + @ConstraintName)

ALTER TABLE ChannelVideo
ADD FOREIGN KEY (UserChannelId) REFERENCES UserChannel On delete cascade;

Create table UserDetail(
UserDetailId int primary key identity(1,1),
AppUserId int foreign key references AppUser,
FirstName varchar(max),
LastName varchar(max),
Email varchar(max))
