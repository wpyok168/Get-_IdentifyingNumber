using HPSocket;
using HPSocket.Sdk;
using HPSocket.Tcp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Get__IdentifyingNumber
{
    
    internal class HpSocketClient
    {
        private readonly ITcpClient _client;
        public event Action<FileResponse> OnReceiveEvent;

        public HpSocketClient()
        {
            _client = new TcpClient();
            _client.OnReceive += _client_OnReceive;
            
        }

        public bool CloseCliend()
        {
           return _client.Stop();
        }

        public bool Connect(string address, ushort port)
        {
            return _client.Connect(address, port);
        }
        private HandleResult _client_OnReceive(IClient sender, byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            var response = JsonSerializer.Deserialize<FileResponse>(json);

            if (response != null)
            {
                Console.WriteLine($"Response: {response.Status}, Message: {response.Message}");

                if (!string.IsNullOrEmpty(response.Data))
                {
                    Console.WriteLine("Received file chunk.");
                    // 可在此处将文件数据写入磁盘
                    long offset = response.Offset;
                    byte[] fileData = Convert.FromBase64String(response.Data);
                    using (FileStream fs = new FileStream(System.Environment.CurrentDirectory + "\\BadIdentifyingNumber.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        fs.Seek(0, SeekOrigin.Begin);
                        fs.WriteAsync(fileData, 0, fileData.Length);
                        OnReceiveEvent?.Invoke(response); // 更新进度
                    }
                }
                OnReceiveEvent?.Invoke(response);
            }

            return HandleResult.Ok;
        }
        public void UploadFile(string filePath, long offset = 0)
        {
            string fileName = Path.GetFileName(filePath);
            byte[] fileData = File.ReadAllBytes(filePath);
            string base64Data = Convert.ToBase64String(fileData);

            var request = new FileRequest
            {
                Command = "upload",
                FileName = fileName,
                Offset = offset,
                Data = base64Data
            };

            SendRequest(request);
        }

        public void DownloadFile(string fileName, long offset = 0)
        {
            var request = new FileRequest
            {
                Command = "download",
                FileName = fileName,
                Offset = offset
            };

            SendRequest(request);
        }

        private void SendRequest(FileRequest request)
        {
            string json = JsonSerializer.Serialize<FileRequest>(request);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            _client.Send(bytes, bytes.Length);
        }
    }
    public class FileRequest
    {
        public string Command { get; set; }
        public string FileName { get; set; }
        public long Offset { get; set; }
        public string Data { get; set; }
    }

    public class FileResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public long Offset { get; set; }
        public string Data { get; set; }
    }
}
