using BusinessLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProjrctC_.Controller
{
    [Route("api/MetaphorApi")]
    [ApiController]
    public class MetaphorApiController : ControllerBase
    {
        /// <summary>
        /// Retrieves a metaphor by its ID.
        /// </summary>
        /// <param name="ID">The unique identifier of the metaphor.</param>
        /// <returns>Returns the metaphor if found, otherwise returns an error.</returns>
        [HttpGet("GetMetaphor/{ID}", Name = "GetMetaphor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MetaphorDTO> GetMetaphorByID(int ID)
        {
            if (ID < 1) { return BadRequest($"This ID : {ID} Bad :("); }

            clsMetaphor Metaphor = clsMetaphor.GetMetaphortByID(ID);
            if (Metaphor == null) { return NotFound($"This ID : {ID} Not Found :("); }

            MetaphorDTO MDTO = Metaphor.MDTO;

            return Ok(MDTO);
        }

        //================================================//

        /// <summary>
        /// Retrieves all metaphors.
        /// </summary>
        /// <returns>Returns a list of all metaphors or an error if no metaphors exist.</returns>
        [HttpGet("GetAllMetaphors", Name = "GetAllMetaphors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<MetaphorDTO>> GetAllMetaphors()
        {
            List<MetaphorDTO> Metaphor = clsMetaphor.GetAllMetaphors();
            if (Metaphor.Count == 0) { return NotFound("There Is Not Metaphor !"); }

            return Ok(Metaphor);
        }

        //===============================================//

        /// <summary>
        /// Adds a new metaphor.
        /// </summary>
        /// <param name="MetaphorInfo">The metaphor information to be added.</param>
        /// <returns>Returns the created metaphor or an error if unsuccessful.</returns>
        [HttpPost("AddMetaphor", Name = "AddMetaphor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<MetaphorDTO> AddMetaphor(MetaphorDTO MetaphorInfo)
        {
            if (!(DABook.BookExist(MetaphorInfo.BookID) && DAClint.ClintExist(MetaphorInfo.ClintID)))
            {
                return BadRequest("Not Found Book Or Not Found Clint");
            }

            clsBook Book = clsBook.GetBookByID(MetaphorInfo.BookID);

            if (!Book.Available) { return NotFound("This Book Not Available !"); }

            MetaphorInfo.Price = Book.Price;

            clsMetaphor Metaphor = new clsMetaphor(MetaphorInfo);

            if (!Metaphor.Save())
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                new
                {
                    Message = "An Internal Error Occurred , Please Try Again Later :)"
                });
            }

            MetaphorInfo.Id = Metaphor.Id;

            clsBook.AvailableBook(Metaphor.BookID, false);

            return MetaphorInfo.Id > 0 ? CreatedAtRoute("GetClint", new { Id = MetaphorInfo.Id }, MetaphorInfo) :
                BadRequest("Added Unsuccessful");
        }

        //===============================================//

        /// <summary>
        /// Updates the return date for a metaphor.
        /// </summary>
        /// <param name="ID">The ID of the metaphor to update.</param>
        /// <param name="DateReturn">The new return date.</param>
        /// <returns>Returns success or an error message.</returns>
        [HttpPut("UpdateMetaphor/{ID}/{DateReturn}", Name = "UpdateMetaphor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateMetaphorint(int ID, DateTime DateReturn)
        {
            if (ID < 1) { return BadRequest($"This ID : {ID} Bad :("); }

            clsMetaphor Metaphor = clsMetaphor.GetMetaphortByID(ID);

            if (Metaphor == null)
            {
                return NotFound($"This ID : {ID} Not Found :(");
            }

            if (DateReturn < new DateTime(1753, 1, 1) || DateReturn > new DateTime(9999, 12, 31))
            {
                return BadRequest($"DateReturn value: {DateReturn} is out of range.");
            }

            Metaphor.Id = ID;
            Metaphor.DateReturn = DateReturn;

            if (!Metaphor.Save(DateReturn))
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                new
                {
                    Message = "An Internal Error Occurred , Please Try Again Later :)"
                });
            }

            clsBook.AvailableBook(Metaphor.BookID, true);

            return Ok("Update Successfully");
        }

        //===============================================//

        /// <summary>
        /// Deletes a metaphor by its ID.
        /// </summary>
        /// <param name="ID">The ID of the metaphor to delete.</param>
        /// <returns>Returns success or an error message.</returns>
        [HttpDelete("DeleteMetaphor/{ID}", Name = "DeleteMetaphor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteMetaphor(int ID)
        {
            if (ID < 1) { return BadRequest($"This ID : {ID} Bad :("); }

            if (!clsMetaphor.MetaphorExists(ID))
            {
                return NotFound($"This ID : {ID} Not Found :(");
            }

            clsMetaphor.DeleteAllByBookID(ID);
            clsMetaphor.Delete(ID);

            return Ok("Delete Successfully");
        }

        //===============================================//

        /// <summary>
        /// Deletes all metaphors from the database.
        /// </summary>
        /// <returns>Returns success or an error message.</returns>
        [HttpDelete("DeleteAllMetaphors", Name = "DeleteAllMetaphors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteAllBook()
        {
            if (!clsMetaphor.DeleteAll())
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                new
                {
                    Message = "An Internal Error Occurred , Please Try Again Later :)"
                });
            }
            return Ok("Delete Successfully");
        }
    }
}
