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

        // 1. Tüm Görevleri Listele (GET api/Todo)
        [HttpGet]
        public IActionResult GetTasks()
        {
            var tasks = new List<TaskItem>();
            try
            {
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Veritabanı hatası: {ex.Message}");
            }
        }

        // 2. Yeni Görev Ekle (POST api/Todo)
        [HttpPost]
        public IActionResult CreateTask([FromBody] string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return BadRequest("Görev başlığı boş olamaz.");

            try
            {
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Veritabanı hatası: {ex.Message}");
            }
        }

        // 3. Görevi Güncelle (PUT api/Todo/5)
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] TaskItem updatedItem)
        {
            if (updatedItem == null) return BadRequest("Geçersiz veri.");

            try
            {
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
                        if (rowsAffected == 0) return NotFound("Güncellenecek görev bulunamadı!");
                    }
                }
                return Ok(new { message = "Görev başarıyla güncellendi!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Veritabanı hatası: {ex.Message}");
            }
        }

        // 4. Görevi Sil (DELETE api/Todo/5)
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "DELETE FROM tasks WHERE id = @id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        int rowsAffected = command.ExecuteNonQuery();
                        
                        if (rowsAffected == 0) return NotFound("Silinecek görev bulunamadı!");
                    }
                }
                return Ok(new { message = "Görev başarıyla silindi!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Veritabanı hatası: {ex.Message}");
            }
        }
    }
}
