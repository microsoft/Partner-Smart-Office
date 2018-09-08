// -----------------------------------------------------------------------
// <copyright file="SecurityNetworkProtocol.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using Newtonsoft.Json;

    [JsonConverter(typeof(EnumConverter))]
    public enum SecurityNetworkProtocol
    {
        Unknown = -1,
        Ip = 0,
        Icmp = 1,
        Igmp = 2,
        Ggp = 3,
        Ipv4 = 4,
        Tcp = 6,
        Pup = 12, // 0x0000000C
        Udp = 17, // 0x00000011
        Idp = 22, // 0x00000016
        Ipv6 = 41, // 0x00000029
        Ipv6RoutingHeader = 43, // 0x0000002B
        Ipv6FragmentHeader = 44, // 0x0000002C
        IpSecEncapsulatingSecurityPayload = 50, // 0x00000032
        IpSecAuthenticationHeader = 51, // 0x00000033
        IcmpV6 = 58, // 0x0000003A
        Ipv6NoNextHeader = 59, // 0x0000003B
        Ipv6DestinationOptions = 60, // 0x0000003C
        Nd = 77, // 0x0000004D
        Raw = 255, // 0x000000FF
        Ipx = 1000, // 0x000003E8
        Spx = 1256, // 0x000004E8
        SpxII = 1257, // 0x000004E9
        UnknownFutureValue = 32767, // 0x00007FFF
    }
}
