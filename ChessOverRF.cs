using System;
using Chess.Core;
using CookComputing.XmlRpc;
using XMLRPC;
using MessageMaker;
using System.Text;

public static class ChessOverRF
{

    public static void Main(string[] args)
    {
        
        IFldigiRPC proxy = (IFldigiRPC)XmlRpcProxyGen.Create(typeof(IFldigiRPC));

        Message message = new Message("init", "NO6H", "1234");

        byte[] output = message.toBytes();

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

    public static void PrintByteArray(byte[] bytes)
    {
        var sb = new StringBuilder("new byte[] { ");
        foreach (var b in bytes)
        {
            sb.Append(b + ", ");
        }
        sb.Append("}");
        Console.WriteLine(sb.ToString());
    }
}