
using Microsoft.AspNetCore.Mvc;
using Helsi_TestTask.Models;
using Helsi_TestTask.Models.Abstractions;

namespace Helsi_TestTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksListsController : ControllerBase
    {

        private readonly IDefaultAPI API;

        public TasksListsController(IDefaultAPI api)
        {
            API = api;
        }




        // POST: api/TasksLists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TasksList>> PostTasksList([FromBody] TasksList tasksList, [FromHeader] string userid)
        {
            if (await API.TasksListExists(userid, tasksList.Name))
            {
                return BadRequest($"User <{userid}> already have one or more ToDo Lists with the name <{tasksList.Name}>");
            }

            if (await API.CreateTaskList(userid, tasksList) == true)
                return CreatedAtAction("GetTasksList", new { id = tasksList.Id }, tasksList);

            return BadRequest("Something went wrong");
        }




        [HttpGet]
        public async Task<ActionResult<IEnumerable<TasksList>>> GetTasksLists([FromHeader] string userid, [FromQuery] int? page, [FromQuery] string? orderby)
        {
            var tasklists = await API.GetOwnedTaskLists(userid, page, orderby);
            if (tasklists != null)
                return tasklists;

            return NotFound();
        }




        // GET: api/TasksLists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TasksList>> GetTasksList(long id, [FromHeader] string userid)
        {

            var result = await API.GetTasksList(userid, id);
            if (result != null)
                return result;

            return NotFound();
        }





        // PUT: api/TasksLists/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTasksList(long id, [FromBody] TasksList tasksList, [FromHeader] string userid)
        {
            if (id != tasksList.Id)
            {
                return BadRequest("The IDs do not match");
            }
            var result = await API.UpdateTaskList(userid, id, tasksList);
            if (result == true)
                return NoContent();
            return BadRequest("Item was not found or an error occurred");
        }





        // DELETE: api/TasksLists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTasksList(long id, string username)
        {

            await API.DeleteTaskList(username, id);
            return NotFound();
        }






        

        [HttpGet("Relations/{id}")]
        public async Task<ActionResult<Relation>> GetRecepients(long id, [FromHeader] string userid)
        {
            var result = await API.GetRelation(userid, id);
            if (result != null)
                return result;

            return NotFound();
        }

        [HttpPut("Relations/{id}")]
        public async Task<IActionResult> AddRelation(long id, [FromHeader] string userid, [FromQuery] string newRelation)
        {
            if(await API.AddRecepient(userid, id, newRelation))
                return Ok();
            return BadRequest("Item was not found or an error occurred");
        }

        [HttpDelete("Relations/{id}")]
        public async Task<IActionResult> RemoveRelation(long id, [FromHeader] string userid, [FromQuery] string toDeleteRelation)
        {
            if (await API.RemoveRecepient(userid, id, toDeleteRelation))
                return Ok();
            return BadRequest("Item was not found or an error occurred");
        }

    }
}
