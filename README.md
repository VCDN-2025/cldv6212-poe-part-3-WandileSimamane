# ABC Retail — Order System

ABC Retail is a **cloud-native e-commerce web application** built on ASP.NET Core MVC. Instead of a traditional relational database, it uses **Azure Storage** as its entire back-end persistence layer — Tables, Blobs, Queues, and Files — to manage customers, products, and orders in a scalable, cost-effective, NoSQL-first architecture.

---

## Overview

The system demonstrates how a full e-commerce workflow can be built entirely on Azure Storage primitives rather than a conventional SQL database, pairing each storage service with the part of the domain it fits best: structured records in Tables, images in Blobs, async order processing via Queues, and document storage via File Shares.

---

## Features

### Customer Management
- Full **CRUD** functionality for customer profiles.

### Product Management
- Full **CRUD** functionality for products, including **image uploads** stored in Blob Storage.

### Order Management
- Full **CRUD** functionality for orders.
- Integrated **asynchronous messaging**: every new order triggers a queue message, decoupling the web app from downstream order processing.

### File Uploads
- Dedicated page for uploading documents (e.g., dummy contracts) to Azure File Storage.

---

## Tech Stack

| Category | Technology |
| :--- | :--- |
| **Framework** | ASP.NET Core MVC |
| **Language** | C# |
| **IDE** | Visual Studio |
| **Cloud Services** | Azure Storage (Tables, Blobs, Queues, Files) |
| **Frontend** | Razor Views + HTML/CSS/JavaScript |

---

## Azure Storage Architecture

| Service | Role | Details |
| :--- | :--- | :--- |
| **Table Storage** | Primary NoSQL database | Stores Customer, Product, and Order records — scalable, structured, non-relational |
| **Blob Storage** | Image storage | Stores product images; the public URL is saved in the Product table for display |
| **Queue Storage** | Async messaging | New orders trigger a queue message, decoupling the app from order processing logic |
| **File Storage** | Document storage | Cloud file share for documents such as dummy contracts |

---

## Getting Started

### Prerequisites
- An Azure account with permission to create a Storage Account
- Visual Studio with ASP.NET Core workload installed

### 1. Azure Setup

1. **Create a Storage Account** in the Azure Portal with **Tables**, **Blobs**, **Queues**, and **Files** enabled.
2. **Get the connection string** — go to the Storage Account → *Access keys* → copy the connection string (one string covers all four services).
3. **Create the required containers/shares**:
   - Blob container: `product-images`
   - File share: `contracts`

### 2. Project Configuration

1. **Clone the repository.**
2. **Add connection strings** to `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "AzureTableStorage": "your_connection_string_here",
     "AzureBlobStorage": "your_connection_string_here",
     "AzureQueueStorage": "your_connection_string_here",
     "AzureFileStorage": "your_connection_string_here"
   }
   ```
   The same connection string can be reused across all four keys if the services share one storage account.
3. **Restore NuGet packages** in Visual Studio. Key packages:
   - `Azure.Data.Tables`
   - `Azure.Storage.Blobs`
   - `Azure.Storage.Queues`
   - `Azure.Storage.Files.Shares`

### 3. Run the Application

Press `F5` in Visual Studio. Tables and file shares are created automatically on first run.

---

## Deployment to Azure App Service

1. **Create an App Service** in the Azure Portal, configured for ASP.NET Core.
2. **Configure application settings** — under the App Service's *Configuration* blade, add an application setting for each connection string in `appsettings.json`, using the same names and values.
3. **Publish** directly from Visual Studio using the Publish wizard.

---

## Live Demo

🌐 **Live Site**: [abcretailordersy...southafricanorth-01.azurewebsites.net](https://abcretailordersy-a4cfffbphmcmbcfu.southafricanorth-01.azurewebsites.net/)

📺 **YouTube Walkthrough**: [Watch the demo](https://www.youtube.com/watch?v=lzHLhpw9xUQ)

> **Note**: Azure resources were wiped and had to be re-deployed — twice. If the live link is unavailable, this is likely why.

> **Note**: Azure Functions and shared Services are included in the repo as zipped files, since they could not be pushed while living in the same solution.

---

## References

| # | Source | Description | Link |
| :--- | :--- | :--- | :--- |
| 1 | YouTube — School Playlist | Getting started with Azure Table Storage | [Playlist](https://www.youtube.com/playlist?list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7) |
| 2 | Microsoft Learn | Azure Table Storage documentation | [Docs](https://learn.microsoft.com/en-us/azure/storage/tables/) |
| 3 | Code Maze | Blog post on Azure Table Storage with ASP.NET Core | [Blog](https://code-maze.com/azure-table-storage-aspnetcore/) |
| 4 | ChatGPT | Aid with writing clean code comments | [Chat](https://chatgpt.com/share/68b095af-96b8-8000-be5f-7bd8ca1ac440) |
| 5 | ChatGPT | UI refinement assistance | [Chat](https://chatgpt.com/share/68b095dc-fbe4-8000-ba79-5d2c0662aeb7) |

---

## Author

**Wandile Simamane**
