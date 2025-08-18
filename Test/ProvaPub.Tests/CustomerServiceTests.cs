using Microsoft.EntityFrameworkCore;
using Moq;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;
using Xunit;

namespace ProvaPub.Tests
{
    public class CustomerServiceTests
    {
        private Mock<TestDbContext> _mockDbContext; // Mock do contexto do banco de dados
        private CustomerService _customerService; // Instância do serviço que será testado

        public CustomerServiceTests()
        {
            // Inicializa os mocks e o serviço antes de cada teste para garantir isolamento

            // Cria uma coleção de clientes em memória para simular o DbSet de Clientes
            var customers = new List<Customer>().AsQueryable();
            var mockCustomersDbSet = new Mock<DbSet<Customer>>();

            // Configura o DbSet mockado para se comportar como um IQueryable
            mockCustomersDbSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customers.Provider);
            mockCustomersDbSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customers.Expression);
            mockCustomersDbSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customers.ElementType);
            mockCustomersDbSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

            // Configura o FindAsync para retornar um cliente específico quando procurado pelo ID
            // Nota: FindAsync é um método assíncrono, então precisamos do retorno simulado com Task.FromResult
            mockCustomersDbSet.Setup(m => m.FindAsync(It.IsAny<int>()))
                              .Returns<object[]>(async (ids) => {
                                  var customerId = (int)ids[0];
                                  return await Task.FromResult(customers.FirstOrDefault(c => c.Id == customerId));
                              });


            // Cria uma coleção de pedidos em memória para simular o DbSet de Pedidos
            var orders = new List<Order>().AsQueryable();
            var mockOrdersDbSet = new Mock<DbSet<Order>>();

            // Configura o DbSet mockado para se comportar como um IQueryable
            mockOrdersDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockOrdersDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockOrdersDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockOrdersDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            // Configura o mock do contexto do banco de dados
            _mockDbContext = new Mock<TestDbContext>();
            _mockDbContext.Setup(m => m.Customers).Returns(mockCustomersDbSet.Object);
            _mockDbContext.Setup(m => m.Orders).Returns(mockOrdersDbSet.Object);

            // Cria uma nova instância do CustomerService, injetando o contexto mockado
            _customerService = new CustomerService(_mockDbContext.Object);
        }

        // Teste: CustomerId inválido (menor ou igual a zero) deve lançar ArgumentOutOfRangeException
        [Fact]
        public async Task CanPurchase_ThrowsException_ForInvalidCustomerId()
        {
            // ARRANGE
            var customerId = 0; // Cliente inválido
            var purchaseValue = 100m;

            // ACT & ASSERT
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _customerService.CanPurchase(customerId, purchaseValue));
        }

        //// Teste: PurchaseValue inválido (menor ou igual a zero) deve lançar ArgumentOutOfRangeException
        //[Fact]
        //public async Task CanPurchase_ThrowsException_ForInvalidPurchaseValue()
        //{
        //    // ARRANGE
        //    var customerId = 1;
        //    var purchaseValue = 0m; // Valor de compra inválido

        //    // ACT & ASSERT
        //    await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _customerService.CanPurchase(customerId, purchaseValue));
        //}

        //// Teste: Cliente não existente deve lançar InvalidOperationException
        //[Fact]
        //public async Task CanPurchase_ThrowsException_ForNonExistentCustomer()
        //{
        //    // ARRANGE
        //    var customerId = 999; // ID de cliente que não existe
        //    var purchaseValue = 100m;

        //    // ACT & ASSERT
        //    await Assert.ThrowsAsync<InvalidOperationException>(() => _customerService.CanPurchase(customerId, purchaseValue));
        //}

        //// Teste: Cliente já fez uma compra no último mês
        //[Fact]
        //public async Task CanPurchase_ReturnsFalse_WhenCustomerHasRecentOrder()
        //{
        //    // ARRANGE
        //    var customerId = 1;
        //    var purchaseValue = 50.0m;
        //    var customer = new Customer { Id = customerId }; // Cria um cliente
        //    var orders = new List<Order>
        //    {
        //        // Adiciona um pedido recente (15 dias atrás)
        //        new Order { CustomerId = customerId, OrderDate = DateTime.UtcNow.AddDays(-15) }
        //    }.AsQueryable();

        //    // Configura o mock do DbSet de Clientes para retornar o cliente
        //    _mockDbContext.Setup(m => m.Customers.FindAsync(customerId)).ReturnsAsync(customer);

        //    // Configura o mock do DbSet de Pedidos para retornar os pedidos recentes
        //    var mockOrdersDbSet = new Mock<DbSet<Order>>();
        //    mockOrdersDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
        //    mockOrdersDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
        //    mockOrdersDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
        //    mockOrdersDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());
        //    _mockDbContext.Setup(m => m.Orders).Returns(mockOrdersDbSet.Object);

        //    // ACT
        //    var result = await _customerService.CanPurchase(customerId, purchaseValue);

        //    // ASSERT
        //    Assert.False(result); // Espera que a compra não seja permitida
        //}

        //// Teste: Cliente nunca comprou e o valor da compra é maior que 100
        //[Fact]
        //public async Task CanPurchase_ReturnsFalse_WhenFirstTimeCustomerPurchasesOver100()
        //{
        //    // ARRANGE
        //    var customerId = 2;
        //    var purchaseValue = 150m; // Valor acima de 100
        //    var customer = new Customer { Id = customerId }; // Cliente sem pedidos

        //    // Configura o mock do DbSet de Clientes para retornar o cliente
        //    _mockDbContext.Setup(m => m.Customers.FindAsync(customerId)).ReturnsAsync(customer);

        //    // Garante que o cliente não tem pedidos anteriores para simular "never bought before"
        //    _mockDbContext.Setup(m => m.Orders.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
        //                  .ReturnsAsync(0);

        //    // ACT
        //    var result = await _customerService.CanPurchase(customerId, purchaseValue);

        //    // ASSERT
        //    Assert.False(result); // Espera que a compra não seja permitida
        //}

        //// Teste: Compra válida (todos os requisitos atendidos)
        //[Fact]
        //public async Task CanPurchase_ReturnsTrue_WhenAllConditionsMet()
        //{
        //    // ARRANGE
        //    var customerId = 7;
        //    var purchaseValue = 50m; // Valor válido
        //    var customer = new Customer { Id = customerId }; // Cliente existente, sem pedidos recentes

        //    // Mockar cliente existente
        //    _mockDbContext.Setup(m => m.Customers.FindAsync(customerId)).ReturnsAsync(customer);

        //    // Mockar que não há pedidos recentes
        //    _mockDbContext.Setup(m => m.Orders.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
        //                  .ReturnsAsync(0);

        //    // ACT
        //    var result = await _customerService.CanPurchase(customerId, purchaseValue);

        //    // ASSERT
        //    // Este assert só passará se o teste for executado em horário comercial e dia de semana.
        //    // Para um controle total, o CustomerService precisaria de um 'IDateTimeProvider' injetado.
        //    if (DateTime.UtcNow.Hour >= 8 && DateTime.UtcNow.Hour <= 18 &&
        //        DateTime.UtcNow.DayOfWeek != DayOfWeek.Saturday && DateTime.UtcNow.DayOfWeek != DayOfWeek.Sunday)
        //    {
        //        Assert.True(result); // Espera que a compra seja permitida
        //    }
        //    else
        //    {
        //        // Se o teste rodar fora do horário comercial, ele falharia aqui, pois a regra no serviço é dinâmica.
        //        // Este é um ponto que mostra a necessidade de refatorar o serviço.
        //        Assert.False(result);
        //    }
        //}
    }
}
