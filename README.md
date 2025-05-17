# 🎟️ Backend Event Booking System 
This is the backend implementation for an event booking system , It includes a powerful RESTful API, real-time notifications, scheduled email reminders, and several advanced backend features.


🚀 Features Implemented

    1.SignalR Integration:
      Sends real-time messages to users when a new event is added with same category they liked
      
    2. Email Reminder sent 24 hours before a booked event
    
    3. Caching to improve performance
    
    4. Unit of Work and Generic Repository Pattern
    
    5. Specification Design Pattern
    
    6. Usage of AsNoTracking to speed up read-only operations
    
    7. Data Seeding with initial events, categories, and tags
    
    8. Tags and Categories support for events
    
    9. Role-based Permissions (Admin & User)
    
    10. Centralized Exception Handling
    
    11. JWT Authentication
    
    12. Image URL Resolver for event images
    
    13. Image Upload & Delete functionality
    
    14. Pagination for event listings
    
    15. AutoMapper for clean DTO mapping
    
    18. First registered account is automatically assigned the Admin role
    
    16. Automatic Update Database 
    
    17. Swagger / OpenAPI Documentation

🧪 Setup instructions
    
    1. Clone the Repository
    
    2. Configure the appsettings.json
    
          Set your ApiBaseUrl to your local host
     
          Set your EmailSettings
    
    4. Run the Application
    
          The application will:
      
          Apply all migrations
      
          Seed initial data (categories, events, etc.)
      
          Create the database if it doesn't exist
    
    5. Use Postman to Test the API
     
          Import the included Postman Collection
    
          Update the Base URL to your local host (e.g., http://localhost:5000)
    
    6. Register an Account
    
          First registered account is assigned Admin role automatically.


📁 Image Upload Notes

      Images are stored in wwwroot/images/
  
      Image URLs are resolved using a custom resolver


🔒 Security & Best Practices

      Role-based access for admin endpoints
  
      JWT-based stateless authentication
  
      Global error handler for consistent API responses
