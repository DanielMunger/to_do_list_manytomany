using Nancy;
using System.Collections.Generic;
using System;
using ToDo.Objects;
using Nancy.ViewEngines.Razor;

namespace ToDo
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] = _ => {
        return View["index.cshtml"];
      };

      Get["/categories"] = _ => {
        List<Category> categoryList = Category.GetAll();
        return View["categories.cshtml", categoryList];
      };

      Get["/categories/{id}"] = parameters => {
        Dictionary<string, object> model = new Dictionary<string, object>();
        var SelectedCategory = Category.Find(parameters.id);
        var CategoryTasks = SelectedCategory.GetTasks();
        model.Add("category", SelectedCategory);
        model.Add("tasks", CategoryTasks);
        return View["category.cshtml", model];
      };

      Get["/categories/new"] = _ => {
        return View["category_form.cshtml"];
      };

      Get["/tasks"] = _ => {
        List<Task> allTasks = Task.GetAll();
        return View["task.cshtml", allTasks];
      };

      Get["/tasks/new"] = _ => {
        return View["task_form.cshtml"];
      };

      Post["/categories/new"] = _ => {
        Category newCategory = new Category(Request.Form["category-name"]);
        newCategory.Save();
        List<Category> categoryList = Category.GetAll();
        return View["categories.cshtml", categoryList];
      };

      Post["/tasks/new"] = _ => {
        return View["tasks.cshtml"];
      };

    }
  }
}
