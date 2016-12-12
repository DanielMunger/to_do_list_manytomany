using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ToDo.Objects
{
  public class Task
  {
    public int Id {get; set;}
    public string Description {get; set;}
    public bool Completed {get;set;}
    public  DateTime _dueDate = new DateTime();

    public Task(string description, DateTime dueDate, bool completed = false, int id = 0)
    {
      this.Id = id;
      this.Description = description;
      this.Completed = completed;
      _dueDate = dueDate;
    }
    public DateTime GetDueDate()
    {
      return _dueDate;
    }

    public override bool Equals(System.Object otherTask)
    {
      if (!(otherTask is Task))
      {
        return false;
      }
      else
      {
        Task newTask = (Task) otherTask;
        bool idEquality = this.Id == newTask.Id;
        bool descriptionEquality = (this.Description == newTask.Description);
        bool dueDateEquality = (this.GetDueDate() == newTask.GetDueDate());
        bool completedEquality = (this.Completed == newTask.Completed);
        return (descriptionEquality && idEquality  && dueDateEquality && completedEquality);
      }
    }
    public override int GetHashCode()
    {
      return this.Description.GetHashCode();
    }
    public void TaskCompleted()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("UPDATE tasks SET completed = @Completed OUTPUT INSERTED.completed WHERE id = @TaskId ;", conn);
      SqlParameter taskId = new SqlParameter("@TaskId", this.Id);
      SqlParameter completedParameter = new SqlParameter("@Completed", "true");
      cmd.Parameters.Add(taskId);
      cmd.Parameters.Add(completedParameter);

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this.Completed = rdr.GetBoolean(0);
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
    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO tasks (description, due_date, completed) OUTPUT INSERTED.id VALUES (@TaskDescription, @TaskDueDate, @TaskCompleted);", conn);

      SqlParameter descriptionParameter = new SqlParameter();
      descriptionParameter.ParameterName = "@TaskDescription";
      descriptionParameter.Value = this.Description;

      SqlParameter dueDateParameter = new SqlParameter();
      dueDateParameter.ParameterName = "@TaskDueDate";
      dueDateParameter.Value = this.GetDueDate();

      SqlParameter taskCompleted = new SqlParameter("@TaskCompleted", this.Completed);
      cmd.Parameters.Add(descriptionParameter);
      cmd.Parameters.Add(dueDateParameter);
      cmd.Parameters.Add(taskCompleted);

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
    public void AddCategory(Category newCategory)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("INSERT INTO categories_tasks (category_id, task_id) VALUES (@CategoryId, @TaskId);", conn);
      SqlParameter categoryIdParameter = new SqlParameter("@CategoryId", newCategory.Id);
      SqlParameter taskIdParameter = new SqlParameter("@TaskId", this.Id);
      cmd.Parameters.Add(categoryIdParameter);
      cmd.Parameters.Add(taskIdParameter);

      cmd.ExecuteNonQuery();
      if(conn!=null)
      {
        conn.Close();
      }
    }

    public static Task Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM tasks WHERE id = @TaskId;", conn);
      SqlParameter taskIdParameter = new SqlParameter();
      taskIdParameter.ParameterName = "@TaskId";
      taskIdParameter.Value = id.ToString();
      cmd.Parameters.Add(taskIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundTaskId = 0;
      string foundTaskDescription = null;
      DateTime foundTaskDueDate = new DateTime();
      bool completed = false;
      while(rdr.Read())
      {
        foundTaskId = rdr.GetInt32(0);
        foundTaskDescription = rdr.GetString(1);
        foundTaskDueDate = rdr.GetDateTime(2);
        completed = rdr.GetBoolean(3);
      }
      Task foundTask = new Task(foundTaskDescription, foundTaskDueDate, completed, foundTaskId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return foundTask;
    }

    public static List<Task> GetAll()
    {
      List<Task> allTasks = new List<Task>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM tasks;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int taskId = rdr.GetInt32(0);
        string taskDescription = rdr.GetString(1);
        DateTime dueDate = rdr.GetDateTime(2);
        bool completed = rdr.GetBoolean(3);
        Task newTask = new Task(taskDescription, dueDate, completed, taskId);
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
    public List<Category> GetCategories()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("SELECT * FROM categories JOIN tasks ON (tasks.id = @TaskId);", conn);
      SqlParameter taskIdParameter = new SqlParameter("@TaskId", this.Id);
      cmd.Parameters.Add(taskIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      List<Category> categories = new List<Category>{};

      while(rdr.Read())
      {
        int categoryId = rdr.GetInt32(0);
        string categoryName = rdr.GetString(1);
        Category newCategory = new Category(categoryName, categoryId);
        categories.Add(newCategory);
      }
      return categories;
    }
    public static List<Task> Sort()
    {
      List<Task> allTasks = new List<Task>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM tasks ORDER BY due_date;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int taskId = rdr.GetInt32(0);
        string taskDescription = rdr.GetString(1);
        DateTime dueDate = rdr.GetDateTime(2);
        bool completed = rdr.GetBoolean(3);
        Task newTask = new Task(taskDescription, dueDate, completed, taskId);
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

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM tasks WHERE id = @TaskID; DELETE FROM categories_tasks WHERE task_id = @TaskId", conn);
      SqlParameter taskIdParameter = new SqlParameter("@TaskId", this.Id);
      cmd.Parameters.Add(taskIdParameter);
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
      SqlCommand cmd = new SqlCommand("DELETE FROM tasks;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }
  }
}
