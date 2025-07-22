# Products API

A clean and modular **.NET Web API** project for managing products, built using layered architecture and simple practices.

## Tech Stack

- **.NET 8 Web API**
- **Entity Framework Core**
- **SQL Server**
- **Dependency Injection**
- **NUnit & Moq** 

## Features Implemented

-  Create, Read, Update, Delete (CRUD) operations for Products
-  Pagination support
-  Concurrency handling using RowVersion
-  Custom Action Filters
-  Clean separation using:
    - API Layer
    - Application Layer
    - Domain Layer
    - Infrastructure Layer

##  How to Run

1. Clone the repo  
2. Open the project in an IDE(VS 2022 preferred)
3. Update the connection strings in appsettings.json
4. Do EF core migrations
5. Build and run project using with - dotnet run
6. Unit tests are lacated in Product.Backend.UnitTests project, run it with - dotnet test


##  Future improvements

- Individual projects for each arch layer.
- Add API for bulk creation of product.
- Cover more unit tests.
- Take care of security and performance aspects.
