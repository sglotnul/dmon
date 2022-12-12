begin transaction;

create table Championships (
	id int identity(1,1) primary key, 
	name nvarchar(128) not null, 
	startsAt date not null, 
	endsAt date
);

create table ChampionshipStages (
	name nvarchar(128) not null,
	championshipId int foreign key references Championships(id) not null,
	startsAt date not null, 
	endsAt date,
	primary key (name, championshipId)
);

create table Members (
	id int identity(1,1) primary key, 
	name nvarchar(128) not null, 
	country nvarchar(128) not null
);

create table Plays(
	id int identity(1,1) primary key, 
	stageName nvarchar(128) not null,
	championshipId int not null,
	location nvarchar(128) not null,
	startsAt datetime not null, 
	endsAt datetime,
	foreign key (stageName, championshipId) references ChampionshipStages(name, championshipId)
)

create table MemberToPlay(
	memberId int foreign key references Members(id) not null,
	playId int foreign key references Plays(id) not null,
	command nvarchar(128) not null,
	isWinner bit
);

create table Users(
	username nvarchar(128) primary key,
	password nvarchar(64) not null,
);

create table Roles(
	name nvarchar(128) primary key,
	permissions int not null
);

create table RoleToUser(
	userName nvarchar(128) foreign key references Users(username),
	roleName nvarchar(128) foreign key references Roles(name),
	primary key (userName, roleName)
);

create table RoleToTable(
	roleName nvarchar(128) foreign key references Roles(name),
	tableName varchar(128) not null,
	permissions int not null,
	primary key (tableName, roleName)
);

insert into Users values ('admin', 'admin');
insert into Roles values ('sa', 3);
insert into RoleToUser values ('admin', 'sa');
insert into RoleToTable values ('sa', 'RoleToTable', 3);

commit;