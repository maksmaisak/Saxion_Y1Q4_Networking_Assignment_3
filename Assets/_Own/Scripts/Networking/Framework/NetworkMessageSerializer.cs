using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

/// A helper class for reading and writing INetworkMessage to and from byte[].
public static class NetworkMessageSerializer
{
    struct MessageTypeInfo
    {
        public Type type;
        public string name;
    }

    private static readonly MessageTypeInfo[] MessageTypeInfos;
    private static readonly Dictionary<Type, ushort> MessageTypeIndices = new Dictionary<Type, ushort>();

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
            MessageTypeIndices.Add(MessageTypeInfos[index].type, index);
        }
    }

    public static void Serialize(INetworkMessage message, Stream outputStream)
    {
        using (var writer = new MyWriter(outputStream, Encoding.UTF8, leaveOpen: true))
        {
            Serialize(message, writer);
        }
    }
    
    public static INetworkMessage Deserialize(Stream inputStream)
    {
        using (var reader = new MyReader(inputStream, Encoding.UTF8, leaveOpen: true))
        {
            return Deserialize(reader);
        }
    }

    #region Helpers
    
    public static byte[] Serialize(INetworkMessage message)
    {
        using (var outputStream = new MemoryStream())
        {
            Serialize(message, outputStream);
            return outputStream.ToArray();
        }
    }

    public static INetworkMessage Deserialize(byte[] bytes)
    {
        using (var stream = new MemoryStream(bytes))
        {
            return Deserialize(stream);
        }
    }

    #endregion
    
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
}