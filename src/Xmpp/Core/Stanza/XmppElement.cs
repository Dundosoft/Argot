using System.Xml.Linq;
using System.Reflection;

namespace Xmpp.Core.Stanza
{
    public abstract class XmppElement : XElement
    {
        public XmppElement(XElement other) : base(other)
        {
        }

        protected XmppElement(XName name) : base(name)
        {
        }
        protected T Element<T>(XName name)
           where T : XElement =>
            Convert<T>(Element(name));

        protected string GetAttributeValue(XName name)
        {
            var attribute = Attribute(name);
            return attribute?.Value;
        }

        protected T GetAttributeEnumValue<T>(XName name)
            where T : Enum
        {
            var attribute = GetAttributeValue(name);

            return string.IsNullOrEmpty(attribute) ?
                (T)Enum.Parse(typeof(T), attribute, true)
                : default;
        }

        protected void SetAttributeEnumValue<T>(XName name, T value)
            where T : Enum
        {
            SetAttributeValue(name, value.ToString().ToLowerInvariant());
        }

        public static ConstructorInfo GetConstructor(Type type, IReadOnlyCollection<Type> parameters)
        {
            var results = from constructor in type.GetTypeInfo().DeclaredConstructors
                          let constructorParameters = constructor.GetParameters().Select(_ => _.ParameterType).ToArray()
                          where constructorParameters.Length == parameters.Count &&
                                !constructorParameters.Except(parameters).Any() &&
                                !parameters.Except(constructorParameters).Any()
                          select constructor;

            return results.FirstOrDefault();
        }

        private static T Convert<T>(XElement element)
           where T : XElement
        {
            if (element is null)
            {
                return default;
            }

            var constructor = GetConstructor(typeof(T), new[] { typeof(XElement) });
            return (T)constructor?.Invoke(new object[] { element });
        }

    }
}
