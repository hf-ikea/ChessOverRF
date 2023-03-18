#pragma warning disable CS8602
#pragma warning disable CS8600

using System;
using Chess.Core;
using CookComputing.XmlRpc;
using XMLRPC;
using MessageMaker;
using System.Text;
using System.Text.RegularExpressions;
using Game;
using Chess.Core.Model;
using System.Text.Json;

public static class ChessOverRF
{
    static int pos;

    static Message? currentMsg;

    static Regex? hexRx;
    static Regex? msgSearcher;

    static bool transmitting;

    static bool newMessage;

    static Player? opponent;
    static ChessGame? game;

    public static void Main(string[] args)
    {
        IFldigiRPC proxy = (IFldigiRPC)XmlRpcProxyGen.Create(typeof(IFldigiRPC));
        //proxy.ClearRX();

        Console.Write("Enter your callsign: ");
        string? callsign = Console.ReadLine().ToUpper();

        Console.Write("Are you the host of the game? (y/n): ");
        string host = Console.ReadLine().ToLower();

        pos = 0;
        transmitting = false;
        hexRx = new Regex(@"[a-f0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        msgSearcher = new Regex(@"(ff99fa)([a-f0-9][a-f0-9])+(fa99ff)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        if (host == "y")
        {
            Console.WriteLine("Starting new game...\nTxing init message.");
            byte[] message = new Message("init", callsign, "").toBytes();
            SendMessage(Convert.ToHexString(message), proxy);

            while(true)
            {
                RunLoops(proxy);
                if(newMessage)
                {
                    if(currentMsg.callsign == callsign) continue;
                    newMessage = false;

                    if(currentMsg.type == "join")
                    {
                        opponent = new Player(currentMsg.callsign);
                        opponent.opponentTurn = false;
                        Console.WriteLine(opponent.callsign + " has joined your game.");
                        
                        game = new ChessGame();
                        game.ShowBoard(Console.OpenStandardOutput());
                        ChessMovement moveStruct = MakeMove(game);

                        while(!moveStruct.result.IsSuccess)
                        {
                            moveStruct = MakeMove(game);
                            Console.WriteLine(moveStruct.result.Description);
                        }

                        SendMessage(Convert.ToHexString(new Message("turn", callsign, JsonSerializer.Serialize(moveStruct)).toBytes()), proxy);
                        while(transmitting)
                        {
                            Thread.Sleep(10);
                        }
                    }
                }
            }
        }
        else
        {
            Console.Write("Press enter when you are ready to start rxing.");
            Console.ReadLine();
            // start recieve loop
            while(true)
            {
                RunLoops(proxy);
                if(newMessage)
                {
                    if(currentMsg.callsign == callsign) continue;
                    newMessage = false;

                    if(currentMsg.type == "init")
                    {
                        Console.Write("New game from " + currentMsg.callsign + "!\nWould you like to join? (y/n): ");
                        if(Console.ReadLine().ToLower() == "y")
                        {
                            Console.WriteLine("Joining Game...");
                            SendMessage(Convert.ToHexString(new Message("join", callsign, "").toBytes()), proxy);
                            opponent = new Player(currentMsg.callsign);
                        }
                        else continue;
                    }
                    else if(currentMsg.type == "startGame")
                    {
                        opponent.opponentTurn = true;
                    }
                }
            }
        }
    }

    public static ChessMovement MakeMove(ChessGame game)
    {
        Console.Write("It is your turn.\nEnter the square to move from: ");
        string source = Console.ReadLine();
        while(source.Length != 2)
        {
            Console.Write("Invalid Input.\nEnter the square to move from: ");
            source = Console.ReadLine();
        }
        char fromColumn = source.ToUpper()[0];  
        int fromRow = Convert.ToInt32(source.Substring(1,1));

        Console.Write("Enter the square to move to: ");
        source = Console.ReadLine();
        while(source.Length != 2)
        {
            Console.Write("Invalid Input.\nEnter the square to move to: ");
            source = Console.ReadLine();
        }
        char toColumn = source.ToUpper()[0];  
        int toRow = Convert.ToInt32(source.Substring(1,1));

        ChessMovement returnVal = new ChessMovement();
        returnVal.result = game.Move(fromColumn, fromRow, toColumn, toRow);
        returnVal.fromColumn = fromColumn;
        returnVal.fromRow = fromRow;
        returnVal.toColumn = toColumn;
        returnVal.toRow = toRow;

        return returnVal;
    }

    public static void RunLoops(IFldigiRPC proxy)
    {
        CheckRXState(proxy);
        Thread.Sleep(10);
        MessageSearcher(proxy);
        Thread.Sleep(10);
    }

    public static void MessageSearcher(IFldigiRPC proxy)
    {
        Thread.Sleep(10);
        int curPos = proxy.GetRXLength();
        Thread.Sleep(10);
        string msgList = ConvertHex(Encoding.UTF8.GetString(proxy.GetRXWidget(0, curPos)));
        MatchCollection matches = msgSearcher.Matches(msgList);

        if(matches.Count == 0) return;
        Console.WriteLine(matches[0].Value);
        
        Message message = new Message("", "", "");
        message.fromBytes(StringToByteArray(matches[0].Value));
        currentMsg = message;
        newMessage = true;
        proxy.ClearRX();
        
    }

    public static void CheckRXState(IFldigiRPC proxy)
    {
        if (proxy.GetTRState() == "RX") transmitting = false;
    }

    public static string ConvertHex(string? str)
    {
        string returnStr = "";

        foreach (char c in str)
        {
            if (!hexRx.IsMatch(c.ToString()))
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

        if (!transmitting)
        {
            proxy.StartTx();
        }
        else
        {
            throw new Exception("Already transmitting.");
        }
    }

    public static byte[] StringToByteArray(string hex) {
        return Enumerable.Range(0, hex.Length)
                        .Where(x => x % 2 == 0)
                        .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                        .ToArray();
    }

    public struct ChessMovement
    {
        public MovementResult result;
        public char fromColumn;
        public int fromRow;
        public char toColumn;
        public int toRow;
    }
}