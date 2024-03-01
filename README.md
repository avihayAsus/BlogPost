# Blog Post API in .NET 8

## Overview

This project is a simple example of a Blog Post API built using .NET 8 It provides basic CRUD (Create, Read, Update, Delete) operations for managing blog posts and comments.

## Prerequisites

Before you begin, ensure you have the following installed:

## Getting Started

1. **Clone this repository:**
https://github.com/avihayAsus/blogpost.git

2.Configure the database connection in appsettings.json
"ConnectionStrings": {
    "AppConnection": "***"
}
3.Run the following commands in the terminal to apply the database migrations:
dotnet restore
dotnet ef database update
4. dotnet run
5.open swegger
https://localhost:7232/swagger/index.html



