using System.Xml.Linq;

namespace Argot.XMPP.Core.Stanzas;

public partial class IQ : Stanza
{
    public enum IQTypes
    {
        get,
        set,
        result,
        error
    }

    public IQTypes IQType
    {
        get =>
            Attribute("type").Value switch
            {
                "get" => IQTypes.get,
                "set" => IQTypes.set,
                "result" => IQTypes.result,
                _ => IQTypes.error,
            };
        set =>
            SetAttributeValue("type", value);
    }
    public IQ(IQTypes type, string id = "") : base(XNamespace.Get(Namespaces.JabberClient) + "iq")
    {
        IQType = type;
        SetAttributeValue("id", string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id);
    }

    public IQ() : base(XNamespace.Get(Namespaces.JabberClient) + "iq") { }

    public new string ID => Attribute("id").Value;

    public IQ Reply()
    {
        IQType = IQTypes.result;
        var to = Attribute("from").Value;
        SetAttributeValue("from", Attribute("to").Value);
        SetAttributeValue("to", to);
        return this;
    }

    public IQ Error()
    {
        var result = Reply();
        result.IQType = IQTypes.error;
        var error = new XElement(XNamespace.Get(Namespaces.StanzaErrors) + "error", new XElement(XNamespace.Get(Namespaces.StanzaErrors) + "service-unavailable"));
        error.SetAttributeValue("type", "cancel");
        result.Add(error);
        return result;
    }
}
