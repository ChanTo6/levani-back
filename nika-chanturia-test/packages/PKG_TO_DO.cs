using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using nika_chanturia_test.Model;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace nika_chanturia_test.packages
{
    public interface IPKG_TO_DO
    {
        Task AddBook(string name, int quantity, string author, decimal price);
        Task<List<Book>> FetchBooks();
        public  Task InsertBooks(BuyBookRequest bookPurchases);
        public  Task<List<Book>> get_purchased_books();

    }
    public class PKG_TO_DO : PKG_BASE, IPKG_TO_DO
    {
        private readonly IConfiguration _configuration;
        public PKG_TO_DO(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public async Task AddBook(string name, int quantity, string author, decimal price)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(Connstr))
                {
                    await conn.OpenAsync();
                    using (OracleCommand cmd = new OracleCommand("chanturia_books_exam.add_book", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                       
                        cmd.Parameters.Add("p_name", OracleDbType.Varchar2).Value = name;
                        cmd.Parameters.Add("p_quantity", OracleDbType.Int32).Value = quantity;
                        cmd.Parameters.Add("p_author", OracleDbType.Varchar2).Value = author;
                        cmd.Parameters.Add("p_price", OracleDbType.Decimal).Value = price;

                      
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine("OracleException: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }


        public async Task<List<Book>> FetchBooks()
        {
            var books = new List<Book>();

            try
            {
                using (OracleConnection conn = new OracleConnection(Connstr))
                {
                    await conn.OpenAsync();
                    using (OracleCommand cmd = new OracleCommand("chanturia_books_exam.fetch_books", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;


                        using (OracleDataReader reader = (OracleDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                books.Add(new Book
                                {
                                    id = reader.GetInt32(0),
                                    name = reader.GetString(1),
                                    quantity = reader.GetInt32(2),
                                    author = reader.GetString(3),
                                    price = reader.GetDecimal(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine("OracleException: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return books;
        }

        public async Task<List<Book>> get_purchased_books()
        {
            var books = new List<Book>();

            try
            {
                using (OracleConnection conn = new OracleConnection(Connstr))
                {
                    await conn.OpenAsync();
                    using (OracleCommand cmd = new OracleCommand("chanturia_books_exam.get_purchased_books", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                   
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = (OracleDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                books.Add(new Book
                                {
                                   
                                    id = reader.GetInt32(0),             
                                    name = reader.GetString(1),          
                                    quantity = reader.GetInt32(3),      
                                    author = reader.GetString(2),       
                                    buyer_name = reader.GetString(4)    
                                });
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine("OracleException: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return books;
        }





        public async Task InsertBooks(BuyBookRequest bookPurchases)
        {
            using (OracleConnection conn = new OracleConnection(Connstr))
            {
                await conn.OpenAsync();
                string jsonData = PrepareBooksJson(bookPurchases);
                string insertProcedureSql = "chanturia_books_exam.buy_book";
                using (OracleCommand cmd = new OracleCommand(insertProcedureSql, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_json_data", OracleDbType.Clob).Value = jsonData;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public string PrepareBooksJson(BuyBookRequest bookPurchases)
        {
            string jsonData = "[" + string.Join(",", bookPurchases.buyBookbookidquantities.Select(bp =>
                $"{{ \"bookId\": {bp.bookid}, \"quantity\": {bp.quantity}, \"buyerName\": \"{bookPurchases.buyerName}\" }}")) + "]";
            return jsonData;
        }






    }




}


