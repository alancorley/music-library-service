# Music Library Service REST API with ASP.NET Core 3

Hello There! :wave: Currently, this is pretty much just an API library with a simple web client to all me to display and learn new development skills. Yes, I know I am a nerd and I love to learn new things! I am trying out the latest in ASP.NET Core 3, Entity Framework Code First, MVC, and REST APIs. While I have included a basic ASP.NET Core MVC front-end web application to illustrate and consume the API library using HttpClient, it is by no means meant to be a polished end result. This is more of a proof of concept and a pet project for implementing best practices and the latest and greatest features. 

This project contains a music library that uses a REST API service written in C# and uses ASP.NET Core 3 and utilizing the MVC pattern. It utilizes AutoMapper, Entity Framework Core Code First, and a SQL Server Express database. The API supports uploading, downloading, listing, deleting, and creating music files and corresponding metadata to a SQL Server database (this can easily be ported to use any other persistent datasource since it is using Repository Pattern to decouple data access from the controllers and views).

In order to handle my requirements for being scalable and handle multiple concurrent requests, I implemented asynchronous threading for I/O bound work such as file system, database, and network calls. I had fun testing this functionality using a free load testing app called WebSurge where you can set the number of concurrent threads and requests and see all kinds of statistics. While this test is limited to the number of cores on your development machine and not completely indicative of a live production environment, there are ways to artificially simulate load and see how it scales under heavy load. I suggest you play around with it if that interests you too. :)

Initially I had plans to store the audio files in the SQL Server FileStream format where file references are stored in the database and the files themselves are kept on the file system under SQL Server Database Engine NTFS management. But it became apparent to me that FileStream isn't yet supported by the Entity Framework Code First approach I was using so I decided to simply store the files on the file system and add references to the database without the FileStream option enabled. In a real world scenario where this project is used a in a production environment, I would initialize the database first manually and implement FileStream, and then all code migrations afterwards would be fine (hypothetically). 

The web application client project which consumes the API methods is really a rough and dirty implementation for demonstrating the functionality of the API and it also gave me a real-world scenario to use as my playground. Yes, I could always use PostMan or Fiddler for testing my API (which I did plenty of), but I wanted a realistic playground to consume and test functionality. I have included Swagger API documentation 



## Architecture Considerations
- Using MVC (Model View Controller) architecture to separate application components for security and ease of development
- Using AutoMapper to keep classes that represent database entities separate from outer facing model (DTO) classes that are returned to the client to facilitate object mapping
- Ability to handle multiple concurrent requests
- Using consistent naming and URI conventions for ease of implementation and consumption
- Include useful status codes to give client feedback, whether it failed, passed or the request was wrong
- Implement Repository Pattern and interfaces to allow for consolidated query logic, easier unit testing, and ability to use a different datasource if ever needed
- Efficient use of server resources. To make this more scalable, I made these API calls asynchronous so that if ever hosted on an actual web server, it could handle multiple concurrent requests without issue. I am making sure that objects are removed from memory when they're completed, threads are destroyed, and other resources are managed as tightly as possible.

## Testing it out / Deployment
1. Clone this repository or download the source code
2. Open the solution using Visual Studio 2019 (Core 3.0 is required)
2. Create sample database using 'Package Manager Console'. The migration folder already contains the latest dbcontext information all you need to do is run 'update-database' to create the database.
3. Ensure that both MusicClient and MusicLibrary.API projects are set to 'Start' in the Startup Project dialog of the project. (Right click the solution in Visual Studio, click Properties > Startup Project screen)
4. Run the solution using Visual Studio. The API and the web application for consuming the API will both run.
5. Optional: Use an HTTP client like [Postman](https://www.getpostman.com/) or [Fiddler](https://www.telerik.com/download/fiddler) to interact directly with the API methods. 

## Future Wishlist
- To avoid storing large audio files directly in the database which would dramatically increase the size of the database over time, I'd like to employ the use of the FileStream method for managing the unstructured failes (i.e. audio files). This method integrates the SQL Server Database Engine with the NTFS files system by storing BLOB files on the file system and includes SQL Server integration to insert, update, query, search, and backup these files. This will increase read operations on the server. Currently this feature is missing from the Entity Framework Core Code First configuration options.
- Include feature to assign multiple genres to music 
- Include feature to upload artist and album artwork
- Include ability for bulk inserts for improved performance under load
- Implement unit testing
- Implement better caching resources to handle more concurrent traffic
- Implement security and  authentication mechanism to restrict access and user management functionality
- Implement features for filtering and searching library metadata for better usability
- Implement additional exception and error logging
- Implement embedded audio player with on the fly transcoding
- Ability to create custom playlists
- Implement .NET Core 3 Razor pages to consume the API

## Production/Real-life Considerations
- Horizontal scaling: consider hosting the API and the code making requests to the API on multiple cloud servers
- Consider load balancing these servers to handle high trafffic and increase realiability and speed
- Vertical scaling: consider beefing up the existing hardware with more memory, faster processors, more space, etc. 
- Consider caching data to improve performance 
- Implement queueing to avoid buffer spikes in activity and have as few processes as possible. This can be done using background workers/processes to queue up tasks when needed.
- Implement realtime alerts and monitoring
- Employ load testing to identify bottlenocks

Any feedback is greatly appreciated! If you have any suggestions to make it even better, let me know!

