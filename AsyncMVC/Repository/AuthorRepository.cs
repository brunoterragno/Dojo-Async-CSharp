using AsyncMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncMVC.Repository
{
    public class AuthorRepository
    {
        private const string connStr = @"Server=.\SQLEXPRESS;Database=DojoAsync;Integrated Security=SSPI; Asynchronous Processing=true";

        public IEnumerable<Author> GetAuthors()
        {
            var authors = new List<Author>();
            using (var conn = new SqlConnection(connStr))
            {
                using (var cmd = new SqlCommand("GetAuthors", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            authors.Add(new Author
                                {
                                    FirstName = (string)reader["FirstName"],
                                    LastName = (string)reader["LastName"]
                                });
                        }
                    }
                }
            }

            return authors;
        }

        public void GetAuthorsEAP()
        {
            var syncCtx = SynchronizationContext.Current;

            var conn = new SqlConnection(connStr);
            var cmd = new SqlCommand("GetAuthors", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();

            cmd.BeginExecuteReader(iar =>
                {
                    try
                    {
                        using (SqlDataReader reader = cmd.EndExecuteReader(iar))
                        {
                            var authors = new List<Author>();
                            while (reader.Read())
                            {
                                authors.Add(new Author
                                    {
                                        FirstName = (string)reader["FirstName"],
                                        LastName = (string)reader["LastName"]
                                    });
                            }

                            var args = new GetAuthorsCompletedEventArgs(authors);

                            syncCtx.Post(_ =>
                                {
                                    GetAuthorsCompleted(this, args);
                                }, null);
                        }
                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Dispose();
                    }
                }, null);
        }

        public event EventHandler<GetAuthorsCompletedEventArgs> GetAuthorsCompleted = delegate { };

        public async Task<IEnumerable<Author>> GetAuthorsAsync()
        {
            var authors = new List<Author>();
            using (var conn = new SqlConnection(connStr))
            {
                using (var cmd = new SqlCommand("GetAuthors", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            authors.Add(new Author
                            {
                                FirstName = (string)reader["FirstName"],
                                LastName = (string)reader["LastName"]
                            });
                        }
                    }
                }
            }

            return authors;
        }
    }

    public class GetAuthorsCompletedEventArgs : EventArgs
    {
        public GetAuthorsCompletedEventArgs(IEnumerable<Author> authors)
        {
            Authors = authors;
        }
        public IEnumerable<Author> Authors { get; private set; }
    }
}