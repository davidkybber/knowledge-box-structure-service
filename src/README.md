# KnowledgeBox API

A .NET 8.0 Web API implementation for the KnowledgeBox application, providing a complete backend service for managing knowledge boxes with JWT authentication.

## Features

- ✅ Full CRUD operations for Knowledge Boxes
- ✅ JWT-based authentication
- ✅ Search functionality (text search and tag filtering)
- ✅ Public/private knowledge box support
- ✅ User authorization and ownership validation
- ✅ Comprehensive error handling
- ✅ Unit tests for service layer
- ✅ Swagger/OpenAPI documentation
- ✅ CORS support
- ✅ In-memory database for development

## API Endpoints

### Authentication
- `GET /auth/test-token` - Generate a test JWT token
- `POST /auth/generate-test-token` - Generate a custom test JWT token

### Knowledge Boxes
- `GET /knowledgeboxes` - Get all knowledge boxes for authenticated user
- `GET /knowledgeboxes/{id}` - Get a specific knowledge box by ID
- `POST /knowledgeboxes` - Create a new knowledge box
- `PUT /knowledgeboxes/{id}` - Update an existing knowledge box
- `DELETE /knowledgeboxes/{id}` - Delete a knowledge box
- `GET /knowledgeboxes/search` - Search knowledge boxes by query and/or tags
- `GET /knowledgeboxes/public` - Get all public knowledge boxes

## Quick Start

### 1. Build and Run

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

The API will be available at `https://localhost:5001` (or `http://localhost:5000`)

### 2. Generate Test Token

First, get a test JWT token:

```bash
curl -X GET "https://localhost:5001/auth/test-token"
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "test-user-123",
  "email": "test@example.com",
  "expiresAt": "2024-01-15T12:00:00Z",
  "usage": "Use this token in Authorization header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### 3. Use the API

Use the token in the Authorization header for all protected endpoints:

```bash
curl -X GET "https://localhost:5001/knowledgeboxes" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### 4. Create a Knowledge Box

```bash
curl -X POST "https://localhost:5001/knowledgeboxes" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Machine Learning Fundamentals",
    "topic": "Artificial Intelligence",
    "content": "Introduction to machine learning concepts and algorithms",
    "isPublic": false,
    "tags": ["machine-learning", "ai", "python"]
  }'
```

## Data Models

### KnowledgeBox
```csharp
public class KnowledgeBox
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Topic { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UserId { get; set; }
    public bool IsPublic { get; set; }
    public List<string> Tags { get; set; }
    public List<string> Collaborators { get; set; }
}
```

### Request Models
- `CreateKnowledgeBoxRequest` - For creating new knowledge boxes
- `UpdateKnowledgeBoxRequest` - For updating existing knowledge boxes

### Response Models
- `KnowledgeBoxResponse` - Single knowledge box response
- `KnowledgeBoxListResponse` - List of knowledge boxes response
- `DeleteKnowledgeBoxResponse` - Delete operation response

## Authentication

The API uses JWT Bearer tokens for authentication. All endpoints except `/auth/*` require a valid JWT token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

The token should contain the following claims:
- `sub` or `user_id` or `NameIdentifier` - User ID

## Business Rules

1. **Ownership**: Users can only modify/delete their own knowledge boxes
2. **Public Access**: Public knowledge boxes are readable by all authenticated users
3. **Search Scope**: Search includes user's own knowledge boxes and public ones
4. **Content Validation**: Title and topic are required fields
5. **Tags**: Tags are case-insensitive and automatically trimmed/normalized

## Error Handling

The API returns appropriate HTTP status codes:

- `200 OK` - Successful operation
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required or invalid
- `403 Forbidden` - User doesn't have permission
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

Error responses follow this format:
```json
{
  "success": false,
  "message": "Detailed error message",
  "error": "ERROR_CODE"
}
```

## Testing

Run the unit tests:

```bash
dotnet test
```

The test suite includes:
- Service layer unit tests
- Database integration tests
- Authentication and authorization tests
- Search functionality tests

## Development Notes

- Uses Entity Framework Core with In-Memory database for development
- JWT tokens are configured in `appsettings.json`
- CORS is enabled for all origins (configure appropriately for production)
- Swagger UI is available at `/swagger` in development

## Configuration

Key configuration options in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "KnowledgeBoxAPI",
    "Audience": "KnowledgeBoxClient",
    "ExpiryInMinutes": 60
  }
}
```

## Swagger Documentation

When running in development mode, comprehensive API documentation is available at:
- Swagger UI: `https://localhost:5001/swagger`
- OpenAPI spec: `https://localhost:5001/swagger/v1/swagger.json`

## Architecture

The application follows a clean architecture pattern:

```
Controllers/     - HTTP request handling
├── Services/    - Business logic
├── Data/        - Database context and configuration
├── Models/      - Data models and DTOs
├── Utilities/   - Helper classes
└── Tests/       - Unit tests
```

## Next Steps

For production deployment, consider:
1. Replace in-memory database with a persistent database (SQL Server, PostgreSQL, etc.)
2. Configure proper JWT settings with secure keys
3. Implement proper CORS policies
4. Add rate limiting
5. Add database migrations
6. Set up logging and monitoring
7. Add integration tests
8. Configure environment-specific settings