CREATE DATABASE IotMonitor;
GO;

USE IotMonitor
GO;

CREATE TABLE LogSensores(
	Id int primary key identity,
	ReceivedDate datetime not null,
	Content varchar(255),
	Protocol char(3) not null,
	CreatedDate datetime default GETDATE()
);
GO;