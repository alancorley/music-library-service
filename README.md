# Music Library Service REST API with ASP.NET Core 3

Hello There! :wave: Currently, this is pretty much just an API library for managing a personal music collection and a showcase and playground for me to display and learn new development skills. Yes, I know I am a nerd and I love to learn new things! I am trying out the latest in .NET Core 3 and REST API to see what new features they've added. While I have included a basic ASP.NET Core MVC front-end web application to illustrate and consume the API library, it is by no means meant to be a polished end result. This is more of a proof of concept and a pet project for implementing REST API best practices.

This project contains a music library that uses a REST API service written in C# and uses ASP.NET Core 3 and the MVC pattern. It utilizes AutoMapper, Entity Framework Core Code First, and a SQL Server Express database. The API supports uploading, downloading, listing, deleting, and updating music files and metadata to a SQL Server database (this can easily be ported to use any other persistent datasource since it is using Repository Pattern to decouple data access from the controllers and views). 

In order to handle my requirements for being scalable and handle multiple concurrent requests, I implemented asynchronous threading for I/O bound work such as file system, database, and network calls. I had fun testing this functionality using a free load testing app called WebSurge whereby you can set the number of concurrent threads and requests and see all kinds of statistics. While this is limited to the number of cores on your development machine and not completely indicative of a live production environment, there are ways to artificially simulate load and see how it scales under heavy load.

## Architecture Considerations
- Using MVC (Model View Controller) architecture to separate application components for security and ease of unit testing considerations
- Using AutoMapper to keep classes that represent database entities separate from outer facing model (DTO) classes that are returned to the client to facilitate object mapping
- Ability to handle multiple concurrent requests
- Use consistent naming and URI conventions for ease of implementation and consumption
- Include useful status codes to give client feedback, whether it failed, passed or the request was wrong
- Implement Repository Pattern and interfaces to allow for consolidated query logic, easier unit testing, and ability to use a different datasource if ever needed
- Efficient use of server resources. To make this more scalable, I made these API calls asynchronous so that if ever hosted on an actual web server, it could handle multiple concurrent requests without issue.


## Testing it out / Deployment
1. Clone this repository or download the source code
2. Open the solution using Visual Studio 2019 (Core 3.0 is required)
2. Create sample database using 'Package Manager Console'. The migration folder already contains the latest dbcontext information all you need to do is run 'update-database' to create the database.
3. Ensure that both MusicClient and MusicLibrary.API projects are set to start in the Startup Project dialog of the project. (Right click the solution in Visual Studio, click Properties > Startup Project screen)
4. Run the solution using Visual Studio. The API and the web application for consuming the API will both run.
5. Optional: Use an HTTP client like [Postman](https://www.getpostman.com/) or [Fiddler](https://www.telerik.com/download/fiddler) to interact directly with the API methods. Included in the project are a bunch of Postman HTTP requests, so you can just import them in Postman and use them. The Postman requests are in a file in the root folder of the demos of this module. The file is named MusicLibrary.postman_collection. 

## Future Wishlist
- To avoid storing large audio files directly in the database which would dramatically increase the size of the database over time, I'd like to employ the use of the FileStream method for managing the unstructured failes (i.e. audio files). This method integrates the SQL Server Database Engine with the NTFS files system by storing BLOB files on the file system and includes SQL Server integration to insert, update, query, search, and backup these files. This will increase read operations on the server. Currently this feature is missing from the Entity Framework Core Code First configuration options.
- Include feature to assign multiple genres to music 
- Include feature to upload artist and album artwork
- Implement authentication mechanism to restrict access and user management functionality
- Implement features for filtering and searching library metadata for better usability
- Implement additional exception and error logging
- Implement audio player
- Ability to create custom playlists
- Include ability for bulk inserts for improved performance under load
- Implement .NET Core 3 Razor pages to consume the API

Any feedback is greatly appreciated! If you have any suggestions to make it even better, let me know!

