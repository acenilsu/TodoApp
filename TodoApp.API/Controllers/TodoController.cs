using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TodoApp.API.Controllers
{
    using TodoApp.API;
    using Microsoft.AspNetCore.Mvc;
    using MySql.Data.MySqlClient;

    namespace TodoApp.API.Controllers
    {
       
        [Route("api/Todo")]
        [ApiController]
        public class TodoController : ControllerBase
        {
            private readonly string _connectionString;

            public TodoController(IConfiguration configuration)
            {
                _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            }

            // 1. Tüm Görevleri Listele (GET api/todo)
            [HttpGet]
            public IActionResult GetTasks()
            {
                var tasks = new List<TaskItem>();

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "SELECT * FROM tasks ORDER BY created_at DESC";
                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new TaskItem
                            {
                                Id = reader.GetInt32("id"),
                                Title = reader.GetString("title"),
                                IsCompleted = reader.GetBoolean("is_completed"),
                                CreatedAt = reader.GetDateTime("created_at")
                            });
                        }
                    }
                }
                return Ok(tasks);
            }

            // 2. Yeni Görev Ekle (POST api/todo)
            [HttpPost]
            public IActionResult CreateTask([FromBody] string title)
            {
                if (string.IsNullOrEmpty(title)) return BadRequest("Görev başlığı boş olamaz.");

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "INSERT INTO tasks (title) VALUES (@title)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@title", title);
                        command.ExecuteNonQuery();
                    }
                }
                return Ok(new { message = "Görev başarıyla eklendi!" });
            }
            // 3. Görevi Güncelle (PUT api/todo/id)
            [HttpPut("{id}")]
            public IActionResult UpdateTask(int id, [FromBody] TaskItem updatedItem)
            {
                if (updatedItem == null) return BadRequest("Geçersiz veri.");

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "UPDATE tasks SET title = @title, is_completed = @isCompleted WHERE id = @id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@title", updatedItem.Title);
                        command.Parameters.AddWithValue("@isCompleted", updatedItem.IsCompleted);
                        command.Parameters.AddWithValue("@id", id);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            return NotFound(new { message = "Güncellenmek istenen görev bulunamadı!" });
                        }
                    }
                }

                return Ok(new { message = "Görev başarıyla güncellendi!" });
            }
            // 4. Görevi Sil (DELETE api/todo/id)
            [HttpDelete("{id}")]
            public IActionResult DeleteTask(int id)
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "DELETE FROM tasks WHERE id = @id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            return NotFound(new { message = "Silinmek istenen görev bulunamadı!" });
                        }
                    }
                }

                return Ok(new { message = "Görev başarıyla silindi!" });
            }
        }
    }
}
