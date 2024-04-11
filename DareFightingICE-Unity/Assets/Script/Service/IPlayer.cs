using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;

public interface IPlayer : IAIInterface
{
    public bool IsCancelled { get; set; }
    public bool PlayerNumber { get; set; }
}
