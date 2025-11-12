ABC Retail Order System
Project Overview
This is a web application for ABC Retail, an e-commerce platform that manages customer, product, and order information. The project demonstrates a modern, scalable, and cloud-native approach by leveraging various Azure Storage services as its back-end data persistence layer. This solution avoids a traditional relational database in favor of a more flexible and cost-effective NoSQL architecture.

Technology Stack
Framework: ASP.NET Core MVC

Language: C#

Development Environment: Visual Studio

Cloud Services: Azure Storage (Tables, Blobs, Queues, Files)

Front-end: Razor Views with HTML, CSS, and JavaScript (default ASP.NET Core template)

Key Features
The application provides a complete set of features for managing the e-commerce system:

Customer Management: Full CRUD (Create, Read, Update, Delete) functionality for customer profiles.

Product Management: Full CRUD functionality for products, including image uploads.

Order Management: Full CRUD functionality for orders, with an integrated asynchronous messaging system.

File Uploads: A dedicated page for uploading documents, such as dummy contracts.

Azure Storage Integration
This project is built around the following Azure Storage services:

Azure Table Storage: Used as the primary NoSQL database to store Customer, Product, and Order data. It is highly scalable and optimized for large volumes of structured, non-relational data.

Azure Blob Storage: Used to store unstructured data, specifically the product images. The application saves the public URL of the image in the Product table and uses it to display the images on the website.

Azure Queue Storage: Implements an asynchronous messaging system. When a new order is created, a message is sent to a queue, decoupling the web application from the back-end order processing logic.

Azure File Storage: Serves as a cloud-based file share for storing documents, such as "dummy contracts."

Setup and Configuration
To run this project, you need an Azure Storage Account and the corresponding connection strings.

1. Azure Setup
Create an Azure Storage Account: In the Azure Portal, create a new Storage Account. Ensure it has the following services enabled: Tables, Blobs, Queues, and Files.

Get Connection Strings: Navigate to your Storage Account in the portal, go to "Access keys," and copy your connection string. This single string provides access to all four services.

Create Containers and Shares:

Blob Container: Create a new blob container named product-images.

File Share: Create a new file share named contracts.

2. Project Configuration
Clone the Repository: Clone this project to your local machine.

Add Connection Strings: Open the appsettings.json file and add your Azure Storage connection string under ConnectionStrings. You can use the same string for all four services if they are in the same storage account.

"ConnectionStrings": {
  "AzureTableStorage": "your_connection_string_here",
  "AzureBlobStorage": "your_connection_string_here",
  "AzureQueueStorage": "your_connection_string_here",
  "AzureFileStorage": "your_connection_string_here"
}

Restore NuGet Packages: In Visual Studio, ensure all NuGet packages are restored. The key packages are:

Azure.Data.Tables

Azure.Storage.Blobs

Azure.Storage.Queues

Azure.Storage.Files.Shares

3. Running the Application
After configuring the connection strings, you can run the application directly from Visual Studio by pressing F5. The application will automatically create the necessary tables and file shares when first accessed.

Deployment to Azure App Service
To deploy the application to a live environment:

Create an App Service: In the Azure Portal, create a new Azure App Service and configure it for ASP.NET Core.

Configure Application Settings: Go to the App Service's "Configuration" settings. For each connection string in your appsettings.json file, add a new "Application setting" with the same name and your connection string as the value. This ensures the deployed application can access your Azure services.

Publish: Use the Publish wizard in Visual Studio to deploy the application directly to your new App Service.

References
Website ABC Retail Link: https://st10445696ordersystem.azurewebsites.net/

YouTube Demo: https://www.youtube.com/watch?v=C8pHQhVYRsk

YouTube School Playlist for grtting started with Table Storage: https://www.youtube.com/playlist?list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7

Microsoft Learn: https://learn.microsoft.com/en-us/azure/storage/tables/

Blog Post on Azure tables: https://code-maze.com/azure-table-storage-aspnetcore/

ChatGPT Chat for aid with creating clean comments: https://chatgpt.com/share/68b095af-96b8-8000-be5f-7bd8ca1ac440

ChatGPT Chat for UI refining: https://chatgpt.com/share/68b095dc-fbe4-8000-ba79-5d2c0662aeb7
