using System.Security.Cryptography;
using System.Text;

namespace Argot.XMPP.Core.SASL;

public class SASLDigestMd5 : SASLHandler
{
    private int _state = 0;
    public SASLDigestMd5()
    {
    }

    public override SASLMechanismType SASLMechanismType => SASLMechanismType.DigestMD5;

    public override string Initiate() => string.Empty;

    public override string NextChallenge(string challenge)
    {
        if (_state == 0)
        {
            var challengeString = Convert.FromBase64String(challenge);
            var challengeFields = Encoding.UTF8.GetString(challengeString, 0, challengeString.Length);
            var fields = new Dictionary<string, string>();
            foreach (var f in challengeFields.Split(','))
            {
                var nextField = f.Split(new[] { '=' });
                fields[nextField[0]] = nextField[1].Trim('"');
            }
            fields["username"] = JID.Local;
            fields["realm"] = JID.Domain;
            fields["cnonce"] = "100500100";
            fields["nc"] = "00000001";
            fields["digest-uri"] = string.Format("xmpp/{0}", fields["realm"]);
            // fields["authzid"] = realm.connectionJid.FullJid;
            var x = string.Format("{0}:{1}:{2}", JID.Local, JID.Domain, Password);
            var md5 = MD5.Create();
            var y = md5.ComputeHash(Encoding.UTF8.GetBytes(x));
            var a1 = y.Concat(Encoding.UTF8.GetBytes(string.Format(":{0}:{1}", fields["nonce"], fields["cnonce"])));
            var a2 = string.Format("AUTHENTICATE:{0}", fields["digest-uri"]);
            var ha1 = BitConverter.ToString(md5.ComputeHash(a1.ToArray())).Replace("-", string.Empty).ToLower();
            var ha2 =
                BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(a2))).Replace("-", string.Empty).
                    ToLower();
            var kd = string.Format("{0}:{2}:{3}:{4}:auth:{1}", ha1, ha2, fields["nonce"], fields["nc"],
                                   fields["cnonce"]);
            fields["response"] =
                BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(kd))).Replace("-", string.Empty).
                    ToLower();
            var responseString = string.Format(
                "username=\"{0}\",realm=\"{1}\",nonce=\"{2}\",nc={4},cnonce=\"{3}\",qop=\"auth\",digest-uri=\"{5}\",response=\"{6}\",charset=utf-8",
                fields["username"], fields["realm"], fields["nonce"], fields["cnonce"], fields["nc"],
                fields["digest-uri"], fields["response"]);
            _state = 1;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(responseString));
        }
        return string.Empty;
    }
}
