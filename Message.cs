#pragma warning disable CS8604

using System;
using System.Text;
using System.Collections.Generic;
using MessageMaker;

namespace MessageMaker
{
    public class Message
    {
        public string type;
        public string callsign;
        public string? payload;

        Dictionary<string, byte> typeDict;

        Dictionary<byte, string> lookUp;

        byte callEnd;
        byte payloadEnd;

        public Message(string type, string callsign, string payload)
        {
            this.type = type;
            this.callsign = callsign;
            this.payload = payload;

            typeDict = new Dictionary<string, byte>();
            typeDict.Add("init", 0x11);
            typeDict.Add("join", 0x22);
            typeDict.Add("startGame", 0x33);

            lookUp = new Dictionary<byte, string>();
            lookUp.Add(0x11, "init");
            lookUp.Add(0x22, "join");
            lookUp.Add(0x33, "startGame");

            callEnd = 0xAF;
            payloadEnd = 0xAE;
        }

        public byte[] convertToUTF8(string toUTF)
        {
            return Encoding.ASCII.GetBytes(toUTF);
        }

        public Message fromBytes(byte[] bytes)
        {
            byte[] decodedBytes = ReedSolomon.Decode(bytes.ToList<byte>());
            int callEndByte = Array.IndexOf(decodedBytes, callEnd);

            this.type = lookUp[decodedBytes[4]];
            this.callsign = Encoding.UTF8.GetString(new ArraySegment<byte>(decodedBytes, 6, callEndByte - 6).ToArray());
            // decode payload if it exists
            if (this.type == "init" || this.type == "join" || this.type == "ack") this.payload = null;
            else
            {
                this.payload = Encoding.UTF8.GetString(new ArraySegment<byte>(decodedBytes, callEndByte, Array.IndexOf(decodedBytes, payloadEnd) - callEndByte).ToArray());
            }

            return this;
        }

        public byte[] toBytes()
        {
            List<byte> byteArray = new List<byte>();

            // add header
            byteArray.Add(0xFF);
            byteArray.Add(0x99);
            byteArray.Add(0xFA);
            byteArray.Add(0x00);

            // add type
            byteArray.Add(typeDict[type]);
            byteArray.Add(0x00);

            // add callsign
            foreach (byte byteInString in convertToUTF8(callsign))
            {
                byteArray.Add(byteInString);
            }
            byteArray.Add(callEnd);

            // check if payload isnt null
            if (payload != "")
            {
                // add payload
                foreach (byte byteInString in convertToUTF8(payload))
                {
                    byteArray.Add(byteInString);
                }
            }
            byteArray.Add(payloadEnd);

            List<byte> list = ReedSolomon.Encode(byteArray).ToList();

            list.Add(0x00);
            list.Add(0xFA);
            list.Add(0x99);
            list.Add(0xFF);

            return list.ToArray();
        }
    }
}