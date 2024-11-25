using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace S7_300_MockingServer_UI
{
    public partial class Form1 : Form
    {

        MaxwellForm maxForm = new MaxwellForm();

        static byte[] response = new byte[]
            {
                /* Headers */

                0x03, 0x00, 0x00, 0xE1,                         // TPKT Header (Length: 225 bytes)
                0x02, 0xF0, 0x80,                               // ISO Header
                0x32, 0x03, 0x00, 0x00,                         // S7Comm Header with Return Code
                0x05, 0x00, 0x00, 0x02, 0x00,                   // Sequence Length + Reserved
                0xCC, 0x00, 0x00, 0x04, 0x01, 0xFF, 0x04, 0x06, // Header Info
                0x40,                                           // Header Reference Data
                

                /* S7 Payload */

                //1   //B                   //0 <- ReadRequest FLAG                                 
                0x31, 0x42, /*byte 27 -> */ 0x30,               // HeartBit, Assy Type, ReadRequest
                0x30, 0x30,                                     // Dummy
                0x30, 0x30,                                     // Work Complete and Work Result 
                0x30, 0x30, 0x30,                               // Dummy

 /*byte 35 -> */0x42, 0x42, 0x34, 0x31,                         // B B 4 1
                0x46, 0x33, 0x4C, 0x41, 0x51, 0x4C,             // F3LAQL
                0x31, 0x32, 0x33,                               // 123
                0x34, 0x35, 0x36,                               // 456
                
                0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x30, 0x30, 0x30, 0x30, 0x30,
                0x30, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

        Thread loopThread;
        private CancellationTokenSource cancellationTokenSource;

        public Form1()
        {
            InitializeComponent();

            cancellationTokenSource = new CancellationTokenSource();
            loopThread = new Thread(() => LoopingThread(cancellationTokenSource.Token));
            loopThread.Start();
        }

        public static void UpdateValue(int addressOffset, char newValue)
        {
            byte newValueChar = (byte)newValue;

            if (addressOffset < 0 || addressOffset >= response.Length)
            {
                MessageBox.Show($"Address offset out of range...");
            }
            response[addressOffset] = newValueChar;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancellationTokenSource.Cancel(); // Signal the thread to stop
            loopThread.Join(); // Wait for the thread to finish
        }

        private static void LoopingThread(CancellationToken token)
        {
            string serverIp = "172.16.100.51";
            int serverPort = 102; // S7Comm standard port

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Disable Nagle's Algorithm
                listener.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

                listener.Bind(localEndPoint);
                listener.Listen(10);
                Debug.WriteLine($"Mock S7-300 server listening on {serverIp}:{serverPort}");

                while (!token.IsCancellationRequested)
                {
                    // Use the Poll method to check for new connections and respect the token cancellation
                    if (listener.Poll(1000, SelectMode.SelectRead))
                    {
                        Socket clientSocket = listener.Accept();


                        Debug.WriteLine("Real S7Comm connection received.");
                        Thread clientThread = new Thread(() => HandleRealConnection(clientSocket));
                        clientThread.Start();

                    }
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine($"Socket error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                listener.Close();
                Debug.WriteLine("Server socket closed.");
            }
        }


        private static void HandlePingConnection(Socket clientSocket)
        {
            try
            {
                byte[] buffer = new byte[1024];

                while (true)
                {
                    int bytesRead = clientSocket.Receive(buffer);
                    if (bytesRead == 0)
                        break;

                    Debug.WriteLine("Ping packet received, sending TCP ACK.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ping connection error: {ex.Message}");
            }
            finally
            {
                clientSocket.Close();
                Debug.WriteLine("Ping connection closed.");
            }
        }

        private static void HandleRealConnection(Socket clientSocket)
        {
            try
            {
                byte[] buffer = new byte[1024];

                // Step 1: Handle COTP Connection Request
                int bytesRead = clientSocket.Receive(buffer);
                Debug.WriteLine("COTP Connection Request received.");

                byte[] cotpAck = CreateCOTPAck();
                clientSocket.Send(cotpAck);
                Debug.WriteLine("Sent COTP Connection Acknowledgment.");

                // Step 2: Handle S7Comm Setup Communication
                bytesRead = clientSocket.Receive(buffer);
                Debug.WriteLine("S7Comm Setup Communication received.");

                byte[] setupAck = CreateS7SetupCommAck();
                clientSocket.Send(setupAck);
                Debug.WriteLine("Sent S7Comm Setup Communication Acknowledgment.");

                // Step 3: Handle further requests
                while ((bytesRead = clientSocket.Receive(buffer)) > 0)
                {
                    Debug.WriteLine("Received Request...");

                    if (IsReadSZLRequest(buffer))
                    {
                        Debug.WriteLine("Read SZL Request identified.");
                        byte[] szlResponse = CreateReadSZLResponse();
                        //Thread.Sleep(30); 
                        clientSocket.Send(szlResponse);
                        Debug.WriteLine("Sent SZL Response.");
                    }
                    else if (IsReadRequest(buffer))
                    {
                        Debug.WriteLine("Read DB180 Request identified.");
                        byte[] readResponse = CreateS7ReadResponse();
                        Thread.Sleep(1000);
                        clientSocket.Send(readResponse);
                        Debug.WriteLine("Sent DB180 Read Response.");
                    }
                    else if (IsWriteRequest(buffer))
                    {
                        Debug.WriteLine("Write DB180 identified.");

                        if (IsReadRequestReset(buffer))
                        {
                            Debug.WriteLine("Write DB180 for ReadRequest RESET identified.");
                            UpdateValue(27, '0');
                        }
                        else
                        {
                            Debug.WriteLine("Write DB180 for HeartBit RESET identified.");
                            //byte[] readResponse = CreateS7ReadResponse();
                            Thread.Sleep(1000);
                            //clientSocket.Send(readResponse);
                            //Debug.WriteLine("Sent DB180 Read Response.");
                        }
                    }
                    else if (IsCPUFunctionRequest(buffer))
                    {
                        Debug.WriteLine("CPU Function Read Request identified.");
                        byte[] cpuResponse = CreateCPUFunctionResponse();
                        clientSocket.Send(cpuResponse);
                        Debug.WriteLine("Sent CPU Function Response.");
                    }
                    else
                    {
                        Debug.WriteLine("Unknown request.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                clientSocket.Close();
                Debug.WriteLine("Client disconnected.");
            }
        }

        private static byte[] CreateCOTPAck()
        {
            return new byte[]
            {
                0x03, 0x00, 0x00, 0x16,  // TPKT Header: Length 22 bytes
                0x11, 0xD0,              // Proper ISO Control COTP PDU
                0x00, 0x01,              // Destination Reference
                0x44, 0x31,              // Source Reference (mocked)
                0x00,                    // Class (Normal Class 0)
                0xC0, 0x01, 0x0A,        // TPDU Param (size negotiation 0A=1024)
                0xC1, 0x02, 0x01, 0x00,  // Source TSAP 0100
                0xC2, 0x02, 0x01, 0x02   // Destination TSAP 0102
            };
        }

        private static byte[] CreateS7SetupCommAck()
        {
            return new byte[]
            {
                0x03, 0x00, 0x00, 0x1B,  // TPKT Header (27 bytes)
                0x02, 0xF0, 0x80,        // ISO Header
                0x32, 0x03,              // S7Comm Header Start
                0x00, 0x00,              // Reserved
                0x04, 0x00, 0x00, 0x08,  // Protocol ID + Reserved Params
                0x00, 0x00, 0x00, 0x00,  // Unknown sequence
                0xF0,                    // Session Parameter
                0x00, 0x00,              // Reserved
                0x01, 0x00, 0x01,        // Status Valid
                0x00, 0xF0               // Footer
            };
        }

        private static bool IsReadSZLRequest(byte[] buffer)
        {
            return buffer.Length > 30 &&  // Ensure sufficient length
                   buffer[8] == 7 &&      // Function Code for Read SZL Request (decimal)
                   buffer[21] == 0xFF &&  // Updated based on real-world packet
                   buffer[22] == 0x09 &&  // Another Identifier (SZL related)
                   buffer[25] == 0x24;    // SZL Header Identifier (decimal for 0x24)
        }

        private static byte[] CreateReadSZLResponse()
        {
            return new byte[]
            {
                0x03, 0x00, 0x00, 0x3D, // TPKT Header (Length: 61 bytes)
                0x02, 0xF0, 0x80,       // ISO Header
                0x32, 0x07, 0x00, 0x00, // S7Comm Header with Return Code
                0x2C, 0x00, 0x00, 0x0C, // Sequence Length + Reserved
                0x00, 0x20,             // Data Length (32 bytes)
                0x00, 0x01,             // SZL ID (0x0001)
                0x12, 0x08, 0x12, 0x84, // Reserved SZL Data
                0x01, 0x0C, 0x00, 0x00, // Additional Fields and Sequence
                0x00, 0x00, 0xFF, 0x00, // Corrected alignment for Data Block Indicators
                0x09, 0x00, 0x1C, 0x04, // Read SZL Block Data Header
                0x24, 0x00, 0x00, 0x00, // Structure Fields SZL
                0x14, 0x00, 0x01, 0x51, // Header Reference Data
                0x44, 0xFF, 0x08, 0x00, // Offset to SZL Type 0x44
                0x00, 0x00, 0x00, 0x00, // Null / Padding
                0x00, 0x00, 0x24, 0x09, // Corrected Padding
                0x16, 0x02, 0x07, 0x47, // Continuation Packet Data Final
                0x15, 0x02              // Footer SZL End Info
            };
        }

        private static bool IsReadRequest(byte[] buffer)
        {
            return buffer.Length > 17 && buffer[17] == 0x04;
        }

        private static bool IsWriteRequest(byte[] buffer)
        {
            return buffer.Length > 17 && buffer[17] == 0x05;
        }

        private static bool IsReadRequestReset(byte[] buffer)
        {
            return buffer[30] == 0x10;
        }

        private static byte[] CreateS7ReadResponse()
        {

            // Check the actual length of the byte array
            Debug.WriteLine($"Response Length Before Padding: {response.Length} bytes");

            // Ensure we have exactly 225 bytes
            if (response.Length < 225)
            {
                int missingBytes = 225 - response.Length;
                byte[] padding = new byte[missingBytes];
                byte[] completeResponse = new byte[225];
                Buffer.BlockCopy(response, 0, completeResponse, 0, response.Length);
                Buffer.BlockCopy(padding, 0, completeResponse, response.Length, missingBytes);
                response = completeResponse;
            }

            Debug.WriteLine($"Response Length After Padding: {response.Length} bytes");

            return response;
        }





        private static bool IsCPUFunctionRequest(byte[] buffer)
        {
            return buffer.Length > 21 && buffer[22] == 0x44; // Updated to match CPU function identifier based on byte 22
        }

        private static byte[] CreateCPUFunctionResponse()
        {
            return new byte[]
            {
                0x03, 0x00, 0x00, 0x3D, // TPKT Header (Length: 61 bytes)
                0x02, 0xF0, 0x80,       // ISO Header
                0x32, 0x07, 0x00, 0x00, // S7Comm Header with Return Code
                0x2C, 0x00, 0x00, 0x0C, // Sequence Length + Reserved
                0x00, 0x20,             // Data Length (32 bytes)
                0x00, 0x01,             // SZL ID (0x0001)
                0x12, 0x08, 0x12, 0x84, // Reserved SZL Data
                0x01, 0x0C, 0x00, 0x00, // Additional Fields and Sequence
                0x00, 0x00, 0xFF, // Corrected alignment for Data Block Indicators
                0x09, 0x00, 0x1C, 0x04, // Read SZL Block Data Header
                0x24, 0x00, 0x00, 0x00, // Structure Fields SZL
                0x14, 0x00, 0x01, 0x51, // Header Reference Data
                0x44, 0xFF, 0x08, 0x00, 0x00, // Offset to SZL Type 0x44
                0x00, 0x00, 0x00, 0x00, // Null / Padding
                0x00, 0x00, 0x24, 0x09, // Corrected Padding
                0x16, 0x02, 0x07, 0x47, // Continuation Packet Data Final
                0x15, 0x02              // Footer SZL End Info
            };
        }

        private void setReadRequestButton_Click(object sender, EventArgs e)
        {
            response[35] = (byte)engineNumberTextBox.Text[0]; //B
            response[36] = (byte)engineNumberTextBox.Text[1]; //B
            response[37] = (byte)engineNumberTextBox.Text[2]; //4
            response[38] = (byte)engineNumberTextBox.Text[3]; //1
            response[39] = (byte)engineNumberTextBox.Text[4]; //F
            response[40] = (byte)engineNumberTextBox.Text[5]; //3
            response[41] = (byte)engineNumberTextBox.Text[6]; //L
            response[42] = (byte)engineNumberTextBox.Text[7]; //A
            response[43] = (byte)engineNumberTextBox.Text[8]; //Q
            response[44] = (byte)engineNumberTextBox.Text[9]; //L
            response[45] = (byte)engineNumberTextBox.Text[10]; //1
            response[46] = (byte)engineNumberTextBox.Text[11]; //2
            response[47] = (byte)engineNumberTextBox.Text[12]; //3
            response[48] = (byte)engineNumberTextBox.Text[13]; //4
            response[49] = (byte)engineNumberTextBox.Text[14]; //5
            response[50] = (byte)engineNumberTextBox.Text[15]; //6

            UpdateValue(27, '1'); // Set ReadRequest to 1 and login the engine
        }

        private void label1_DoubleClick(object sender, EventArgs e)
        {
            maxForm.Show();
        }
    }
}
