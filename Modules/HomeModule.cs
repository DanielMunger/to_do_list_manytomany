using Nancy;
using System.Collections.Generic;
using System;
using ToDo.Objects;

namespace ToDo
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] = _ => {
        return View["index.cshtml"];
      };
      // Get["/categories"] = _ => {
      //   List<Category> categoryList = Category.GetAll();
      //   return View["categories.cshtml", categoryList];
      // };
      // Get["/categories/{id}"] = parameters => {
      //   Category selectedCategory = Category.Find(parameters.id);
      //   return View["tasks.cshtml", selectedCategory];
      // };
      // Get["/categories/new"] = _ => {
      //   return View["category_form.cshtml"];
      // };
      // Get["/tasks/new"] = _ => {
      //   return View["task_form.cshtml"];
      // };
      // Post["/categories/new"] = _ => {
      //   Category newCategory = new Category(Request.Form["category-name"]);
      //   newCategory.Save();
      //   List<Category> categoryList = Category.SaveAll();
      //   return View["categories.cshtml", categoryList];
      // };
      // Post["/tasks/new"] = _ => {
      //   return View["tasks.cshtml"];
      // };

    }
  }
}
