# Enterprise Request Management API

Enterprise-level ASP.NET Core Web API demonstrating request submission and approval workflow.

## Tech Stack
- ASP.NET Core Web API
- SQL Server
- Dapper
-  JWT Authentication + Role-based Authorization
- Swagger (OpenAPI)

## Features
- Login and generate JWT token
- Role-based access:
  - Requester/Admin: Create request
  - Approver/Admin: Approve/Reject request, View requests
- Request Details endpoint with Approval History
- Audit Logs for request creation and approvals/rejections
- SQL Server schema included

## Setup (Local)
1. Create DB: 'EnterpriseRequestDB'
2. Run script: 'Database/schema.sql'
3. Update connection string in 'appsettings.json'
4. Run project and open `/swagger`
5.  Insert demo users (if not already):

## Endpoints
- POST '/api/requests'
- GET '/api/requests'
- POST '/api/requests/{requestId}/action'
  
SQL -INSERT INTO Users (FullName, Email, Role, IsActive) VALUES
('Amit Requester', 'requester@test.com', 'Requester', 1),
('Neha Approver', 'approver@test.com', 'Approver', 1),
('Admin User', 'admin@test.com', 'Admin', 1);

# API Config
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=EnterpriseRequestDB;Trusted_Connection=True;TrustServerCertificate=True;"
}

Update connection string in appsettings.json

## Screenshots

Check screenshots on folder which is in application screenshot folder for reference.

## Sample Payloads
- swagger link
- created request
- approved request
- get details

### Create Request
'''json
{
  "title": "Laptop Request",
  "description": "Need laptop for new joiner",
  "department": "IT",
  "createdByUserId": 1
}
