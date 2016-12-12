using Xunit;
using System;
using System.Collections.Generic;
using ToDo.Objects;
using System.Data;
using System.Data.SqlClient;

namespace  ToDo
{
  public class CategoryTest : IDisposable
  {
    public CategoryTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=todo_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void GetAll_DatabaseEmptyAtFirst_true()
    {
      //Arrange
      // Category newCategory = new Category(""){};
      // Act
      int result = Category.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Save_SavesToDatabase_true()
    {
      //Arrange
      Category testCategory = new Category("Yardwork");

      //Act
      testCategory.Save();
      List<Category> result = Category.GetAll();
      List<Category> testList = new List<Category>{testCategory};

      //Assert
      Assert.Equal(testList, result);
    }
    // [Fact]
    // {
    //
    // }
    [Fact]
    public void Equal_AreObjectsEquivalent_true()
    {
      //Arrange
      Category firstCategory = new Category("Holidays.");
      Category secondCategory = new Category("Holidays.");
      //Act
      //Assert
      Assert.Equal(firstCategory, secondCategory);
    }
    //
    [Fact]
    public void Find_FindsTaskInDatabase_true()
    {
      //Arrange
      Category testCategory = new Category("Holidays.");
      testCategory.Save();

      //Act
      Category foundCategory = Category.Find(testCategory.Id);

      //Assert
      Assert.Equal(testCategory, foundCategory);
    }
    [Fact]
    public void Delete_DeletesCategoryAssociationsFromDatabase_True()
    {
      DateTime testDate = new DateTime(2009, 4,5);
      Task testTask = new Task("Mow the Lawn", testDate);
      testTask.Save();

      Category testCategory = new Category("House Work");
      testCategory.Save();

      testTask.AddCategory(testCategory);
      testTask.Delete();

      List<Category> result = testTask.GetCategories();
      List<Category> test = new List<Category>{};

      Assert.Equal(result, test);
    }

    public void Dispose()
    {
      Category.DeleteAll();
      Task.DeleteAll();
    }

  }
}
