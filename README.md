# Enterprise Request Management API

Enterprise-level ASP.NET Core Web API demonstrating request submission and approval workflow.

## Tech Stack
- ASP.NET Core Web API
- SQL Server
- Dapper
- Swagger (OpenAPI)

## Features
- Create request
- View requests
- Approve/Reject request (stores approval history)
- SQL Server schema included

## Setup (Local)
1. Create DB: 'EnterpriseRequestDB'
2. Run script: 'Database/schema.sql'
3. Update connection string in 'appsettings.json'
4. Run project and open `/swagger`

## Endpoints
- POST '/api/requests'
- GET '/api/requests'
- POST '/api/requests/{requestId}/action'

## Sample Payloads

### Create Request
'''json
{
  "title": "Laptop Request",
  "description": "Need laptop for new joiner",
  "department": "IT",
  "createdByUserId": 1
}
