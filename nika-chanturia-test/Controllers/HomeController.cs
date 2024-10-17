using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nika_chanturia_test.Model;
using nika_chanturia_test.packages;
using System.Xml.Linq;

namespace nika_chanturia_test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IPKG_TO_DO _package;
        IPKG_TO_DO package;

        private IConfiguration _configuration;

        public HomeController(IPKG_TO_DO package, IConfiguration configuration)
        {
            _package = package;
            _configuration = configuration;
        }


        [HttpPost("AddBook")]
        public async Task<IActionResult> AddBook([FromBody] Book newBook)
        {
            await _package.AddBook(newBook.name, newBook.quantity, newBook.author, newBook.price);

            return Ok(new { message = "User created successfully" });
        }

        [HttpGet("FetchBooks")]
        public async Task<IActionResult> FetchBooks()
        {
            var books = await _package.FetchBooks();
            return Ok(books);
        }

        [HttpGet("get_purchased_books")]
        public async Task<IActionResult> get_purchased_books()
        {
            var books = await _package.get_purchased_books();
            return Ok(books);
        }



        [HttpPost("BuyBooks")]
        public async Task<IActionResult> InsertProducts([FromBody] BuyBookRequest bookPurchaseRequest)
        {
            if (bookPurchaseRequest == null || bookPurchaseRequest.buyBookbookidquantities == null || bookPurchaseRequest.buyBookbookidquantities.Count == 0)
            {
                return BadRequest("No books found in JSON data.");
            }

            try
            {
                foreach (var book in bookPurchaseRequest.buyBookbookidquantities)
                {
                    Console.WriteLine($"BookId: {book.bookid}, Quantity: {book.quantity}");
                }

                await _package.InsertBooks(bookPurchaseRequest);

                return Ok("Books inserted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
