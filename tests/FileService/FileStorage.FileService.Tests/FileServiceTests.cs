using Xunit;
using Moq;
using System.Threading.Tasks;
using FileStorage.FileService.Application.Interfaces;
using FileStorage.FileService.Application.DTOs;
using FileStorage.FileService.Domain;
using FileStorage.FileService.Infrastructure.Services;
using FileStorage.Shared;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;
using FileStorage.FileService.Infrastructure.Repositories;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;

namespace FileStorage.FileService.Tests
{
    public class FileServiceTests
    {
        private readonly Mock<IRepository<FileEntity>> _mockFileRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly FileService _fileService;

        public FileServiceTests()
        {
            _mockFileRepository = new Mock<IRepository<FileEntity>>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(c => c["UploadPath"]).Returns(Path.Combine(Path.GetTempPath(), "uploads_test"));

            _fileService = new FileService(
                _mockConfiguration.Object,
                _mockFileRepository.Object
            );
        }

        [Fact]
        public async Task UploadFilesAsync_ShouldReturnSuccess_WhenFilesAreValid()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.txt");
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.ContentType).Returns("text/plain");
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var files = new FormFileCollection { mockFile.Object };
            var userId = 1;

            _mockFileRepository.Setup(r => r.AddAsync(It.IsAny<FileEntity>()))
                               .ReturnsAsync((FileEntity fe) => { fe.Id = 1; return fe; });

            // Act
            var result = await _fileService.UploadFilesAsync(files, userId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data.Files);
            _mockFileRepository.Verify(r => r.AddAsync(It.IsAny<FileEntity>()), Times.Once);
        }

        [Fact]
        public async Task DownloadFileAsync_ShouldReturnSuccess_WhenFileExistsAndAuthorized()
        {
            // Arrange
            var fileId = 1;
            var userId = 1;
            var filePath = Path.Combine(Path.GetTempPath(), "uploads_test", Guid.NewGuid().ToString() + ".txt");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            await File.WriteAllTextAsync(filePath, "Test content");

            var fileEntity = new FileEntity { Id = fileId, UserId = userId, FilePath = filePath };
            _mockFileRepository.Setup(r => r.GetByIdAsync(fileId)).ReturnsAsync(fileEntity);

            // Act
            var result = await _fileService.DownloadFileAsync(fileId, userId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.IsType<FileStream>(result.Data);

            // Clean up
            File.Delete(filePath);
        }

        [Fact]
        public async Task DeleteFileAsync_ShouldReturnSuccess_WhenFileExistsAndAuthorized()
        {
            // Arrange
            var fileId = 1;
            var userId = 1;
            var filePath = Path.Combine(Path.GetTempPath(), "uploads_test", Guid.NewGuid().ToString() + ".txt");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            await File.WriteAllTextAsync(filePath, "Test content");

            var fileEntity = new FileEntity { Id = fileId, UserId = userId, FilePath = filePath };
            _mockFileRepository.Setup(r => r.GetByIdAsync(fileId)).ReturnsAsync(fileEntity);
            _mockFileRepository.Setup(r => r.DeleteAsync(fileId)).Returns(Task.CompletedTask);

            // Act
            var result = await _fileService.DeleteFileAsync(fileId, userId);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.Data);
            _mockFileRepository.Verify(r => r.DeleteAsync(fileId), Times.Once);
            Assert.False(File.Exists(filePath));
        }

        // Helper to mock IQueryable for EF Core
        private static IQueryable<T> BuildMock<T>(this IQueryable<T> queryable)
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

    // Helper classes for mocking IQueryable with async operations (from IdentityService.Tests)
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