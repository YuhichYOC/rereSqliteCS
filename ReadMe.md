## rereSqlite - Headliner by Yuichi Yoshii is licensed under the Apache License, Version2.0

# What can this do ?
1. Create sqlite database file with encryption using SQLCipher.
1. Access to sqlite database file encrypted with SQLCipher.
1. Execute any DDL to sqlite database file.
1. Execute any DML to sqlite database file.
1. Store any string. ( e.g. Password for web service )
1. Store any file. ( e.g. Zip file, pictures or any binary of program )
1. Copy existing encrypted sqlite database file to another one with other password.

# Create sqlite database file with encryption using SQLCipher.
- Enter password as you like.  
![Before enter password](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/01/1.PNG?raw=true)  
![After enter password. This case I use 'test'.](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/01/2.PNG?raw=true)  
- Enter path to create new sqlite database file.  
![Before select the path](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/01/3.PNG?raw=true)  
![After select the path](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/01/4.PNG?raw=true)  
- Close open file dialog, then database file will be created into the path you are entered.  
![After close open file dialog](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/01/5.PNG?raw=true)  
- In my local filesystem ... database file 'test01' is created in E:\workspace.  
![test01 in local filesystem](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/01/6.PNG?raw=true)  

# Access to sqlite database file encrypted with SQLCipher.
- Enter password for sqlite database file when you create it.  
![Before enter password](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/02/1.PNG?raw=true)  
![After enter password](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/02/2.PNG?raw=true)  
- Select database file you made in your filesystem.  
![Before select the file](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/02/3.PNG?raw=true)  
![After select the file](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/02/4.PNG?raw=true)  
![After open it](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/02/5.PNG?raw=true)  

# Execute any DDL to sqlite database file.
- Initially there are 5 tables in database file. These are made by rereSqlite. ( Setup/DataBaseInitializer.cs )  
![Before make any table](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/03/2.PNG?raw=true)  
- To create any table ( or execute any DDL ), you should use 'Execute any SQL queries' page.  
![Creating TABLE03](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/03/3.PNG?raw=true)  
![Before refresh the table list](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/03/4.PNG?raw=true)  
![After refresh the table list](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/03/5.PNG?raw=true)  

# Execute any DML to sqlite database file.
- To execute any DML, you should use 'Execute any SQL queries' page.  
![Execute INSERT statement to TABLE03](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/04/1.PNG?raw=true)  
- To view table data in database, you should double click table name cell in table list.  
![Before double click 'TEST03'](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/04/2.PNG?raw=true)  
![Go to 'Result for select statement' page, open 'Query1'](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/04/3.PNG?raw=true)  
![There is one row in 'TEST03'](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/04/4.PNG?raw=true)  
- rereSqlite can control transaction. To begin one, you should click 'Begin the transaction' button.  
![Before beginning the transaction](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/04/5.PNG?raw=true)  
![Transaction has been begun. Then, execute INSERT and UPDATE statement.](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/04/6.PNG?raw=true)  
- To rollback the transaction, you should click 'Rollback' button.  
![Rollback](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/04/7.PNG?raw=true)  
- There is one row in TEST03. Queries executed a while ago are rolled back.  
![In TEST03, there is one row.](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/04/8.PNG?raw=true)  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/04/9.PNG?raw=true)  
- Next, commit same operations.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/04/10.PNG?raw=true)  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/04/11.PNG?raw=true)  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/04/12.PNG?raw=true)  
- There are two rows in TEST03. Queries are committed.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/04/12.PNG?raw=true)  

# Store any string. ( e.g. Password for web service )
- rereSqlite can store any strings. You should use 'Storage for string datas' page.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/05/1.PNG?raw=true)  
- To show input field, first you should input 'Key string'.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/05/2.PNG?raw=true)  
- Click 'Search' button. If no records find in database file, blank field will be shown.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/05/3.PNG?raw=true)  
- Input value as you like.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/05/4.PNG?raw=true)  
- Click 'Register' button.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/05/5.PNG?raw=true)  
- Strings you input are stored into 'STRING_STORAGE' table.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/05/6.PNG?raw=true)  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/05/7.PNG?raw=true)  
- 'Search' button searches the value with prefix search.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/05/8.PNG?raw=true)  

# Store any file. ( e.g. Zip file, pictures or any binary of program )
- As same as storing string data, this program can store any files.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/06/1.PNG?raw=true)  
- Show blank field.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/06/2.PNG?raw=true)  
- Select file to store into database.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/06/3.PNG?raw=true)  
- Click 'Register' button.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/06/4.PNG?raw=true)  
- To take out the file stored in database, you should click 'Take out the file into filesystem' button.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/06/5.PNG?raw=true)  
- Enter the full path to save file.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/06/6.PNG?raw=true)  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/06/7.PNG?raw=true)  

# Copy existing encrypted sqlite database file to another one with other password.
- rereSqlite can clone sqlite database file.
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/07/1.PNG?raw=true)  
- Enter new password for cloned database file.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/07/2.PNG?raw=true)  
- Enter full path for cloned database file.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/07/3.PNG?raw=true)  
- Click 'Start clone the database file with new password' button.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/07/4.PNG?raw=true)  
- Open cloned database file.  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/07/5.PNG?raw=true)  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/07/6.PNG?raw=true)  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/07/7.PNG?raw=true)  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/07/8.PNG?raw=true)  
![](https://github.com/YuhichYOC/rereSqlite---Headliner/blob/master/screen/07/9.PNG?raw=true)  
