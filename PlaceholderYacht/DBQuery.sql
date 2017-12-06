﻿use WindCatchersDB

GO
create schema Sai

GO
drop table Sai.VPP
GO
create table Sai.VPP
(
	ID int identity primary key,
	BoatID int foreign key references sai.Boat(ID),
	TWS int not null,
	WindDegree int not null,
	Knot decimal(4,2) not null,
	constraint vpp_uq unique nonclustered(BoatID, TWS, WindDegree)
)

GO
drop table Sai.Boat
GO
create table Sai.Boat
(
	ID int identity primary key,
	[UID] nvarchar(450),
	Modelname varchar(50) not null,
	Manufacturer varchar(50) not null,
	Boatname varchar(50) default 'Standard'
)