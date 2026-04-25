# Brew Haven - Coffee Shop Management System

Brew Haven is a robust ASP.NET Core MVC web application designed to streamline coffee shop operations. It provides a seamless interface for managing customer orders, tracking menu items, and providing real-time nutritional data via external API integration.

## 🚀 Project Overview
This application serves as a centralized platform for both baristas and managers. It leverages the Power of .NET 8 to provide a high-performance, scalable solution for order management and business analytics.

---

## 🛠 Technical Details

### API Endpoints
The system integrates with the **API Ninjas Nutrition API** to provide customers and staff with accurate nutritional information for menu items.
* **Endpoint:** `https://api.api-ninjas.com/v1/nutrition?query={item}`
* **Usage:** When a user views a menu item, the application fetches calories, protein, and sugar content dynamically to display on the "Nutrition Info" page.

### Data Model (ERD)
The database architecture is designed for referential integrity and performance. The core entities include:
* **Customers:** Stores user profiles and contact information.
* **Orders:** Tracks transaction details and timestamps.
* **OrderItems:** A junction table facilitating a many-to-many relationship between Orders and Menu Items.
* **Menu Items:** Contains pricing, category, and description data.

> **Note:** The updated ERD diagram can be found in the `/docs/assets` folder of this repository.

### CRUD Implementation
Brew Haven follows the standard MVC pattern for full CRUD operations across all major entities:
* **Create:** Interactive forms for placing new customer orders and adding menu items.
* **Read:** Dynamic dashboards that display order history and inventory lists using SQL-backed queries.
* **Update:** Edit functionality for modifying active orders or updating pricing in the menu.
* **Delete:** Soft-deletion and hard-deletion capabilities for managing outdated records.

---

## 🧠 Technical Challenges & Solutions

### 1. Handling Transient Database Failures
**Challenge:** During development, the application occasionally lost connection to the SQL server, resulting in `System.InvalidOperationException` (Transient Failures).
**Solution:** Implemented **Connection Resiliency** in `Program.cs` by enabling `EnableRetryOnFailure()` within the SQL Server configuration. This allows the app to automatically retry failed database commands caused by temporary network blips.

### 2. Corrupted IDE Configuration Files
**Challenge:** Team members encountered build errors (e.g., `MSB40...`) where the project would not load because of corrupted `.csproj.user` files.
**Solution:** Identified that local user settings should not be shared across environments. We updated our `.gitignore` to exclude `.user` files and provided a protocol for team members to delete local corrupted metadata to refresh the environment.

### 3. External API Integration & JSON Parsing
**Challenge:** Integrating the Nutrition API led to issues with inconsistent JSON responses when certain nutrients were missing from the external database.
**Solution:** Developed a robust DTO (Data Transfer Object) and used nullable types in C# to handle missing JSON keys without crashing the application.

### 4. Transitioning to Dynamic Architecture
**Challenge:** Converting a static HTML prototype into a dynamic MVC system while maintaining the UI design.
**Solution:** Utilized **Razor Views** and **View Components** to modularize the frontend, allowing us to pass dynamic data from the controllers while keeping the CSS/JS intact.

---

## 👥 Contributors
* **Group 10**

## 🔗 Repository
[View Project on GitHub](https://github.com/your-username/Coffee-Shop-Group-10)
