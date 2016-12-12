using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace ToDo.Objects
{
  public class Category
  {
    public int Id{get; set;}
    public string Name {get; set;}
    public Category(string Name, int Id = 0)
    {
      this.Id = Id;
      this.Name = Name;
    }

    public override bool Equals(System.Object otherCategory)
    {
        if (!(otherCategory is Category))
        {
          return false;
        }
        else
        {
          Category newCategory = (Category) otherCategory;
          bool idEquality = this.Id == newCategory.Id;
          bool nameEquality = this.Name == newCategory.Name;
          return (idEquality && nameEquality);
        }
    }
    public override int GetHashCode()
    {
      return this.Name.GetHashCode();
    }

    public void Save()
   {
     SqlConnection conn = DB.Connection();
     conn.Open();

     SqlCommand cmd = new SqlCommand("INSERT INTO categories (name) OUTPUT INSERTED.id VALUES (@CategoryName);", conn);

     SqlParameter nameParameter = new SqlParameter();
     nameParameter.ParameterName = "@CategoryName";
     nameParameter.Value = this.Name;
     cmd.Parameters.Add(nameParameter);
     SqlDataReader rdr = cmd.ExecuteReader();

     while(rdr.Read())
     {
       this.Id = rdr.GetInt32(0);
     }
     if (rdr != null)
     {
       rdr.Close();
     }
     if(conn != null)
     {
       conn.Close();
     }
   }

    public static List<Category> GetAll()
    {
      List<Category> allCategories = new List<Category>{};
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM categories;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int categoryId = rdr.GetInt32(0);
        string categoryName = rdr.GetString(1);
        Category newCategory = new Category(categoryName, categoryId);
        allCategories.Add(newCategory);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allCategories;
    }

    public void Delete ()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM categories WHERE id = @CategoryID; DELETE FROM categories_tasks WHERE category_id= @CategoryId;", conn);

      SqlParameter categoryIdParameter = new SqlParameter("@CategoryId", this.Id);
      cmd.Parameters.Add(categoryIdParameter);
      cmd.ExecuteNonQuery();

      if(conn!=null)
      {
        conn.Close();
      }
    }
    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM categories; ", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }
    public void AddTask(Task newTask)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO category_tasks (category_id, task_id) VALUES (@CategoryId, @TaskId);", conn);

      SqlParameter categoryId = new SqlParameter();
      categoryId.ParameterName = "@CategoryId";
      categoryId.Value = this.Id;

      SqlParameter taskId = new SqlParameter();
      taskId.ParameterName = "@TaskId";
      taskId.Value = newTask.Id;

      cmd.Parameters.Add(categoryId);
      cmd.Parameters.Add(taskId);

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this.Id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }
    public List<Task> GetTasks()
    {
      List<Task> allTasks = new List<Task>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM category_tasks WHERE category_id = @CategoryId;", conn);
      SqlParameter categoryIdParameter = new SqlParameter();
      categoryIdParameter.ParameterName = "@CategoryId";
      categoryIdParameter.Value = this.Id;
      cmd.Parameters.Add(categoryIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      List<int> taskIds = new List<int> {};
      while(rdr.Read())
      {
        int thisTaskId = rdr.GetInt32(0);
        taskIds.Add(thisTaskId);
        if(rdr != null)
        {
          rdr.Close();
        }
        List<Task> tasks = new List<Task> {};
        foreach (int taskId in taskIds)
        {
          SqlCommand taskQuery = new SqlCommand("SELECT * FROM tasks WHERE id = @TaskId;", conn);
          SqlParameter taskIdParameter = new SqlParameter();
          taskIdParameter.ParameterName = "@TaskId";
          taskIdParameter.Value = taskId;
          taskQuery.Parameters.Add(taskIdParameter);

          SqlDataReader queryReader = taskQuery.ExecuteReader();
          while(queryReader.Read())
          {
            int foundTaskId = queryReader.GetInt32(0);
            string taskDescription = queryReader.GetString(1);
            DateTime taskDueDate = queryReader.GetDateTime(2);
            Task foundTask = new Task(taskDescription, taskDueDate, foundTaskId);
            tasks.Add(foundTask);
          }
          if(queryReader != null)
          {
            queryReader.Close();
          }
        }
        if(conn != null)
        {
          conn.Close();
        }
        return tasks;
      }

      while(rdr.Read())
      {
        int taskId = rdr.GetInt32(0);
        string taskDescription = rdr.GetString(1);
        DateTime taskDueDate = rdr.GetDateTime(2);
        Task newTask = new Task(taskDescription, taskDueDate, taskId);
        allTasks.Add(newTask);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allTasks;
    }

    public static Category Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM categories WHERE id = @CategoryId;", conn);
      SqlParameter categoryIdParameter = new SqlParameter();
      categoryIdParameter.ParameterName = "@CategoryId";
      categoryIdParameter.Value = id.ToString();
      cmd.Parameters.Add(categoryIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundCategoryId = 0;
      string foundCategoryDescription = null;

      while(rdr.Read())
      {
        foundCategoryId = rdr.GetInt32(0);
        foundCategoryDescription = rdr.GetString(1);
      }
      Category foundCategory = new Category(foundCategoryDescription, foundCategoryId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundCategory;
    }
  }
}
