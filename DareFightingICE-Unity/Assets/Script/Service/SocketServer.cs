using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketServer : Singleton<SocketServer>
{
    private Socket server;
    public void StartServer() {
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint localEndPoint = new(ipAddr, 12345);
        
        server = new(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        server.Bind(localEndPoint);
        server.Listen(10);

        Debug.Log("Waiting connection ... ");
    }
}
