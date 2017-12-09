using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Packing.Internal
{
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    internal class PackageInfo
    {
        private static readonly Dictionary<Type, PackageInfo> PackageInfos = new Dictionary<Type, PackageInfo>();

        public static PackageInfo GetPackageInfo(Type type)
        {
            return PackageInfos.TryGetValue(type, out var packageInfo) ? packageInfo : null;
        }

        public static Type MakeUpPackage(params Type[] typesOfUpMessage)
        {
            foreach (var type in typesOfUpMessage)
            {
                if (!typeof(IUpMessage).IsAssignableFrom(type))
                {
                    throw new ArgumentException($"The type '{type.FullName}' does not derive from '{nameof(IUpMessage)}'.", nameof(typesOfUpMessage));
                }
            }
            return MakePackage(typesOfUpMessage);
        }

        public static Type MakeDownPackage(params Type[] typesOfDownMessage)
        {
            foreach (var type in typesOfDownMessage)
            {
                if (!typeof(IDownMessage).IsAssignableFrom(type))
                {
                    throw new ArgumentException($"The type '{type.FullName}' does not derive from '{nameof(IDownMessage)}'.", nameof(typesOfDownMessage));
                }
            }
            return MakePackage(typesOfDownMessage);
        }

        private static Type MakePackage(params Type[] typesOfMessage)
        {
            throw new NotImplementedException();
        }

        public IUpMessage GetMessage(IPackage package)
        {
            return (IMessage)PropertyOfMessageType(package.MessageType).GetValue(package);
        }

        public void SetMessage(IPackage package, IDownMessage value)
        {
            package.MessageType = MessageTypeOfClass(value.GetType());
            PropertyOfMessageType(package.MessageType).SetValue(package, value);
        }
    }
}
