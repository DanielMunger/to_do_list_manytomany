using Xunit;
using System;
using System.Collections.Generic;
using ToDo.Objects;
using System.Data;
using System.Data.SqlClient;

namespace  ToDo
{
  public class TaskTest : IDisposable
  {
    public TaskTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=todo_test;Integrated Security=SSPI;";
    }
    public DateTime testDate = new DateTime(2017, 1, 1);
    [Fact]
    public void Equal_AreObjectsEquivalent_true()
    {
      //Arrange
      Task firstTask = new Task("Buy lettuce.", testDate);
      Task secondTask = new Task("Buy lettuce.", testDate);
      //Act
      //Assert
      Assert.Equal(firstTask, secondTask);
    }
    [Fact]
    public void Completed_MarksTaskCompletedInDB_true()
    {
      Task newTask = new Task("Mow the lawn", testDate);
      newTask.Save();
      newTask.TaskCompleted();
      bool actualBool = newTask.Completed;
      Assert.Equal(true, actualBool);
    }

    [Fact]
    public void GetAll_TaskTableEmptyAtFirst_true()
    {
      //Arrange
      // Act
      int result = Task.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Save_SavesToDatabase_true()
    {
      //Arrange
      Task testTask = new Task("Mow the lawn", testDate, true);
      testTask.Save();
      //Act

      List<Task> result = Task.GetAll();
      List<Task> testList = new List<Task>{testTask};
      //Assert
      Assert.Equal(testList, result);
    }




    [Fact]
    public void Find_FindsTaskInDatabase_true()
    {
      //Arrange
      Task testTask = new Task("Mow the lawn", testDate);
      testTask.Save();

      //Act
      Task foundTask = Task.Find(testTask.Id);

      //Assert
      Assert.Equal(testTask, foundTask);
    }

    [Fact]
    public void Sort_PutsTasksInOrderByAscendingDate_true()
    {
      //Arrange

      DateTime tempDate = new DateTime(2017, 1, 15);
      Task testTask1 = new Task("Mow the lawn", tempDate);
      testTask1.Save();
      tempDate = new DateTime(2017, 1, 5);
      Task testTask3 = new Task("Floss", tempDate);
      testTask3.Save();
      tempDate = new DateTime(2017, 10, 15);
      Task testTask2 = new Task("Mow the lawn", tempDate);
      testTask2.Save();



      List<Task> orderedTasks = new List<Task>{testTask3, testTask1, testTask2};
      //Act
      List<Task> allTasks = Task.Sort();
      //Assert
      Assert.Equal(orderedTasks, allTasks);
    }

    [Fact]
    public void GetCategories_ReturnsAllCategories_True()
    {

      Task testTask = new Task("Mow the Lawn", testDate);
      testTask.Save();

      Category testCategory = new Category("Yard Work");
      testCategory.Save();
      Category testCategory2 = new Category("Chores");
      testCategory2.Save();

      testTask.AddCategory(testCategory);
      testTask.AddCategory(testCategory2);
      List<Category> result = testTask.GetCategories();
      List<Category> test = new List<Category> {testCategory, testCategory2};

      Assert.Equal(result, test);
    }
    [Fact]
    public void Delete_DeletesTaskFromCategory_True()
    {
      Category testCategory = new Category("Home Stuff");
      testCategory.Save();

      Task newTask = new Task("Mow the Lawn", testDate);
      newTask.Save();
      newTask.AddCategory(testCategory);
      newTask.Delete();
      List<Task> result = new List<Task>{};
      List<Task> test = new List<Task>{};

      Assert.Equal(result, test);
    }

    public void Dispose()
    {
      Task.DeleteAll();
      Category.DeleteAll();
    }

  }
}
