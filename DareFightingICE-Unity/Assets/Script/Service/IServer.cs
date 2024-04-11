using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections;
using UnityEngine;

public interface IServer
{
    public bool IsOpen { get; set; }
    public void StartServer();
    public void StopServer();
    public IPlayer GetPlayer(bool playerNumber);
}
