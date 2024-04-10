using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SocketServer : Singleton<SocketServer>
{
    private Socket server;
    private SocketPlayer[] players;
    private Thread processingThread;
    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 11111;
    public bool IsOpen { get; set; }
    public SocketServer() {
        this.players = new SocketPlayer[] { new(true), new(false) };
        this.IsOpen = false;
    }
    public void StartServer() {
        if (this.server == null)
        {
            IPAddress ipAddr = IPAddress.Parse(Host);
            IPEndPoint localEndPoint = new(ipAddr, Port);
            server = new(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            try {
                server.Bind(localEndPoint);
                server.Listen(10);
                this.IsOpen = true;
                Debug.Log("Socket server started, listening on " + Port);

                processingThread = new(MainProcess);
                processingThread.Start();
            } catch (Exception e) {
                Debug.Log("Socket server failed to start: " + e.Message);
            }
        }
    }
    public void StopSocketServer()
    {
        if (server != null)
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].Close();
            }
            processingThread.Abort();
            server.Close();
            server = null;
            this.IsOpen = false;
        }
    }
    void OnApplicationQuit() {
        if (this.IsOpen) {
            StopSocketServer();
            this.IsOpen = false;
            Debug.Log("Socket server stopped");
        }
    }
    public SocketPlayer GetPlayer(bool playerNumber)
    {
        return this.players[playerNumber ? 0 : 1];
    }
    void MainProcess()
    {
        while (true)
        {
            Socket client = server.Accept();
            byte[] data = new byte[4];
            client.Receive(data);
            if (data[0] == 0)
            {
                players[0].SetSocketClient(client);
                Debug.Log("Player 1 connected");
            }
            else if (data[0] == 1)
            {
                players[1].SetSocketClient(client);
                Debug.Log("Player 2 connected");
            }
        }
    }
}
