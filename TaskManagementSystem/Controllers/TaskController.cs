using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    public class TaskController : Controller
    {
        private readonly IConfiguration _configuration;

        public TaskController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "User");
            }
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            return View(LoadTasks());
            
        }
        public IActionResult Default()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "User");
            }
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            return View();

        }
        public IActionResult View(int id)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "User");
            }
            TaskViewModel taskview = new TaskViewModel();
            if (id>0)
            {
                DataTable dtTask = LoadTasks(taskId: id);
                if (dtTask != null)
                {
                    if (dtTask.Rows.Count > 0)
                    {
                        DataRow drow = dtTask.Rows[0];
                        taskview.Id = (int)drow["id"];
                        taskview.Title = (string)drow["title"];
                        taskview.Description = (string)drow["taskdescription"];
                        taskview.DueDate = Convert.ToDateTime(drow["duedate"]);
                        taskview.Status = (string)drow["taskstatus"];
                        taskview.CreatedBy = (string)drow["createduser"];
                    }
                }
            }
            ViewBag.Title = "New";
            return View(taskview);
        }
        public IActionResult Create(int? id)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "User");
            }
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            TaskMaster task = new TaskMaster();
            task.DueDate = DateTime.Now.Date;
            if (id != null)
            {
                DataTable dtTask = LoadTasks(taskId: id);
                if (dtTask != null)
                {
                    if (dtTask.Rows.Count > 0)
                    {
                        DataRow drow = dtTask.Rows[0];
                        task.Id = (int)drow["id"];
                        task.Title = (string)drow["title"];
                        task.Description = (string)drow["taskdescription"];
                        task.DueDate = Convert.ToDateTime(drow["duedate"]);
                        task.Status = (string)drow["taskstatus"];
                        task.CreatedBy = (int)drow["createdby"];
                    }
                }
                return View(task);
            }
            ViewBag.Title = "New";
            return RedirectToAction("Create", "Task", task);
        }
        public IActionResult Edit(int? id)
        {
            TaskMaster task = new TaskMaster();
            if(id!=null)
            {
                DataTable dtTask = LoadTasks(taskId: id);
                if(dtTask!=null)
                {
                    if(dtTask.Rows.Count>0)
                    {
                        DataRow drow = dtTask.Rows[0];
                        task.Id = (int)drow["id"];
                        task.Title = (string)drow["title"];
                        task.Description = (string)drow["taskdescription"];
                        task.DueDate = (DateTime)drow["duedate"];
                        task.Status = (string)drow["taskstatus"];
                        task.CreatedBy = (int)drow["createdby"];
                    }
                }
            }
            return View(task);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaskMaster task)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetString("UserId") != null)
                {

                    //ViewBag.UserName = HttpContext.Session.GetString("UserName");
                    int userId;
                    string userid = HttpContext.Session.GetString("UserId");
                    int.TryParse(userid, out userId);
                    if (userId > 0)
                    {
                        task.CreatedBy = userId;
                    }
                    else
                    {
                        TempData["msg"] = "alert('Not saved,user not existed');";
                        return View(task);
                    }
                    int saved = SaveTask(task);
                    if (saved > 0)
                    {
                        TempData["msg"] = "alert('Saved');";
                        if (task.Id == 0)
                            return RedirectToAction("Create");
                        else
                            return RedirectToAction("Create",new TaskMaster());
                    }
                    else
                    {
                        return View(task);
                    }
                }
                else
                {
                    TempData["msg"] = "alert('No credential');";
                    return View(task);
                }
            }
            return View();
        }
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "User");
            }
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            int deleted = DeleteTask(id);
            if(deleted>0)
            {
                TempData["msg"] = "alert('Deleted');";
            }
            return RedirectToAction("Index");

        }
        private DataTable LoadTasks(int? taskId=null,int? createdBy = null)
        {
            using (SqlConnection sqlcon = new SqlConnection(_configuration.GetConnectionString("dbconnection")))
            {
                sqlcon.Open();
                SqlCommand sqlcmd = new SqlCommand("GetTasks", sqlcon);
                sqlcmd.CommandType = System.Data.CommandType. StoredProcedure;
                sqlcmd.Parameters.AddWithValue("@id", taskId);
                sqlcmd.Parameters.AddWithValue("@createdby", createdBy);
                DataTable dtUser = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
                da.Fill(dtUser);
                return dtUser;
            }
        }
        
        private int DeleteTask(int taskId)
        {
            using (SqlConnection sqlcon = new SqlConnection(_configuration.GetConnectionString("dbconnection")))
            {
                sqlcon.Open();
                SqlCommand sqlcmd = new SqlCommand("DeleteTask", sqlcon);
                sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;
                sqlcmd.Parameters.AddWithValue("@id", taskId);
                int deleted = sqlcmd.ExecuteNonQuery();
                return deleted;
            }
        }
        private int SaveTask(TaskMaster task)
        {
            using (SqlConnection sqlcon = new SqlConnection(_configuration.GetConnectionString("dbconnection")))
            {
                sqlcon.Open();
                SqlCommand sqlcmd = new SqlCommand("SaveTask", sqlcon);
                sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;
                sqlcmd.Parameters.AddWithValue("@id", task.Id);
                sqlcmd.Parameters.AddWithValue("@title", task.Title);
                sqlcmd.Parameters.AddWithValue("@description", task.Description);
                sqlcmd.Parameters.AddWithValue("@duedate", task.DueDate);
                sqlcmd.Parameters.AddWithValue("@status", task.Status);
                sqlcmd.Parameters.AddWithValue("@createdby", task.CreatedBy);
                int id = (int)sqlcmd.ExecuteScalar();
                return id;
            }
        }
    }
}
