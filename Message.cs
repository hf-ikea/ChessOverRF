#pragma warning disable CS8604

using System.Text;

namespace MessageMaker
{
    public class Message
    {
        public string type;
        public string callsign;
        public string? payload;

        readonly Dictionary<string, byte> typeDict;

        readonly Dictionary<byte, string> lookUp;

        readonly byte callEnd;
        readonly byte payloadEnd;

        public Message(string type, string callsign, string payload)
        {
            this.type = type;
            this.callsign = callsign;
            this.payload = payload;

            typeDict = new Dictionary<string, byte>
            {
                { "init", 0x11 },
                { "join", 0x22 },
                { "turn", 0x33 },
                { "win", 0x44 }
            };

            lookUp = new Dictionary<byte, string>
            {
                { 0x11, "init" },
                { 0x22, "join" },
                { 0x33, "turn" },
                { 0x44, "win" }
            };

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

            type = lookUp[decodedBytes[4]];
            callsign = Encoding.UTF8.GetString(new ArraySegment<byte>(decodedBytes, 6, callEndByte - 6).ToArray());
            // decode payload if it exists
            if (type == "init" || type == "join" || type == "win") payload = null;
            else
            {
                payload = Encoding.UTF8.GetString(new ArraySegment<byte>(decodedBytes, callEndByte + 1, Array.IndexOf(decodedBytes, payloadEnd) - callEndByte - 1).ToArray());
            }

            return this;
        }

        public byte[] toBytes()
        {
            List<byte> byteArray = new List<byte>
            {
                // add header
                0xFF,
                0x99,
                0xFA,
                0x00,

                // add type
                typeDict[type],
                0x00
            };

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