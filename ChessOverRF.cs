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

        Console.WriteLine(string.Join(", ", output));

        Console.WriteLine(string.Join(", ", ReedSolomon.Decode(output.ToList())));

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