### PAC Project

This project allows for "vendors" to be added and maintained by administrators and parses the site(s) added to determine the plants which they presently have in inventory. The technology stack is 

* C# Dotnet Core MVC
* Vue
* MariaDB via Dapper
* NodeJS

Note: Additional setup instructions may reside in each of the project folders.

## CrawlerTests

All unit test(s) for site parsing and spidering are run here. You can execute them from Visual Studios Tests window.

## DesignAssets
All high res graphic assets are retained here.

## ImportPlants
Initial plant import from JSON into MariaDB 

## Libraries
Dotnet dependancies that are not on Nuget

## migrations
All .sql migrations in order to get from one version to another.  Utilizes a project called Mite in order to maintain and execute them.  See https://github.com/soitgoes/Mite for details.

## Repositories
C Sharp project containing the database logic.

## Savvy Crawler
The spidering libraries

## Shared
All POCO objects used.

## web

webapi is a Dotnet Core API application
vueapp is a vue based.

For local development you should run webapi in Visual studio or from command line:  

```dotnet run

Then run the vue app.  

```npm start

The vue application will output a url and that will be the vue application but will proxy to the web application.  You must change the proxyDestination in the vue.config.js to match the random port selected by dotnet when the webapi is launched.

## API Documentation

API documentation is generated automatically by the comments above the controller methods. See the following example in Controllers/PlantController.cs 

 /// <summary>
/// Finds all the vendors that have the plant designated.
/// </summary>
/// <param name="plantId">uuid for plant</param>
/// <param name="zipCode">5 digit zipcode</param>
/// <param name="radius">in miles</param>
/// <returns></returns>

## Quickstart

* Install Mite.  See instructions here.  https://github.com/soitgoes/Mite
```cd migrations
* Alter the connection string to match your local mariadb settings in mite.config
```mite up
* Alter the connection string in the webapi/appsettings.json 
* Run the webapi in visual studio "Run" or with dotnet run from terminal
* cd vueapp
* npm i
* npm start
* click the link 
* drop a user in the user table with your email then utilize the forgot password.

