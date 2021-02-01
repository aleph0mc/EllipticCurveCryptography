using EllipticCurves.ExtensionsAndHelpers;
using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using static System.Console;

namespace ConsoleEllipticCurvesSample
{
    class Program
    {
        private static void getWeirstrasseFormFromEdwards()
        {
            Write("d = ");
            var strD = ReadLine();
            var d = BigInteger.Parse(strD);

            WriteLine();
            Write("p = ");
            string strP = ReadLine();
            var p = BigInteger.Parse(strP);

            WriteLine();
            Write("Gx_edw = ");
            string strGx = ReadLine();
            var gx = BigInteger.Parse(strGx);

            WriteLine();
            Write("Gy_edw = ");
            string strGy = ReadLine();
            var gv = BigInteger.Parse(strGy);


            // InvMod constants
            var inv16 = BigInteger.Parse("16").ModInv(p);
            var inv12 = BigInteger.Parse("12").ModInv(p);
            var inv108 = BigInteger.Parse("108").ModInv(p);
            var inv96 = BigInteger.Parse("96").ModInv(p);
            var inv4 = BigInteger.Parse("4").ModInv(p);
            var inv6 = BigInteger.Parse("6").ModInv(p);

            var invOp = (1 + gv).ModInv(p);

            var tmpA = ((1 - d).BigPow(2) * inv16 - (1 + d).BigPow(2) * inv12) % p;
            var a = (tmpA % p + p) % p;

            var tmpB = ((1 + d).BigPow(3) * inv108 - (1 - d).BigPow(2) * (1 + d) * inv96) % p;
            var b = (tmpB % p + p) % p;

            // (u,v) ==> (x,y) = [(1-v)*(1-d)/4/(1+v) - (1+d)/6, (1-v)/u/(1+v) * (1-d)/6] 
            var tmpx1 = ((1 - gv) * (1 - d) * inv4 * invOp) % p;
            var tmpX2 = ((1 + d) * inv6) % p;
            var tmpX = (tmpx1 - tmpX2) % p;
            var x = (tmpX % p + p) % p;

            var y2 = (x.BigPow(3) + BigInteger.Multiply(a, x) + b) % p;
            var y = y2.ModSqrt(p);
        }

        static void Main(string[] args)
        {
            //getWeirstrasseFormFromEdwards();

            // Elliptic curve chosen
            var ecType = EllipticCurveType.E521;

            Console.WriteLine("Elliptic curve type: {0}", ecType.ToString());
            Console.WriteLine();

            // only Alice knows
            var aliceSecretKey = BigInteger.Parse("55179115824979878594564946684576670362812219109178118526265814188406326272077");
            // only Bob knows
            var bobSecretKey = BigInteger.Parse("28186466892849679686038856807396267537577176687436853369");
            // public keys
            // Alice's public key
            EcModPoint pa = null;
            // Bob's public key
            EcModPoint pb = null;
            switch (ecType)
            {
                case EllipticCurveType.SEC256K1:
                    pa = EcCryptographyHelper.SecP256k1KeyPairGenerator(aliceSecretKey);
                    pb = EcCryptographyHelper.SecP256k1KeyPairGenerator(bobSecretKey);
                    break;
                case EllipticCurveType.M383:
                    pa = EcCryptographyHelper.M383KeyPairGenerator(aliceSecretKey);
                    pb = EcCryptographyHelper.M383KeyPairGenerator(bobSecretKey);
                    break;
                case EllipticCurveType.E521:
                    pa = EcCryptographyHelper.E521KeyPairGenerator(aliceSecretKey);
                    pb = EcCryptographyHelper.E521KeyPairGenerator(bobSecretKey);
                    break;
                default:
                    break;
            }

            // Alice sends a message to Bob
            string message = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Error, assumenda beatae ipsum similique dignissimos odit officia dolores laudantium quis dicta.";
            Console.WriteLine("Alice writes to Bob: {0}", message);

            // Alice encrypts the message using Bob's public key
            var encryptStr = EcCryptographyHelper.EcEncryptJson(message, pb, ecType);
            // Bob decrypts the message using his private key
            var decryptedMsg = EcCryptographyHelper.EcDecryptJson(encryptStr, bobSecretKey, ecType);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Bob reads: {0}", decryptedMsg);
            Console.WriteLine();
            Console.WriteLine();

            // Bob sends a message to Alice
            message = "Deleniti illum quaerat vel ullam tempore! Animi, deserunt, rerum, illum harum veniam exercitationem cupiditate perspiciatis corporis eveniet quae ipsum sunt nam vel est neque omnis repellat magnam debitis sequi laboriosam blanditiis.";
            Console.WriteLine("Bob writes to Alice: {0}", message);

            // Bob encrypts the message using Alice's public key
            encryptStr = EcCryptographyHelper.EcEncryptJson(message, pa, ecType);
            // Alice decrypts the message using her private key
            decryptedMsg = EcCryptographyHelper.EcDecryptJson(encryptStr, aliceSecretKey, ecType);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Alice reads: {0}", decryptedMsg);

            Console.WriteLine("Finished....");
            Console.ReadKey();

        }
    }
}
