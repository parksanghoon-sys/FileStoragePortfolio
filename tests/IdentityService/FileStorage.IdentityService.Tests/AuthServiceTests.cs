using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using FileStorage.IdentityService.Application.DTOs;
using FileStorage.IdentityService.Domain;
using FileStorage.IdentityService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using FileStorage.Shared;

namespace FileStorage.IdentityService.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IRepository<User>> _mockUserRepository;
        private readonly Mock<IRepository<RefreshToken>> _mockRefreshTokenRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IRepository<User>>();
            _mockRefreshTokenRepository = new Mock<IRepository<RefreshToken>>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Setup configuration mocks
            _mockConfiguration.Setup(c => c["JwtSettings:Secret"]).Returns("YourSuperSecretKeyThatIsAtLeast32CharactersLong");
            _mockConfiguration.Setup(c => c["JwtSettings:Issuer"]).Returns("FileStoragePlatform");
            _mockConfiguration.Setup(c => c["JwtSettings:Audience"]).Returns("FileStorageUsers");
            _mockConfiguration.Setup(c => c["JwtSettings:AccessTokenExpirationMinutes"]).Returns("60");
            _mockConfiguration.Setup(c => c["JwtSettings:RefreshTokenExpirationDays"]).Returns("7");

            _authService = new AuthService(
                _mockConfiguration.Object,
                _mockUserRepository.Object,
                _mockRefreshTokenRepository.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnSuccess_WhenUserDoesNotExist()
        {
            // Arrange
            var registerRequest = new RegisterRequest { Username = "testuser", Email = "test@example.com", Password = "Password123!" };
            _mockUserRepository.Setup(r => r.GetQueryable()).ReturnsAsync(new List<User>().AsQueryable());
            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

            // Act
            var result = await _authService.RegisterAsync(registerRequest);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("testuser", result.Data.Username);
            _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnFailure_WhenUserAlreadyExists()
        {
            // Arrange
            var registerRequest = new RegisterRequest { Username = "testuser", Email = "test@example.com", Password = "Password123!" };
            var existingUser = new User { Id = 1, Email = "test@example.com", Username = "existing", PasswordHash = "hash" };
            _mockUserRepository.Setup(r => r.GetQueryable()).ReturnsAsync(new List<User> { existingUser }.AsQueryable());

            // Act
            var result = await _authService.RegisterAsync(registerRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User with this email already exists.", result.Message);
            _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnSuccess_WithValidCredentials()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "Password123!" };
            var user = new User { Id = 1, Email = "test@example.com", Username = "testuser", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"), RefreshTokens = new List<RefreshToken>() };
            _mockUserRepository.Setup(r => r.GetQueryable()).ReturnsAsync(BuildMock(new List<User> { user }.AsQueryable()));
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data.AccessToken);
            Assert.NotNull(result.Data.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnFailure_WithInvalidCredentials()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "WrongPassword" };
            var user = new User { Id = 1, Email = "test@example.com", Username = "testuser", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!") };
            _mockUserRepository.Setup(r => r.GetQueryable()).ReturnsAsync(BuildMock(new List<User> { user }.AsQueryable()));

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid credentials.", result.Message);
        }

        // Helper to mock IQueryable for EF Core
        // Requires Microsoft.EntityFrameworkCore.InMemory or similar for full mocking
        // For simple LINQ operations, AsQueryable() is often sufficient.
        // For more complex scenarios, consider a dedicated in-memory database or a mocking library for IQueryable.
        // This is a simplified mock for demonstration.
        private IQueryable<T> BuildMock<T>( IQueryable<T> queryable)
        {
            var mock = new Mock<IQueryable<T>>();
            mock.As<IAsyncEnumerable<T>>().Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));
            mock.As<IQueryable<T>>().Setup(x => x.Provider).Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
            mock.As<IQueryable<T>>().Setup(x => x.Expression).Returns(queryable.Expression);
            mock.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(queryable.ElementType);
            mock.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mock.Object;
        }
    }

    // Helper classes for mocking IQueryable with async operations
    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current => _inner.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return new ValueTask();
        }
    }

    public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider).GetMethod(
                nameof(IQueryProvider.Execute),
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { expression.Type },
                null)
                .MakeGenericMethod(expectedResultType)
                .Invoke(_inner, new[] { expression });

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                .MakeGenericMethod(expectedResultType)
                .Invoke(null, new[] { executionResult });
        }
    }

    public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }
    }
}