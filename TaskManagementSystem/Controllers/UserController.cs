using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public IActionResult Index()
        {
            ViewBag.Title = "User List";
            return View();
        }
        public IActionResult UserRegistration()
        {
            TempData["msg"] = "";
            return View();
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            
            return RedirectToAction("Login", "User");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserRegistration(UserMaster model)
        {
            if(ModelState.IsValid)
            {
                model.Password = Encode(model.Password);
                using (SqlConnection sqlcon = new SqlConnection(_configuration.GetConnectionString("dbconnection")))
                {
                    sqlcon.Open();
                    SqlCommand sqlcmd = new SqlCommand("CreateUser", sqlcon);
                    sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@name", model.UserName);
                    sqlcmd.Parameters.AddWithValue("@password", model.Password);
                    int id = (int)sqlcmd.ExecuteScalar();
                    model.UserId = id;
                    if (model.UserId == -1)
                    {
                        TempData["msg"] = "alert('Duplicate entry');";
                    }
                    else
                    {
                        TempData["msg"] = "alert('Saved');";
                    }
                }
            }
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            model.Password = Encode(model.Password);
            using (SqlConnection sqlcon = new SqlConnection(_configuration.GetConnectionString("dbconnection")))
            {
                sqlcon.Open();
                SqlCommand sqlcmd = new SqlCommand("GetUser", sqlcon);
                sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;
                sqlcmd.Parameters.AddWithValue("@name", model.UserName);
                sqlcmd.Parameters.AddWithValue("@password", model.Password);
                DataTable dtUser = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
                da.Fill(dtUser);
                if (dtUser.Rows.Count > 0)
                {
                    DataRow drow = dtUser.Rows[0];
                    model.Id = (int)drow["id"];
                    //string userjson = JsonConvert.SerializeObject(model);
                    //HttpContext.Session.SetString("currentuser", userjson);
                    HttpContext.Session.SetString("UserId", model.Id.ToString());
                    HttpContext.Session.SetString("UserName", model.UserName);
                    return RedirectToAction("Default", "Task");
                }
                else
                {
                    TempData["msg"] = "alert('Error')";
                }
            }
                return View();
        }
        private string Encode(string encodetext)
        {
            string stringEncode = string.Empty;
            foreach(char letter in encodetext)
            {
                int a = (int)letter;
                a = a + 2;
                stringEncode+=(char)a;
            }
            return stringEncode;
        }
        private string Decode(string encodetext)
        {
            string stringDecode = string.Empty;
            foreach (char letter in encodetext)
            {
                int a = (int)letter;
                a = a - 2;
                stringDecode+=(char)a;
            }
            return stringDecode.ToString();
        }
    }
}
