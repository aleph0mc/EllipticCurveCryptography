using EllipticCurves.ExtensionsAndHelpers;
using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleEllipticCurvesSample
{
    // Finds the integer square root of a positive number
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
            var pa = EccCryptographyHelper.SecP256k1KeyPairGenerator(aliceSecretKey);
            // Bob's public key
            var pb = EccCryptographyHelper.SecP256k1KeyPairGenerator(bobSecretKey);

            // Alice sends a message to Bob
            string message = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Error, assumenda beatae ipsum similique dignissimos odit officia dolores laudantium quis dicta.";
            Console.WriteLine("Alice writes to Bob: {0}", message);

            // Alice encrypts the message using Bob's public key
            var decryptStr = EccCryptographyHelper.EncryptSecP256k1Json(message, pb);
            // Bob decrypts the message using his private key
            var decryptedMsg = EccCryptographyHelper.DecryptSecP256k1Json(decryptStr, bobSecretKey);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Bob reads: {0}", decryptedMsg);
            Console.WriteLine();
            Console.WriteLine();

            // Bob sends a message to Alice
            message = "Deleniti illum quaerat vel ullam tempore! Animi, deserunt, rerum, illum harum veniam exercitationem cupiditate perspiciatis corporis eveniet quae ipsum sunt nam vel est neque omnis repellat magnam debitis sequi laboriosam blanditiis.";
            Console.WriteLine("Bob writes to Alice: {0}", message);

            // Bob encrypts the message using Alice's public key
            decryptStr = EccCryptographyHelper.EncryptSecP256k1Json(message, pa);
            // Alice decrypts the message using her private key
            decryptedMsg = EccCryptographyHelper.DecryptSecP256k1Json(decryptStr, aliceSecretKey);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Alice reads: {0}", decryptedMsg);

            //// Elliptic curve: y^2 = x^3 + a*x + b (mod p)
            //BigInteger p = BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007908834671663");

            //BigInteger a = 0;
            //BigInteger b = 7;
            //// P = G
            //var G = new EcModPoint
            //{
            //    x = BigInteger.Parse("55066263022277343669578718895168534326250603453777594175500187360389116729240"),
            //    y = BigInteger.Parse("32670510020758816978083085130507043184471273380659243275938904335757337482424")
            //};


            //// Order of G is N
            //BigInteger ordN = BigInteger.Parse("115792089237316195423570985008687907852837564279074904382605163141518161494337");

            //Console.WriteLine("Elliptic curve: y^2 = (x^3 {0} {1}*x {2} {3}) (mod {4})", a < 0 ? "-" : "+", BigInteger.Abs(a), b < 0 ? "-" : "+", BigInteger.Abs(b), p);
            //Console.WriteLine("P0: ({0}, {1})", G.x, G.y);


            //Console.WriteLine("Choose a Password (max 30 character length) :");
            //var pwd = Console.ReadLine();
            //if (string.IsNullOrEmpty(pwd) || pwd.Length > 30)
            //{
            //    Console.WriteLine("Password length must be between 1 and 32 characters.");
            //    Environment.Exit(1);
            //}

            //// check the strength
            //var strength = PasswordManager.Strength(pwd);
            //Console.WriteLine("Strength: {0} bits", strength);
            //var hash1 = PasswordManager.Hash(pwd, SHA1.Create());
            //var hash256 = PasswordManager.Hash(pwd, SHA256.Create());
            //var hash512 = PasswordManager.Hash(pwd, SHA512.Create());

            //var bytes = Encoding.UTF8.GetBytes(hash256);

            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(bytes);

            //// My secret key
            ////BigInteger sk = BigInteger.Parse("55179115824979878594564946684576670362812219109178118526265814188406326272077");
            //BigInteger sk = new BigInteger(bytes);
            //// sk mod ordN
            //sk %= ordN;

            //Console.WriteLine("SK: {0}", sk);
            //Console.WriteLine("SK: 0x{0}", sk.ToHexadecimalString());

            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine("---------------------------------- Extension method -----------------------------------------");
            //Console.WriteLine();

            //var sec256k1 = EccCryptographyHelper.SecP256k1KeyPairGenerator(sk);
            //Console.WriteLine("Public Key Pair:");
            //Console.WriteLine();
            //Console.WriteLine("x: {0}", sec256k1.x);
            //Console.WriteLine("y: {0}", sec256k1.y);
            //Console.WriteLine("Hex values");
            //Console.WriteLine("x: 0x{0}", sec256k1.x.ToHexadecimalString());
            //Console.WriteLine("y: 0x{0}", sec256k1.y.ToHexadecimalString());


            //// EC: y^2 = x^3 - 3*x - 14 - Point(3,2), order N = 340
            //var test = EccCryptographyHelper.EcSlowKeyPairGeneratorByClassicPointSum(1009, -3, 4, new EcModPoint { x = 3, y = 2 }, 337);

            Console.WriteLine("Finished....");
            Console.ReadKey();

        }
    }
}
