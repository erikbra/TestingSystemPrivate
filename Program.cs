using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace TestingSystemPrivate
{
    public class Program
    {
        // I can reference the X509SecurityToken type if I do the hack in the .csproj to copy
        // System.Private.ServiceModel.dll to the output folder.
        // If I omit the hack, this type is not recognized.
        static X509SecurityToken _token;

        public static void Main()
        {
            _token = new X509SecurityToken(new X509Certificate2());
            Console.WriteLine("Token: " + _token);

            // If I include the hack in the csproj, I get an error that this type exists in both
            // System.Private.ServiceModel, Version=4.8.0.0 and
            // System.ServiceModel.Primitives, Version=4.8.0.0
            var c = new SecurityKeyIdentifierClause();
        }
    }
}
