using BusinessLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProjectC_.Controller
{
    [Route("api/LibraryProjectC_")]
    [ApiController]
    public class BookApiController : ControllerBase
    {
        /// <summary>
        /// Gets a specific book by its ID.
        /// </summary>
        /// <param name="ID">The ID of the book.</param>
        /// <returns>Book details if found, or an appropriate error response.</returns>
        [HttpGet("GetBook/{ID}", Name = "GetBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BookDTO> GetBookByID(int ID)
        {
            if (ID < 1) { return BadRequest($"This ID : {ID} Bad :("); }

            clsBook Book = clsBook.GetBookByID(ID);
            if (Book == null) { return NotFound($"This ID : {ID} Not Found :("); }

            BookDTO BDTO = Book.BDTO;

            return Ok(BDTO);
        }

        /// <summary>
        /// Gets all books in the system.
        /// </summary>
        /// <returns>A list of all books or an appropriate error response if no books are found.</returns>
        [HttpGet("GetAllBooks", Name = "GetAllBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<BookDTO>> GetAllBooks()
        {
            List<BookDTO> Books = clsBook.GetAllBooks();
            if (Books.Count == 0) { return NotFound("There Is Not Books !"); }

            return Ok(Books);
        }

        /// <summary>
        /// Gets all available books.
        /// </summary>
        /// <returns>A list of available books or an appropriate error response if no books are found.</returns>
        [HttpGet("GetAllAvailableBooks", Name = "GetAllAvailableBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<BookDTO>> GetAllAvailableBooks()
        {
            List<BookDTO> Books = clsBook.GetAllAvailableBooks();
            if (Books.Count == 0) { return NotFound("There Is Not Available Books !"); }

            return Ok(Books);
        }

        /// <summary>
        /// Gets all unavailable books.
        /// </summary>
        /// <returns>A list of unavailable books or an appropriate error response if no books are found.</returns>
        [HttpGet("GetAllNotAvailableBooks", Name = "GetAllNotAvailableBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<BookDTO>> GetAllNotAvailableBooks()
        {
            List<BookDTO> Books = clsBook.GetAllNotAvailableBooks();
            if (Books.Count == 0) { return NotFound("There Is Not Available Books !"); }

            return Ok(Books);
        }

        /// <summary>
        /// Adds a new book to the system.
        /// </summary>
        /// <param name="BookInfo">The details of the book to add.</param>
        /// <returns>The created book or an error response if the ISBN already exists.</returns>
        [HttpPost("AddBook", Name = "AddBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<BookDTO> AddBook(BookDTO BookInfo)
        {
            if (clsBook.ISBNExist(BookInfo.ISBN)) { return BadRequest($"This ISBN : {BookInfo.ISBN} Already Exists !"); }

            clsBook Book = new clsBook(BookInfo);

            if (!Book.Save())
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                new
                {
                    Message = "An Internal Error Occurred , Please Try Again Later :)"
                });
            }

            BookInfo.Id = Book.Id;

            return CreatedAtRoute("GetBook", new { Id = BookInfo.Id }, BookInfo);
        }

        /// <summary>
        /// Updates the details of a specific book.
        /// </summary>
        /// <param name="ID">The ID of the book to update.</param>
        /// <param name="BookInfo">The updated book details.</param>
        /// <returns>A success message or an appropriate error response.</returns>
        [HttpPut("UpdateBook/{ID}", Name = "UpdateBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BookDTO> UpdateBook(int ID, BookDTO BookInfo)
        {
            if (ID < 1) { return BadRequest($"This ID : {ID} Bad :("); }

            clsBook Book = clsBook.GetBookByID(ID);

            if (Book == null)
            {
                return NotFound($"This ID : {ID} Not Found :(");
            }

            Book.Id = ID;
            Book.Title = BookInfo.Title;
            Book.AuthorBookName = BookInfo.AuthorBookName;
            Book.PublicationDate = BookInfo.PublicationDate;
            Book.Price = BookInfo.Price;
            Book.Available = BookInfo.Available;

            if (!Book.Save())
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                new
                {
                    Message = "An Internal Error Occurred , Please Try Again Later :)"
                });
            }

            return Ok("Update Successfully");
        }

        /// <summary>
        /// Deletes a specific book by its ID.
        /// </summary>
        /// <param name="ID">The ID of the book to delete.</param>
        /// <returns>A success message or an appropriate error response.</returns>
        [HttpDelete("DeleteBook/{ID}", Name = "DeleteBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BookDTO> DeleteBook(int ID)
        {
            if (ID < 1) { return BadRequest($"This ID : {ID} Bad :("); }

            if (!clsBook.BookExist(ID))
            {
                return NotFound($"This ID : {ID} Not Found :(");
            }
            clsMetaphor.DeleteAllByBookID(ID);
            clsBook.Delete(ID);

            return Ok("Delete Successfully");
        }

        /// <summary>
        /// Deletes all books in the system.
        /// </summary>
        /// <returns>A success message or an error response if the operation fails.</returns>
        [HttpDelete("DeleteAllBook", Name = "DeleteAllBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BookDTO> DeleteAllBook()
        {
            if (!clsMetaphor.DeleteAll() && !clsBook.DeleteAll())
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
