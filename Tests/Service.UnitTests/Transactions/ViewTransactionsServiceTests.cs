using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyTransaction = BE.src.api.domains.Model.Transaction;

namespace Service.UnitTests.Transactions;
public class ViewTransactionsServiceTests
{
	private readonly Mock<ITransactionRepo> _transactionRepoMock;
	private readonly Mock<IMembershipRepo> _membershipRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly TransactionServ _transactionServ;
	private readonly Guid _userId = Guid.NewGuid();
	public ViewTransactionsServiceTests()
	{
		_transactionRepoMock = new Mock<ITransactionRepo>();
		_membershipRepoMock = new Mock<IMembershipRepo>();
		_cacheServiceMock = new Mock<ICacheService>();
		_transactionServ = new TransactionServ(
			_transactionRepoMock.Object,
			_membershipRepoMock.Object,
			_cacheServiceMock.Object);
	}

	[Fact]
	public async Task ViewTransactionsAsync_Should_ReturnSuccess_WhenGettingTransactionsByUserCacheSuccess()
	{
		// Arrange
		var cacheKey = $"transactions:{_userId}";

		var transactions = new List<MyTransaction>
		{
			new MyTransaction
			{
				MemberUserId = Guid.NewGuid()
			}
		};

		_cacheServiceMock.Setup(cache => cache.Get<List<MyTransaction>>(cacheKey))
			.ReturnsAsync(transactions);

		// Act
		var result = await _transactionServ.ViewTransactions(_userId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(200, jsonResult.StatusCode);
		_cacheServiceMock.Verify(cache => cache.Get<List<MyTransaction>>(cacheKey), Times.Once);
		_transactionRepoMock.Verify(repo => repo.GetTransactions(_userId, default), Times.Never);
		_cacheServiceMock.Verify(cache => cache.Set(cacheKey, transactions, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task ViewTransactionsAsync_Should_ReturnSuccess_WhenGettingTransactionsByUserDatabaseSuccess()
	{
		// Arrange
		var cacheKey = $"transactions:{_userId}";

		var transactions = new List<MyTransaction>
		{
			new MyTransaction
			{
				MemberUserId = Guid.NewGuid()
			}
		};

		_cacheServiceMock.Setup(cache => cache.Get<List<MyTransaction>>(String.Empty))
			.ReturnsAsync(transactions);
		_transactionRepoMock.Setup(repo => repo.GetTransactions(_userId, default))
			.ReturnsAsync(transactions);
		_cacheServiceMock.Setup(cache => cache.Set(cacheKey, transactions, TimeSpan.FromMinutes(10)))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _transactionServ.ViewTransactions(_userId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(200, jsonResult.StatusCode);
		_cacheServiceMock.Verify(cache => cache.Get<List<MyTransaction>>(cacheKey), Times.Once);
		_transactionRepoMock.Verify(repo => repo.GetTransactions(_userId, default), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set(cacheKey, transactions, TimeSpan.FromMinutes(10)), Times.Once);
	}

	[Fact]
	public async Task ViewTransactionsAsync_Should_ThrowException_WhenGettingTransactionsByUserCacheFails()
	{
		// Arrange
		var cacheKey = $"transactions:{_userId}";

		var transactions = new List<MyTransaction>
		{
		};

		_cacheServiceMock.Setup(cache => cache.Get<List<MyTransaction>>(String.Empty))
			.ReturnsAsync(transactions);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _transactionServ.ViewTransactions(_userId));
		_cacheServiceMock.Verify(cache => cache.Get<List<MyTransaction>>(cacheKey), Times.Once);
		_transactionRepoMock.Verify(repo => repo.GetTransactions(_userId, default), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set(cacheKey, transactions, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task ViewTransactionsAsync_Should_ThrowException_WhenGettingTransactionsByUserDatabaseFails()
	{
		// Arrange
		var cacheKey = $"transactions:{_userId}";

		var transactions = new List<MyTransaction>
		{
		};

		_cacheServiceMock.Setup(cache => cache.Get<List<MyTransaction>>(String.Empty))
			.ReturnsAsync(transactions);
		_transactionRepoMock.Setup(repo => repo.GetTransactions(_userId, default))
			.ReturnsAsync(transactions);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _transactionServ.ViewTransactions(_userId));
		_cacheServiceMock.Verify(cache => cache.Get<List<MyTransaction>>(cacheKey), Times.Once);
		_transactionRepoMock.Verify(repo => repo.GetTransactions(_userId, default), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set(cacheKey, transactions, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task ViewTransactionsAsync_Should_ThrowException_WhenSettingTransactionsByUserCacheKeyFails()
	{
		// Arrange
		var cacheKey = $"transactions:{_userId}";

		var transactions = new List<MyTransaction>
		{
			new MyTransaction
			{
				MemberUserId = Guid.NewGuid()
			}
		};

		_cacheServiceMock.Setup(cache => cache.Get<List<MyTransaction>>(String.Empty))
			.ReturnsAsync(transactions);
		_transactionRepoMock.Setup(repo => repo.GetTransactions(_userId, default))
			.ReturnsAsync(transactions);
		_cacheServiceMock.Setup(cache => cache.Set(cacheKey, transactions, TimeSpan.FromMinutes(10)))
			.Throws(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _transactionServ.ViewTransactions(_userId));
		_cacheServiceMock.Verify(cache => cache.Get<List<MyTransaction>>(cacheKey), Times.Once);
		_transactionRepoMock.Verify(repo => repo.GetTransactions(_userId, default), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set(cacheKey, transactions, TimeSpan.FromMinutes(10)), Times.Once);
	}
}

