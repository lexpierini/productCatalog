using AutoMapper;
using Microsoft.EntityFrameworkCore;
using productCatalog.Context;
using productCatalog.Controllers;
using productCatalog.DTOs;
using productCatalog.DTOs.Mappings;
using productCatalog.Pagination;
using productCatalog.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace productCatalogxUnitTest
{
    public class CategoriesUnitTestController
    {

        private readonly IMapper mapper;
        private readonly IUnitOfWork repository;

        public static DbContextOptions<AppDbContext> dbContextOptions { get; }

        public static string connectionStrings = "User ID=dev;Password=dev123;Host=localhost;Port=5432;Database=CatalogDB;";

        static CategoriesUnitTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionStrings)
                .Options;
        }

        public CategoriesUnitTestController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            mapper = config.CreateMapper();

            var context = new AppDbContext(dbContextOptions);

            repository = new UnitOfWork(context);
        }

        // Unit test
        // Test the GET method
        [Fact]
        public async void GetCategories_Return_OkResult()
        {
            //Arrange
            var controller = new CategoriesController(repository, mapper);
            CategoriesParameters categoriesParameters = new CategoriesParameters()
            {
                PageNumber = 1,
                PageSize = 10
            };

            //Act
            var data = await controller.GetAll(categoriesParameters);

            //Assert
            Assert.IsType<CategoryDTO>(data.Value.First());
        }

    }
}
