#pragma warning disable CS8602

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

        

        Console.Write("Enter your callsign: ");
        string? callsign = Console.ReadLine().ToUpper();

        Console.Write("Press enter when you would like to start the game.");
        Console.ReadLine();

        byte[] message = new Message("init", callsign, "").toBytes();
        proxy.SetModem("125OLIVIA-8");
        SendMessage(Convert.ToHexString(message), proxy);

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