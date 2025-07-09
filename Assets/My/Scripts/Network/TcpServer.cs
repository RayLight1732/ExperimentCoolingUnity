using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


namespace experiment
{
    public abstract class DataDecoder<T>
    {
        public abstract Task<T> Accept(NetworkStream stream);

        public static async Task ReadEnsurely(NetworkStream stream, byte[] buffer, int offset, int count, int timeoutMs)
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int bytesRead = await stream.ReadAsync(buffer, offset + totalRead, count - totalRead);
                if (bytesRead == 0)
                {
                    throw new IOException("The connection was closed by the remote host.");
                }
                totalRead += bytesRead;

            }
        }
    }

    public abstract class SerializableData<T>
    {
        public abstract byte[] ToBytes();

        public abstract string GetName();
    }

    public class TcpServer<T>
    {
        private ConcurrentQueue<T> received = new ConcurrentQueue<T>();
        private DataDecoder<T> parser;
        private TcpListener listener;
        private CancellationTokenSource cts;
        private ConcurrentBag<TcpClient> clients = new ConcurrentBag<TcpClient>();
        public TcpServer(DataDecoder<T> parser)
        {
            this.parser = parser;
        }

        public void StartConnection(IPAddress localAddr, int port)
        {
            if (listener == null)
            {
                cts = new CancellationTokenSource();
                Task.Run(() => StartServerAsync(localAddr, port, cts.Token));
                Debug.Log("start connection");
            }
        }

        private async Task StartServerAsync(IPAddress localAddr, int port, CancellationToken token)
        {
            listener = new TcpListener(localAddr, port);
            listener.Start();
            Debug.Log("Server started.");
            //listener.BeginAcceptSocket(,listener);
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var client = await listener.AcceptTcpClientAsync();
                    if (token.IsCancellationRequested)
                    {
                        client.Close();
                        break;
                    }
                    clients.Add(client);
                    _ = Task.Run(() => HandleClient(client, token));
                }
            }
            catch (ObjectDisposedException) { } // Ignore when listener is stopped
            catch (Exception ex)
            {
                Debug.Log($"Server error: {ex.Message}");
            }
        }

        private async Task HandleClient(TcpClient client, CancellationToken token)
        {
            using NetworkStream stream = client.GetStream();
            Debug.Log($"Client connected: {client.Client.RemoteEndPoint}");
            try
            {
                while (client.Connected && !token.IsCancellationRequested)
                {
                    T data = await parser.Accept(stream);
                    received.Enqueue(data);
                }
            }
            catch (IOException ex)
            {
                Debug.Log($"Client disconnected: {ex.Message}");
            }
            finally
            {
                client.Close();
                Debug.Log("Stream Closed");
            }
        }

        public void CloseConnection()
        {
            if (listener != null)
            {
                cts?.Cancel();
                listener.Stop();
                listener = null;

                foreach (var client in clients)
                {
                    client.Close();
                }
                clients.Clear();

                Debug.Log("Server stopped and all clients disconnected.");
            }
        }

        public bool TryDequeue(out T result)
        {
            return received.TryDequeue(out result);
        }

        public int GetCount()
        {
            return received.Count;
        }


        public void SendDataAll<V>(SerializableData<V> data)
        {
            if (listener == null)
            {
                throw new InvalidOperationException("Server is not running.");
            }

            foreach (var client in clients)
            {
                try
                {
                    if (client.Connected)
                    {
                        NetworkStream stream = client.GetStream();
                        string name = data.GetName();
                        byte[] serialized = data.ToBytes();
                        byte[] sizeHeader = BitConverter.GetBytes(name.Length);
                        byte[] nameHader = Encoding.GetEncoding("UTF-8").GetBytes(name);
                        byte[] sizeHeader2 = BitConverter.GetBytes(serialized.Length);
                        stream.Write(sizeHeader,0,sizeHeader.Length);
                        stream.Write(nameHader,0,nameHader.Length);
                        stream.Write(sizeHeader2,0,sizeHeader2.Length);
                        stream.Write(serialized,0,serialized.Length);
                        stream.Flush();
                    }
                }
                catch (IOException ex)
                {
                    Debug.LogWarning($"Failed to send data to a client: {ex.Message}");
                }
                catch (ObjectDisposedException)
                {
                    // The client may have been closed already; ignore
                }
            }
        }
    }



}

