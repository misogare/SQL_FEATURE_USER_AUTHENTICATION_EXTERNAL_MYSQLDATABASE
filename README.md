# SQL_FEATURE_USER_AUTHENTICATION_EXTERNAL_MYSQLDATABASE
this is a sql feature and user authentication system  in c# .net using asp .net core (MVC architetcture &amp; Razor pages)


## Overview

This project is a C# application built using ASP.NET Core, making it cross-platform and utilizing the MVC architecture for SQL features. It employs Razor Pages for login authentication and interacts with an external MySQL database for data storage.

## Features

### CsvFileModelsController

- **Index**: View and manage uploaded CSV files.
- **Details**: Display details of a specific CSV file.
- **Create**: Upload new CSV files.
- **Edit**: Redirects to the SQL feature for file preparation.

### ERDDatasController

- **Index**: Prepare uploaded CSV files for SQL operations, add, view, alter, or delete data with SQL queries. Two data models are involved.
- **Search**: Execute SQL queries on the CSV data.
- **ClearLog**: Clear the SQL log for the current file.
- **Save**: Save the modified CSV data.

### Login Authentication

- **Login.cshtml and Login.cshtml.cs**: These files contain the logic for user authentication. Users can log in using their email and password, and upon successful authentication, they are redirected to the application's main page.

## Getting Started

1. Clone the repository: 
~~~ bash
git clone https://github.com/yourusername/yourrepository.git
~~~

2. Navigate to the project folder.

3. Run the application: 
~~~ bash
dotnet run
~~~

4. Access the application via a web browser: 
http://localhost:5000


## Dependencies

- ASP.NET Core
- MySQL Database

## Usage

1. Upload your CSV files using the `CsvFileModelsController`.
2. Prepare the uploaded files for SQL operations with the `ERDDatasController`.
3. Execute SQL queries and perform operations on your CSV data.
4. Save the modified data.
5. Use the login functionality to secure your application.

## Roadmap

Future plans for this project include making the SQL feature a standalone API and further enhancing the SQL capabilities. Additionally, improving and expanding the login and authentication system may also be on the agenda.

## Contributing

Feel free to contribute to this project by opening issues or submitting pull requests.

## License

This project is open-source and available under the [MIT License](LICENSE).

