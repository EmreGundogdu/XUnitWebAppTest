using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Web.Controllers;
using UnitTest.Web.Models;
using UnitTest.Web.Repository;

namespace UnitTest.Test
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _controller;
        private List<Product> _products;

        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockRepo.Object);
            _products = new List<Product>() { new Product { Id = 1, Name = "Kalem", Price = 5, Stock = 50 }, new Product { Id = 1, Name = "Defter", Price = 10, Stock = 100 }, };
        }
        [Fact]
        public async void Index_ActionExecute_ReturnView()
        {
            var result = await _controller.Index(); //ndex metodu viewresult dönüyor
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_products);
            var result = await _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
            Assert.Equal<int>(2, productList.Count());
        }
        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Details(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Fact]
        public async void Details_IdInValid_ReturnNotFound()
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(0)).ReturnsAsync(product);
            var result = await _controller.Details(0);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async void Details_ValidId_ReturnProduct(int productId)
        {
            Product product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Details(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }
        [Fact]
        public void Create_ActionExecute_ReturnView()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async void Create_InValidModelState_ReturnView()
        {
            _controller.ModelState.AddModelError("Name", "Name alanı gereklidir.");
            var result = await _controller.Create(_products.First());
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);
        }
        public async void Create_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Create(_products.First());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Fact]
        public async void CreatePost_ValidModelState_CreateMethodExecute()
        {
            Product newProduct = null;
            _mockRepo.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x => newProduct = x);
            var result = await _controller.Create(_products.First());

            _mockRepo.Verify(x => x.Create(It.IsAny<Product>()), Times.Once);
            Assert.Equal(_products.First().Id, newProduct.Id);
        }
        [Fact]
        public async void CreatePost_InValidModelState_NeverCreateExecute()
        {
            _controller.ModelState.AddModelError("Name", "");
            var result = await _controller.Create(_products.First());
            _mockRepo.Verify(x => x.Create(It.IsAny<Product>()), Times.Never);
        }
        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Edit(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Theory]
        [InlineData(1)]
        public async void Edit_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Edit(productId);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, redirect.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async void Edit_ActionExecutes_ReturnProduct(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Edit(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }
        [Theory]
        [InlineData(1)]
        public void EditPost_IdIsNotEqualProductId_ReturnNotFound(int productId)
        {
            var result = _controller.Edit(2, _products.First(x => x.Id == productId));
            var redirect = Assert.IsType<NotFoundResult>(result);
        }
        [Theory]
        [InlineData(1)]
        public void EditPost_InValidModelState_ReturnView(int productId)
        {
            _controller.ModelState.AddModelError("Name", "");
            var result = _controller.Edit(productId, _products.First(x => x.Id == productId));
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);
        }
        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_ReturnRedirectToAction(int productId)
        {
            var result = _controller.Edit(productId, _products.First(x => x.Id == productId));
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_UpdateMethodExecute(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.Update(product));
            _controller.Edit(productId, product);
            _mockRepo.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
        }
        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _controller.Delete(null);
            var redirect = Assert.IsType<NotFoundResult>(result);
        }
        [Theory]
        [InlineData(0)] // id'si 0 olmayan bir data getirmeye çalıştığımızda view notfound döneceği için test başarılı
        public async void Delete_IdIsNotEqualProductId_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Delete(productId);
            Assert.IsType<NotFoundResult>(result);
        }
        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecutes_ReturnProduct(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Delete(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }
        public async void DeleteConfirmed_ActionExecutes_ReturnRedirectToIndexAction(int productId)
        {
            var result = await _controller.DeleteConfirmed(1);
            Assert.IsType<RedirectToActionResult>(result);
        }
        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecute(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            await _controller.DeleteConfirmed(productId);
            _mockRepo.Verify(x => x.Delete(It.IsAny<Product>()), Times.Once);
        }
        [Theory]
        [InlineData(1)]
        public async void ProductExists_IdIsNotEqualProductId_ReturnFalse(int productId)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = _controller.ProductExists(productId);
            //Assert.IsType<bool>(result);
            Assert.Equal(false, result);
        }
    }
}
