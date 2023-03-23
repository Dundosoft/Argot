using System.Xml.Linq;

namespace Argot.XMPP.Core.Stanzas.Stream;

public enum StreamErrorType
{
    Unknown,
    InvalidNamespace,
    NotWellFormed
}

public class StreamError : Stanza
{
    public StreamError()
        : base(XNamespace.Get(Namespaces.Streams) + "error")
    {
    }

    public StreamErrorType ErrorType
    {
        get => Elements().FirstOrDefault().Name.LocalName switch
        {
            "not-well-formed" => StreamErrorType.NotWellFormed,
            _ => StreamErrorType.Unknown,
        };
        set => ReplaceNodes(new Stanza(
            value switch
            {
                StreamErrorType.InvalidNamespace => XNamespace.Get(Namespaces.StreamErrors) + "invalid-namespace",
                StreamErrorType.NotWellFormed => XNamespace.Get(Namespaces.StreamErrors) + "not-well-formed",
                _ => XNamespace.Get(Namespaces.StreamErrors) + "service-unavailable"
            }
            ));
    }
}
