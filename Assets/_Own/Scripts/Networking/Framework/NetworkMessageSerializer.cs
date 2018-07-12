using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

/// A helper class for reading and writing INetworkMessage to and from byte[].
public static class NetworkMessageSerializer
{
    private const int MaxMessageSizeBytes = 512 * (1 << 10); // 512 kB
    private const int MessageReadTimeoutMs = 2000;
    // TODO Have these belong to each connection, so that they don't use the same buffers from different threads.
    private static readonly byte[] ReadBuffer  = new byte[MaxMessageSizeBytes];
    private static readonly byte[] WriteBuffer = new byte[MaxMessageSizeBytes];
    const int TypeIndexSize = sizeof(ushort);
    const int MessageSizeSize = sizeof(int);
    
    struct MessageTypeInfo
    {
        public Type type;
        public string name;
    }

    private static readonly MessageTypeInfo[] MessageTypeInfos;
    //private static readonly Dictionary<Type, ushort> MessageTypeIndices = new Dictionary<Type, ushort>();
    private static readonly ConcurrentDictionary<Type, ushort> MessageTypeIndices = new ConcurrentDictionary<Type, ushort>();
    
    static NetworkMessageSerializer()
    {
        var messageTypes = GetMessageTypes();
        
        CheckAllMessageTypesHaveAPublicParameterlessConstructor(messageTypes);
        
        MessageTypeInfos = messageTypes
            .Select(type => new MessageTypeInfo {type = type, name = GetMessageTypeName(type)})
            .ToArray();
        
        int numTypes = MessageTypeInfos.Length;
        for (ushort index = 0; index < numTypes; ++index)
        {
            bool success = MessageTypeIndices.TryAdd(MessageTypeInfos[index].type, index);
            Assert.IsTrue(success);
        }
    }

    public static void Serialize(INetworkMessage message, Stream outputStream)
    {
        using (var writer = new MyWriter(outputStream, Encoding.UTF8, leaveOpen: true))
        {
            Serialize(message, writer);
        }
        
        /*
        using (var writer = new MyWriter(outputStream, Encoding.UTF8))
        {            
            int messageSize = TypeIndexSize + new SerializedSizeMeasurer(Encoding.UTF8).Measure(message);
            Assert.IsFalse(messageSize > MaxMessageSizeBytes, $"Message too large: {messageSize} > {MaxMessageSizeBytes}");
            Debug.Log("messageSize: " + messageSize);
            
            writer.Serialize(ref messageSize);
            Serialize(message, writer);
        }*/

        /*using (var stream = new MemoryStream(WriteBuffer))
        using (var writer = new MyWriter(stream, Encoding.UTF8))
        {            
            Serialize(message, writer);
            
            int messageSize = (int)stream.Position;
            Assert.IsFalse(messageSize > MaxMessageSizeBytes, $"Message too large: {messageSize} > {MaxMessageSizeBytes}");
            Debug.Log("sizeBytes: " + messageSize);

            outputStream.Write(BitConverter.GetBytes(messageSize), 0, MessageSizeSize);
            outputStream.Write(WriteBuffer, 0, messageSize);
        }*/
    }
    
    public static INetworkMessage Deserialize(Stream inputStream)
    {
        using (var reader = new MyReader(inputStream, Encoding.UTF8, leaveOpen: true))
        {
            return Deserialize(reader);
        }
        
        /*ReadBytesAll(inputStream, ReadBuffer, 0, MessageSizeSize);
        int messageSizeBytes = BitConverter.ToInt32(ReadBuffer, 0);
        Debug.Log("Got messageSize: " + messageSizeBytes);
        ThrowIfInvalidMessageLength(messageSizeBytes);

        ReadBytesAll(inputStream, ReadBuffer, MessageSizeSize, messageSizeBytes, MessageReadTimeoutMs);
        return Deserialize(ReadBuffer, MessageSizeSize, messageSizeBytes);*/
    }
    
    private static void Serialize(INetworkMessage message, MyWriter writer)
    {
        ushort typeIndex = GetTypeIndexOf(message.GetType());
        writer.Serialize(ref typeIndex);

        message.Serialize(writer);
    }

    private static INetworkMessage Deserialize(MyReader reader)
    {        
        ushort typeIndex = 0;
        reader.Serialize(ref typeIndex);
        Type type = GetTypeBy(typeIndex);

        var message = (INetworkMessage)Activator.CreateInstance(type);
        message.Serialize(reader);
        return message;
    }
    
    #region Reflection helpers

    private static IEnumerable<Type> GetMessageTypes()
    {
        Type messsageBaseType = typeof(INetworkMessage);

        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => !type.IsAbstract && !type.IsInterface && messsageBaseType.IsAssignableFrom(type));
    }

    private static string GetMessageTypeName(Type type)
    {
        NetworkMessageAttribute attribute = GetNetworkMessageAttribute(type);
        return attribute?.name ?? type.ToString();
    }
    
    private static NetworkMessageAttribute GetNetworkMessageAttribute(Type type)
    {
        return type
            .GetCustomAttributes(inherit: true)
            .OfType<NetworkMessageAttribute>()
            .LastOrDefault();
    }
    
    private static void CheckAllMessageTypesHaveAPublicParameterlessConstructor(IEnumerable<Type> messageTypes)
    {
        foreach (Type messageType in messageTypes)
        {
            if (!HasPublicParameterlessConstructor(messageType))
            {
                Debug.LogError($"Network message type {messageType} must doesn't have a public parameterless constructor. All network message types must have one.");
            }
        }
    }

    private static bool HasPublicParameterlessConstructor(Type type)
    {
        return type
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
            .Any(IsConstructorParameterless);
    }

    private static bool IsConstructorParameterless(ConstructorInfo info)
    {
        ParameterInfo[] parameters = info.GetParameters();
        return parameters.Length == 0 || parameters.All(p => p.IsOptional);
    }

    #endregion

    private static ushort GetTypeIndexOf(Type type)
    {
        ushort typeIndex;
        bool indexFound = MessageTypeIndices.TryGetValue(type, out typeIndex);
        if (!indexFound)
        {
            throw new ArgumentException($"Couldn't find type index of given type ({type})!");
        }

        return typeIndex;
    }

    private static Type GetTypeBy(ushort typeIndex)
    {
        if (typeIndex >= MessageTypeInfos.Length)
        {
            throw new ArgumentException($"Couldn't find type for given type index ({typeIndex})!");
        }

        return MessageTypeInfos[typeIndex].type;
    }
    
    #region Helpers
        
    private static INetworkMessage Deserialize(byte[] buffer, int offset, int length)
    {
        using (var stream = new MemoryStream(buffer, offset, length))
        using (var reader = new MyReader(stream, Encoding.UTF8))
        {
            return Deserialize(reader);
        }
    }
    
    private static void ReadBytesAll(Stream stream, byte[] buffer, int offset, int numBytesToRead, int timeoutMs = -1)
    {
        bool hasTimeout = (timeoutMs >= 0);

        int initialReadTimeout = stream.ReadTimeout;
        if (hasTimeout)
        {
            stream.ReadTimeout = timeoutMs;
        }

        int totalNumBytesRead = 0;
        while (totalNumBytesRead < numBytesToRead)
        {
            int numBytesRead = stream.Read(buffer, offset + totalNumBytesRead, numBytesToRead - totalNumBytesRead);

            Console.WriteLine($"NetworkMessageSerializer: Read {numBytesRead} bytes");
            if (numBytesRead == 0)
            {
                stream.ReadTimeout = initialReadTimeout;
                throw new IOException("Could not read from stream");
            }

            totalNumBytesRead += numBytesRead;
        }

        stream.ReadTimeout = initialReadTimeout;
    }

    private static void ThrowIfInvalidMessageLength(int messageLengthBytes)
    {
        if (messageLengthBytes <= 0)
        {
            throw new InvalidDataException("Invalid message length");
        }
        if (messageLengthBytes > MaxMessageSizeBytes)
        {
            throw new InvalidDataException("Message length exceeds maximum message length");
        }
    }
    
    #endregion
}