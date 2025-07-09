using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace experiment
{
    public static class StringDataConstants
    {
        public const string STRING_DATA_TYPE = "StringData";
    }
    public class StringDataDecoder : DataDecoder<DecodedData>
    {
        public async override Task<DecodedData> Accept(NetworkStream stream)
        {
            byte[] intBuffer = new byte[4];

            await ReadEnsurely(stream, intBuffer, 0, 4, 100);
            int messageLength = BitConverter.ToInt32(intBuffer, 0);

            byte[] messageBuffer = new byte[messageLength];
            await ReadEnsurely(stream, messageBuffer, 0, messageLength, 100);
            string message = Encoding.UTF8.GetString(messageBuffer);
            return new DecodedData(StringDataConstants.STRING_DATA_TYPE,message);
        }
    }

    public class StringData : SerializableData<string>
    {

        private string message;

        public StringData(string message)
        {
            this.message = message;
        }

        public override string GetName()
        {
            return StringDataConstants.STRING_DATA_TYPE;
        }

        public override byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(message);
        }
    }
}
