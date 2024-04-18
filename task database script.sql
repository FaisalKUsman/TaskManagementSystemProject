create database TaskDB
go
use taskdb
go
create table UserMaster(Id int primary key identity(1,1),UserName varchar(200),UserPassword nvarchar(100),CreatedOn datetime default getdate())
go
insert into usermaster(username,UserPassword) values('testuser','cde43')
go
create procedure CreateUser @Name varchar(200),@Password nvarchar(100)
as
begin
  if(not exists(select usermaster.UserName from usermaster where usermaster.UserName=@Name))
  begin
    insert into usermaster(username,UserPassword) values(@name,@Password);
	select convert(int,@@IDENTITY);
  end
  else
  begin
    select -1;
  end
end
go
create procedure GetUser @Name varchar(200),@password nvarchar(100)
as
begin
  select usermaster.Id,usermaster.UserName from usermaster where usermaster.UserName=@Name and usermaster.UserPassword=@password;
end
go
create table TaskMaster(Id int primary key identity(1,1),Title varchar(200),TaskDescription nvarchar(max),DueDate date,TaskStatus varchar(50),  check(taskstatus in('Incomplete','Complete')),CreatedBy int references usermaster(id),CreatedOn datetime default getdate())
go
create procedure GetTasks @id int=null,@CreatedBy int=null
as
begin
  select taskmaster.id,taskmaster.title,taskdescription,duedate duedate,taskstatus,createdby,usermaster.UserName createduser from taskmaster left outer join usermaster   on usermaster.id=taskmaster.createdby where taskmaster.createdby=isnull(@createdby,taskmaster.createdby) and taskmaster.id=isnull(@id,taskmaster.id)
end
go
create procedure SaveTask @id int =null,@title varchar(200),@description nvarchar(max)=null,@duedate date,@status varchar(100),@createdby int
as
begin
  if(isnull(@id,0)=0)
  begin
    insert into taskmaster(title,taskdescription,duedate,taskstatus,createdby) values(@title,@description,@duedate,@status,@createdby);
	select @id=@@IDENTITY;
  end
  else
  begin
    update taskmaster set title=@title,taskdescription=@description,duedate=@duedate,taskstatus=@status where id=@id;
  end
  select @id;
end
go
create procedure DeleteTask @id int
as
begin
  delete from taskmaster where id=@id;
end
select @@version