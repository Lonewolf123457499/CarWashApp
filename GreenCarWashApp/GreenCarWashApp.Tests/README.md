# Green Car Wash App - Test Suite

This project contains comprehensive NUnit test cases for all controllers in the Green Car Wash App.

## Test Coverage

### PaymentController Tests
- ✅ TestKeys endpoint validation
- ✅ CreateOrder with valid/invalid requests
- ✅ GenerateTestSignature functionality
- ✅ VerifyPayment with valid/invalid signatures
- ✅ Order status updates after payment verification

### AuthController Tests
- ✅ Customer registration
- ✅ Washer registration  
- ✅ User login functionality
- ✅ Service method invocation verification

### CustomerController Tests
- ✅ Profile management (get/update)
- ✅ Vehicle management (add/get/delete)
- ✅ Order placement and retrieval
- ✅ Rating and receipt functionality

### AdminController Tests
- ✅ Wash package management (CRUD operations)
- ✅ Addon management (CRUD operations)
- ✅ Washer and customer management
- ✅ Order statistics and dashboard data

### WasherController Tests
- ✅ Profile retrieval
- ✅ Order management (accept/start/complete)
- ✅ Pending and assigned order retrieval

## Running Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "ClassName=PaymentControllerTests"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Test Dependencies

- **NUnit 3.13.3** - Testing framework
- **Moq 4.20.69** - Mocking framework
- **Microsoft.EntityFrameworkCore.InMemory** - In-memory database for testing
- **Microsoft.AspNetCore.Mvc.Testing** - ASP.NET Core testing utilities

## Test Structure

Each test class follows the AAA pattern:
- **Arrange** - Set up test data and mocks
- **Act** - Execute the method under test
- **Assert** - Verify the expected outcome

All controllers are tested with mocked dependencies to ensure unit test isolation.