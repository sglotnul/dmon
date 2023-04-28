begin transaction;

create table Categories (
	name nvarchar(128) primary key
);

create table Products (
	id bigint identity(1,1) primary key, 
	name nvarchar(128) not null, 
	price money not null,
	categoryName nvarchar(128) foreign key references Categories(name) not null
);

create table Stores (
	id int identity(1,1) primary key, 
	location nvarchar(128) not null,
	manager nvarchar(128)
);

create table ProductToStore(
	storeId int foreign key references Stores(id),
	productId bigint foreign key references Products(id),
	productCount int not null,
	primary key (storeId, productId)
)

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
insert into RoleToTable values ('sa', 'Categories', 3);
insert into RoleToTable values ('sa', 'Products', 3);
insert into RoleToTable values ('sa', 'Stores', 3);
insert into RoleToTable values ('sa', 'ProductToStore', 3);
insert into RoleToTable values ('sa', 'Roles', 3);
insert into RoleToTable values ('sa', 'Users', 3);
insert into RoleToTable values ('sa', 'MemberToPlay', 3);
insert into RoleToTable values ('sa', 'RoleToUser', 3);
insert into RoleToTable values ('sa', 'RoleToTable', 3);

commit;