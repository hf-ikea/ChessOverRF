using System;
using Chess.Core;
using CookComputing.XmlRpc;
using XMLRPC;
using System.Reflection.Emit;

public static class ChessOverRF
{

    public static void Main(string[] args)
    {
        
        IFldigiRPC proxy = (IFldigiRPC)XmlRpcProxyGen.Create(typeof(IFldigiRPC));

        SendMessage("a", proxy);

    }

    public static void SendMessage(string message, IFldigiRPC proxy)
    {
        
        proxy.AddText(message + "^r");

        if(proxy.GetTRState() == "RX")
        {
            proxy.StartTx();
        } else
        {
            throw new Exception("Already transmitting.");
        }
    }

}