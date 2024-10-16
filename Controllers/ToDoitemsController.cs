using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class ToDoitemsController : Controller
    {
        //public string  name { get; set; }
        ApplicationDbContext dbcontext = new ApplicationDbContext();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult create(string name)
        {
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires=DateTime.Now.AddDays(1);
            Response.Cookies.Append("Name",name, cookieOptions);
            return RedirectToAction(nameof(items));
        }
        public IActionResult items()
        {
            var todo = dbcontext.ToDos.ToList();
            return View(todo);
        }
        public IActionResult createnew()
        {
            return View();
        }
        [HttpPost]
        public IActionResult createnew(ToDo todo, IFormFile Details)
        {
            

            if (Details.Length > 0)
            {
                var filename = Guid.NewGuid() + Path.GetExtension(Details.FileName);
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload", filename);
                using (var stream = System.IO.File.Create(filepath))
                {
                    Details.CopyTo(stream);
                }
                todo.Details = filename;
            }

            dbcontext.ToDos.Add(todo);
            dbcontext.SaveChanges();
            TempData["success"] = $"{todo.Title} created successfully";
            return RedirectToAction(nameof(items));
        }

        public IActionResult Delete(int Id)
        {
            var todo = dbcontext.ToDos.Find(Id);
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//upload", todo.Details);
            if (System.IO.File.Exists(filepath)) 
            {
                System.IO.File.Delete(filepath);
            }
            dbcontext.ToDos.Remove(todo);
            dbcontext.SaveChanges();
            TempData["success"] = $" {todo.Title} Deleted successfully";

            return RedirectToAction(nameof(items));
        }
        public IActionResult Edit(int id)
        {
            var todo=dbcontext.ToDos.Find(id);
            return View(todo);
        }
        [HttpPost]
        public IActionResult Edit(ToDo todo , IFormFile Details)
        {
            var olditem= dbcontext.ToDos.AsNoTracking().FirstOrDefault(e=> e.Id == todo.Id);
            if (Details!=null && Details.Length > 0)
            {
                var filename = Guid.NewGuid() + Path.GetExtension(Details.FileName);
                var filepath= Path.Combine(Directory.GetCurrentDirectory(),"wwwroot//upload" ,filename);
                var oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//upload", olditem.Details);
                using (var stream = System.IO.File.Create(filepath))
                {
                    Details.CopyTo(stream);
                }
                todo.Details = filename;

                if (System.IO.File.Exists(oldfilepath))
                {
                    System.IO.File.Delete(oldfilepath);
                }

            }
            else
            {
                todo.Details = olditem.Details;

            }
            dbcontext.ToDos.Update(todo);
            dbcontext.SaveChanges();
            TempData["success"] = $" {todo.Title} Edited successfully";
            return RedirectToAction(nameof(items));
        }

    }
   
}
