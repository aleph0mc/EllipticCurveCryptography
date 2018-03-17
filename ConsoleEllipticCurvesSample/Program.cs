using EllipticCurves.ExtensionsAndHelpers;
using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleEllipticCurvesSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // only Alice knows
            var aliceSecretKey = BigInteger.Parse("55179115824979878594564946684576670362812219109178118526265814188406326272077");
            // only Bob knows
            var bobSecretKey = BigInteger.Parse("28186466892849679686038856807396267537577176687436853369");
            // public keys
            // Alice's public key
            var pa = EcCryptographyHelper.SecP256k1KeyPairGenerator(aliceSecretKey);
            // Bob's public key
            var pb = EcCryptographyHelper.SecP256k1KeyPairGenerator(bobSecretKey);

            // Alice sends a message to Bob
            string message = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Error, assumenda beatae ipsum similique dignissimos odit officia dolores laudantium quis dicta.";
            Console.WriteLine("Alice writes to Bob: {0}", message);

            // Alice encrypts the message using Bob's public key
            var encryptStr = EcCryptographyHelper.EncryptSecP256k1Json(message, pb);
            // Bob decrypts the message using his private key
            var decryptedMsg = EcCryptographyHelper.DecryptSecP256k1Json(encryptStr, bobSecretKey);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Bob reads: {0}", decryptedMsg);
            Console.WriteLine();
            Console.WriteLine();

            // Bob sends a message to Alice
            message = "Deleniti illum quaerat vel ullam tempore! Animi, deserunt, rerum, illum harum veniam exercitationem cupiditate perspiciatis corporis eveniet quae ipsum sunt nam vel est neque omnis repellat magnam debitis sequi laboriosam blanditiis.";
            Console.WriteLine("Bob writes to Alice: {0}", message);

            // Bob encrypts the message using Alice's public key
            encryptStr = EcCryptographyHelper.EncryptSecP256k1Json(message, pa);
            // Alice decrypts the message using her private key
            decryptedMsg = EcCryptographyHelper.DecryptSecP256k1Json(encryptStr, aliceSecretKey);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Alice reads: {0}", decryptedMsg);

            Console.WriteLine("Finished....");
            Console.ReadKey();

        }
    }
}
