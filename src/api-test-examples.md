# API Test Examples

This document provides example API calls to test the KnowledgeBox API functionality.

## 1. Generate Test Token

### Get Default Test Token
```bash
curl -X GET "https://localhost:5001/auth/test-token" \
  -H "accept: application/json"
```

### Generate Custom Test Token
```bash
curl -X POST "https://localhost:5001/auth/generate-test-token" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user-123",
    "email": "user@example.com"
  }'
```

## 2. Knowledge Box Operations

**Note**: Replace `YOUR_TOKEN_HERE` with the actual JWT token from step 1.

### Create a Knowledge Box
```bash
curl -X POST "https://localhost:5001/knowledgeboxes" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Machine Learning Fundamentals",
    "topic": "Artificial Intelligence",
    "content": "Introduction to machine learning concepts including supervised learning, unsupervised learning, and reinforcement learning.",
    "isPublic": false,
    "tags": ["machine-learning", "ai", "python", "data-science"]
  }'
```

### Get All Knowledge Boxes
```bash
curl -X GET "https://localhost:5001/knowledgeboxes" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Get Knowledge Box by ID
```bash
curl -X GET "https://localhost:5001/knowledgeboxes/KNOWLEDGE_BOX_ID" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Update a Knowledge Box
```bash
curl -X PUT "https://localhost:5001/knowledgeboxes/KNOWLEDGE_BOX_ID" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "KNOWLEDGE_BOX_ID",
    "title": "Advanced Machine Learning",
    "content": "Deep dive into machine learning including neural networks, deep learning, and practical applications.",
    "tags": ["machine-learning", "ai", "python", "deep-learning", "neural-networks"]
  }'
```

### Delete a Knowledge Box
```bash
curl -X DELETE "https://localhost:5001/knowledgeboxes/KNOWLEDGE_BOX_ID" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## 3. Search Operations

### Search by Query
```bash
curl -X GET "https://localhost:5001/knowledgeboxes/search?query=machine%20learning" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Search by Tags
```bash
curl -X GET "https://localhost:5001/knowledgeboxes/search?tags=ai,python" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Search by Query and Tags
```bash
curl -X GET "https://localhost:5001/knowledgeboxes/search?query=learning&tags=ai" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Get Public Knowledge Boxes
```bash
curl -X GET "https://localhost:5001/knowledgeboxes/public" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## 4. Complete Test Scenario

Here's a complete test scenario to demonstrate all functionality:

### Step 1: Get Test Token
```bash
TOKEN=$(curl -s -X GET "https://localhost:5001/auth/test-token" | grep -o '"token":"[^"]*' | cut -d'"' -f4)
echo "Token: $TOKEN"
```

### Step 2: Create Multiple Knowledge Boxes
```bash
# Create first knowledge box
curl -X POST "https://localhost:5001/knowledgeboxes" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "PLC Programming Basics",
    "topic": "Industrial Automation",
    "content": "Introduction to Programmable Logic Controllers (PLCs) and ladder logic programming.",
    "isPublic": true,
    "tags": ["plc", "automation", "programming", "industrial"]
  }'

# Create second knowledge box
curl -X POST "https://localhost:5001/knowledgeboxes" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "React Development",
    "topic": "Web Development",
    "content": "Modern React development with hooks, context, and state management.",
    "isPublic": false,
    "tags": ["react", "javascript", "web-development", "frontend"]
  }'
```

### Step 3: List All Knowledge Boxes
```bash
curl -X GET "https://localhost:5001/knowledgeboxes" \
  -H "Authorization: Bearer $TOKEN"
```

### Step 4: Search Knowledge Boxes
```bash
# Search by query
curl -X GET "https://localhost:5001/knowledgeboxes/search?query=programming" \
  -H "Authorization: Bearer $TOKEN"

# Search by tags
curl -X GET "https://localhost:5001/knowledgeboxes/search?tags=react,javascript" \
  -H "Authorization: Bearer $TOKEN"
```

### Step 5: Get Public Knowledge Boxes
```bash
curl -X GET "https://localhost:5001/knowledgeboxes/public" \
  -H "Authorization: Bearer $TOKEN"
```

## 5. Expected Response Examples

### Successful Knowledge Box Creation
```json
{
  "success": true,
  "message": "Knowledge box created successfully",
  "knowledgeBox": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "title": "Machine Learning Fundamentals",
    "topic": "Artificial Intelligence",
    "content": "Introduction to machine learning concepts...",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z",
    "userId": "test-user-123",
    "isPublic": false,
    "tags": ["machine-learning", "ai", "python", "data-science"],
    "collaborators": []
  }
}
```

### Knowledge Box List Response
```json
{
  "success": true,
  "knowledgeBoxes": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "title": "Machine Learning Fundamentals",
      "topic": "Artificial Intelligence",
      "content": "Introduction to machine learning concepts...",
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": "2024-01-15T10:30:00Z",
      "userId": "test-user-123",
      "isPublic": false,
      "tags": ["machine-learning", "ai", "python"],
      "collaborators": []
    }
  ],
  "totalCount": 1
}
```

### Error Response
```json
{
  "success": false,
  "message": "Knowledge box not found or access denied"
}
```

## 6. Testing with Different Users

To test multi-user scenarios:

### Generate token for User 1
```bash
USER1_TOKEN=$(curl -s -X POST "https://localhost:5001/auth/generate-test-token" \
  -H "Content-Type: application/json" \
  -d '{"userId": "user-1", "email": "user1@example.com"}' | \
  grep -o '"token":"[^"]*' | cut -d'"' -f4)
```

### Generate token for User 2
```bash
USER2_TOKEN=$(curl -s -X POST "https://localhost:5001/auth/generate-test-token" \
  -H "Content-Type: application/json" \
  -d '{"userId": "user-2", "email": "user2@example.com"}' | \
  grep -o '"token":"[^"]*' | cut -d'"' -f4)
```

### Test ownership and access control
```bash
# User 1 creates a knowledge box
curl -X POST "https://localhost:5001/knowledgeboxes" \
  -H "Authorization: Bearer $USER1_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "User 1s Knowledge Box",
    "topic": "Personal Learning",
    "content": "This is user 1s private knowledge box.",
    "isPublic": false,
    "tags": ["personal", "learning"]
  }'

# User 2 tries to access User 1s knowledge boxes (should only see public ones)
curl -X GET "https://localhost:5001/knowledgeboxes" \
  -H "Authorization: Bearer $USER2_TOKEN"
```

## 7. Swagger UI Testing

You can also test the API using the Swagger UI interface:

1. Run the application: `dotnet run`
2. Navigate to: `https://localhost:5001/swagger`
3. Use the "Authorize" button to input your Bearer token
4. Test all endpoints directly from the UI

## Notes

- Replace `YOUR_TOKEN_HERE` with actual JWT tokens from the auth endpoints
- Replace `KNOWLEDGE_BOX_ID` with actual knowledge box IDs returned from create operations
- All timestamps are in ISO 8601 format (UTC)
- Tags are automatically normalized to lowercase and trimmed
- Public knowledge boxes are accessible to all authenticated users
- Private knowledge boxes are only accessible to their owners