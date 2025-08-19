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

        // Teste: PurchaseValue inválido (menor ou igual a zero) deve lançar ArgumentOutOfRangeException
        [Fact]
        public async Task CanPurchase_ThrowsException_ForInvalidPurchaseValue()
        {
            // ARRANGE
            var customerId = 1;
            var purchaseValue = 0m; // Valor de compra inválido

            // ACT & ASSERT
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _customerService.CanPurchase(customerId, purchaseValue));
        }

        // Teste: Cliente não existente deve lançar InvalidOperationException
        [Fact]
        public async Task CanPurchase_ThrowsException_ForNonExistentCustomer()
        {
            // ARRANGE
            var customerId = 999; // ID de cliente que não existe
            var purchaseValue = 100m;

            // ACT & ASSERT
            await Assert.ThrowsAsync<InvalidOperationException>(() => _customerService.CanPurchase(customerId, purchaseValue));
        }
    }
}
