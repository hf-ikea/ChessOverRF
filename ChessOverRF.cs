#pragma warning disable CS8602
#pragma warning disable CS8600

using System;
using Chess.Core;
using CookComputing.XmlRpc;
using XMLRPC;
using MessageMaker;
using System.Text;
using System.Text.RegularExpressions;

public static class ChessOverRF
{
    static int pos;

    static string? messages;

    static Regex? hexRx;
    static Regex? msgSearcher;

    public static void Main(string[] args)
    {
        
        IFldigiRPC proxy = (IFldigiRPC)XmlRpcProxyGen.Create(typeof(IFldigiRPC));        

        Console.Write("Enter your callsign: ");
        string? callsign = Console.ReadLine().ToUpper();

        Console.Write("Are you the host of the game? (y/n): ");
        string host = Console.ReadLine();

        pos = 0;
        messages = "";
        hexRx = new Regex(@"/[a-f0-9]/", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        msgSearcher = new Regex(@"/(ff99fa)([a-f0-9][a-f0-9])+(fa99ff)/g", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        if(host.ToLower() == "y")
        {
            Console.WriteLine("Starting new game...\nTxing init message.");
            byte[] message = new Message("init", callsign, "").toBytes();
            Console.WriteLine(string.Join(", ", message));
            Message msg = new Message("", "", "").fromBytes(message);
            Console.WriteLine("Callsign: " + msg.callsign + ", Type: " + msg.type + ", Payload: " + msg.payload);
            SendMessage(Convert.ToHexString(message), proxy);
        }
        else
        {
            // start recieve loop
        }

        

    }

    public static void AddToRxBufferLoop(IFldigiRPC proxy)
    {
        int newPos = proxy.GetRXLength() - pos;
        messages += proxy.GetRXWidget(pos, newPos);
        messages = ConvertHex(messages);
        pos = newPos + pos;
    }

    public static void MessageSearcher(string messages)
    {
        MatchCollection matches = msgSearcher.Matches(messages);
        string packets = "";

        foreach(Match match in matches)
        {
            packets += match.Value;
        }
        messages = messages.Substring(1);
    }

    public static string ConvertHex(string str)
    {
        string returnStr = "";

        foreach(char c in str)
        {
            if(!hexRx.IsMatch(c.ToString()))
            {
                returnStr += "0";
            }
            else
            {
                returnStr += c;
            }
        }

        return returnStr;
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