using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoAPI.API.Models;
using ToDoAPI.DATA.EF;
using System.Web.Http.Cors;

namespace ToDoAPI.API.Controllers
{
    [EnableCors(origins:"*", headers:"*", methods:"*")]
    public class ToDoController : ApiController
    {
        ToDoEntities db = new ToDoEntities();

        //GET - api/todo
        public IHttpActionResult GetToDos()
        {
            List<ToDoViewModel> todos = db.ToDoItems.Include("Category").Select(t => new ToDoViewModel()
            {
                ToDoId = t.ToDoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId,
                Category = new CategoryViewModel()
                {
                    CategoryId = t.Category.CategoryId,
                    Name = t.Category.Name,
                    Description = t.Category.Description
                }
            }).ToList<ToDoViewModel>();

            if (todos.Count ==0)
            {
                return NotFound();
            }
            return Ok(todos);
        }//end GetToDos()

        //GET - api/todo/id
        public IHttpActionResult GetToDo(int id)
        {
            ToDoViewModel todo = db.ToDoItems.Include("Category").Where(t => t.ToDoId == id).Select(t => new ToDoViewModel()
            {
                ToDoId = t.ToDoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId,
                Category = new CategoryViewModel()
                {
                    CategoryId = t.Category.CategoryId,
                    Name = t.Category.Name,
                    Description = t.Category.Description
                }
            }).FirstOrDefault();

            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }//end GetToDo()

        //POST - api/todo (HttpPost)
        public IHttpActionResult PostToDo(ToDoViewModel todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            ToDoItem newTodo = new ToDoItem()
            {
                Action = todo.Action,
                Done = todo.Done,
                CategoryId = todo.CategoryId
            };

            db.ToDoItems.Add(newTodo);
            db.SaveChanges();

            return Ok(newTodo);
        }//end PostToDo()

        //PUT - api/todo (HttpPut)
        public IHttpActionResult PutToDo(ToDoViewModel todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            ToDoItem existingtodo = db.ToDoItems.Where(t => t.ToDoId == todo.ToDoId).FirstOrDefault();

            if (existingtodo != null)
            {
                existingtodo.ToDoId = todo.ToDoId;
                existingtodo.Action = todo.Action;
                existingtodo.Done = todo.Done;
                existingtodo.CategoryId = todo.CategoryId;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end PutToDo()

        //DELETE - api/todo/id (HttpDelete)
        public IHttpActionResult DeleteToDo(int id)
        {
            ToDoItem todo = db.ToDoItems.Where(t => t.ToDoId == id).FirstOrDefault();

            if (todo != null)
            {
                db.ToDoItems.Remove(todo);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end DeleteToDo()

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }//end class
}//end namespace
