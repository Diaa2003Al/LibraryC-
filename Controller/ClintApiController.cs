using BusinessLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProjrctC_.Controller
{
    [Route("api/ClintApi")]
    [ApiController]
    public class ClintApiController : ControllerBase
    {
        /// <summary>
        /// Retrieve a specific client by their ID.
        /// </summary>
        /// <param name="ID">The unique identifier of the client.</param>
        /// <returns>The client's details if found, otherwise an appropriate error response.</returns>
        [HttpGet("GetClint/{ID}", Name = "GetClint")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ClintDTO> GetClintByID(int ID)
        {
            if (ID < 1) { return BadRequest($"This ID : {ID} is invalid."); }

            clsClint Clint = clsClint.GetClintByID(ID);
            if (Clint == null) { return NotFound($"Client with ID: {ID} not found."); }

            ClintDTO CDTO = Clint.CDTO;

            return Ok(CDTO);
        }

        //================================================//

        /// <summary>
        /// Retrieve all clients.
        /// </summary>
        /// <returns>A list of all clients if available, otherwise a 404 response.</returns>
        [HttpGet("GetAllClints", Name = "GetAllClints")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<ClintDTO>> GetAllBooks()
        {
            List<ClintDTO> Clint = clsClint.GetAllClints();
            if (Clint.Count == 0) { return NotFound("No clients found."); }

            return Ok(Clint);
        }

        //===============================================//

        /// <summary>
        /// Add a new client to the system.
        /// </summary>
        /// <param name="ClintInfo">Details of the client to be added.</param>
        /// <returns>The added client's details or an error response if unsuccessful.</returns>
        [HttpPost("AddClint", Name = "AddClint")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ClintDTO> AddBook(ClintDTO ClintInfo)
        {
            if (clsClint.EmailExist(ClintInfo.Email))
            {
                return BadRequest($"The email: {ClintInfo.Email} already exists.");
            }

            clsClint Clint = new clsClint(ClintInfo);

            if (!Clint.Save())
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                new
                {
                    Message = "An internal error occurred. Please try again later."
                });
            }

            ClintInfo.Id = Clint.Id;

            return ClintInfo.Id > 0 ? CreatedAtRoute("GetClint", new { Id = ClintInfo.Id }, ClintInfo) :
                BadRequest("Client could not be added.");
        }

        //===============================================//

        /// <summary>
        /// Update an existing client's details.
        /// </summary>
        /// <param name="ID">The unique identifier of the client.</param>
        /// <param name="ClintInfo">Updated client information.</param>
        /// <returns>A success message or an error response.</returns>
        [HttpPut("UpdateClint/{ID}", Name = "UpdateClint")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateBook(int ID, ClintDTO ClintInfo)
        {
            if (ID < 1) { return BadRequest($"This ID : {ID} is invalid."); }

            clsClint Clint = clsClint.GetClintByID(ID);

            if (Clint == null)
            {
                return NotFound($"Client with ID: {ID} not found.");
            }

            Clint.Id = ID;
            Clint.FirstName = ClintInfo.FirstName;
            Clint.LastName = ClintInfo.LastName;
            Clint.Email = ClintInfo.Email;
            Clint.Phone = ClintInfo.phone;

            if (!Clint.Save())
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                new
                {
                    Message = "An internal error occurred. Please try again later."
                });
            }

            return Ok("Client updated successfully.");
        }

        //===============================================//

        /// <summary>
        /// Delete a specific client by their ID.
        /// </summary>
        /// <param name="ID">The unique identifier of the client to be deleted.</param>
        /// <returns>A success message or an error response.</returns>
        [HttpDelete("DeleteClint/{ID}", Name = "DeleteClint")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteBook(int ID)
        {
            if (ID < 1) { return BadRequest($"This ID : {ID} is invalid."); }

            if (!clsClint.ClintExist(ID))
            {
                return NotFound($"Client with ID: {ID} not found.");
            }

            clsMetaphor.DeleteAllByClintID(ID);
            clsClint.Delete(ID);

            return Ok("Client deleted successfully.");
        }

        //===============================================//

        /// <summary>
        /// Delete all clients from the system.
        /// </summary>
        /// <returns>A success message or an error response if unsuccessful.</returns>
        [HttpDelete("DeleteAllClints", Name = "DeleteAllClints")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteAllBook()
        {
            if (!clsMetaphor.DeleteAll() && !clsClint.DeleteAll())
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                new
                {
                    Message = "An internal error occurred. Please try again later."
                });
            }
            return Ok("All clients deleted successfully.");
        }
    }
}
