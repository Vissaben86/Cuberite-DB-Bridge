using System;
using System.Net;
using System.Net.Sockets;
using MySql.Data.MySqlClient;

namespace Cuberite_DB_Bridge
{
    class Program
    {
        static private Int32 port = 13000;
        static private IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        static private Byte[] bytes = new Byte[256];
        static private String data = null;
        static private TcpListener server = null;
        static private string lServer;
        static private string lUid;
        static private string lPassword;
        static private string lDatabase;

        static void Main(string[] args)
        {

            lServer = args[0];
            lUid = args[1];
            lPassword = args[2];
            lDatabase = args[3];
            
            try
            {
                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Enter the listening loop.
                listen();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        static void listen()
        {
            while (true)
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also user server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                data = null;

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);

                    // Process the data sent by the client.
                    //data = data.ToUpper();

                    if (data == "Close Connection")
                    {
                        client.Client.Disconnect(true);
                        listen();
                    }
                    else
                    {
                        data = mysqlExecute(data);
                    }

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine("Sent: {0}", data);
                }

                // Shutdown and end connection
                client.Close();
            }
        }

        static private string mysqlExecute(string data)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString;
            myConnectionString = "server=" + lServer + ";uid=" + lUid +
                ";pwd=" + lPassword + ";database=" + lDatabase + ";";
            string result = "";
            conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString);

            try
            {
                conn.Open();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(data, conn);

                    object qresult = cmd.ExecuteScalar();
                    if (qresult != null)
                    {
                        conn.Close();
                        return qresult.ToString();
                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    conn.Close();
                    return ex.Message;
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        result = "Cannot connect to server.  Contact administrator";
                        conn.Close();
                        return result;
                    case 1045:
                        result = "Invalid username/password, please try again";
                        conn.Close();
                        return result;
                }
            }


            conn.Close();
            return result;
        }

    }
}