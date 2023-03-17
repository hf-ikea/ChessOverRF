using System;
using System.Text;
using System.Collections.Generic;

namespace MessageMaker {
    public class Message
    {
        string type;
        string callsign;
        string payload;

        Dictionary<string, byte> typeDict;
        
        public Message(string type, string callsign, string payload)
        {
            this.type = type;
            this.callsign = callsign;
            this.payload = payload;

            typeDict = new Dictionary<string, byte>();
            typeDict.Add("init", 0x11);
            typeDict.Add("join", 0x22);
        }

        public byte[] convertToUTF8(string toUTF)
        {
            return Encoding.ASCII.GetBytes(toUTF);
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
            foreach(byte byteInString in convertToUTF8(callsign))
            {
                byteArray.Add(byteInString);
            }

            // add footer 
            byteArray.Add(0x00);
            byteArray.Add(0xFA);
            byteArray.Add(0x99);
            byteArray.Add(0xFF);

            return byteArray.ToArray();
        }
    }
}