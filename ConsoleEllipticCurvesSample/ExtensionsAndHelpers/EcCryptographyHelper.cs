﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;

namespace EllipticCurves.ExtensionsAndHelpers
{
    public class EcModPoint
    {
        public BigInteger x { get; set; }
        public BigInteger y { get; set; }
        public bool IsInf { get; set; }
        public BigInteger OrderN { get; set; }
    }

    public enum EllipticCurveType
    {
        SEC256K1,
        M383,
        E521
    }

    /// <summary>
    /// Helper class for Elliptic Curve Cryptography
    /// ref: http://safecurves.cr.yp.to/equation.html
    /// ref. http://www.secg.org/
    /// </summary>
    public static class EcCryptographyHelper
    {
        #region CONSTANTS FOR CURVE sec256k1

        private static readonly BigInteger SEC256K1_P = BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007908834671663");
        private static readonly BigInteger SEC256K1_N = BigInteger.Parse("115792089237316195423570985008687907852837564279074904382605163141518161494337");
        private static readonly EcModPoint SEC256K1_G = new EcModPoint
        {
            x = BigInteger.Parse("55066263022277343669578718895168534326250603453777594175500187360389116729240"),
            y = BigInteger.Parse("32670510020758816978083085130507043184471273380659243275938904335757337482424")
        };
        private static readonly BigInteger SEC256K1_A = 0;
        private static readonly BigInteger SEC256K1_B = 7;

        #endregion

        #region CONSTANTS FOR CURVE M-383 Montgomery (AKA Curve383187)

        private static readonly BigInteger M383_P = BigInteger.Parse("19701003098197239606139520050071806902539869635232723333974146702122860885748605305707133127442457820403313995153221");
        private static readonly BigInteger M383_N = BigInteger.Parse("2462625387274654950767440006258975862817483704404090416746934574041288984234680883008327183083615266784870011007447");
        private static readonly EcModPoint M383_G = new EcModPoint
        {
            x = BigInteger.Parse("13134002065464826404093013366714537935026579756821815555982764468081907257165736870471422084961638546935542664123876"),
            y = BigInteger.Parse("4737623401891753997660546300375902576839617167257703725630389791524463565757299203154901655432096558642117242906494")
        };
        private static readonly BigInteger M383_A = BigInteger.Parse("6567001032732413202046506683357268967513289878410907777991382234040953628582868435235711042480819273466349716876908");
        private static readonly BigInteger M383_B = BigInteger.Parse("729666781414712578005167409261918774168143319823434197554598026004550403175874270581745671386758349462616491048773");

        #endregion

        #region CONSTANTS FOR CURVE E-521 Edwards

        private static readonly BigInteger E521_P = BigInteger.Parse("17161994150326524287454751997703483043173588250358263523486158647963857958494136754758766516636578496366936590652341426043192829487025423179934212936701086591");
        private static readonly BigInteger E521_N = BigInteger.Parse("1716199415032652428745475199770348304317358825035826352348615864796385795849413675475876651663657849636693659065234142604319282948702542317993421293670108523");
        //private static readonly EcModPoint E521_G = new EcModPoint
        //{
        //    x = BigInteger.Parse("12548554862604340339214227267138030612212946247573784081688804172704756356748401067995657237970831588741416001767303408289646369947502459959521790104254553894"),
        //    y = BigInteger.Parse("5913688689523515869205441309572965034430425538652800303101623058374052639077588954535236458964704491401744419102950722299571100399610901049518344897138107685")
        //};
        private static readonly EcModPoint E521_G = new EcModPoint
        {
            x = BigInteger.Parse("10451214386416793636591034870396351853214685152461763043148622253567734013185531998090274481285095879197813949435720740218611017956842405141626604031965388423"),
            y = BigInteger.Parse("13053158159023441333706021006305066422008818106876041977610246796265937623367302962946524167272899818197851994225231701868043808972251678880647319318228933683")
        };
        private static readonly BigInteger E521_A = BigInteger.Parse("11083787888752546935647860665183499465382942411689711858918144126809991598194129987448370041994456945570313214796303837652895369043703919137040845852007342090");
        private static readonly BigInteger E521_B = BigInteger.Parse("11699553882572132876517186257693693880126439212339140295524707689410546686982692764528835044327482331435330615618320717522500667323909692422432065495019324207");

        #endregion


        #region ELLIPTIC CURVE KEY PAIR GENERATOR

        private static EcModPoint pointAdd(EcModPoint pointP, EcModPoint pointQ, BigInteger p)
        {
            // compute m = (y1 - y0) / (x1 - x0) (mod p)
            var x0 = pointP.x;
            var y0 = pointP.y;

            var x1 = pointQ.x;
            var y1 = pointQ.y;

            // compute m mod p
            BigInteger dy = (y1 - y0) % p;
            BigInteger dx = (x1 - x0) % p;
            dy = (dy + p) % p;
            dx = (dx + p) % p;

            BigInteger inv_dx = dx.ModInv(p);
            BigInteger m = BigInteger.Multiply(dy, inv_dx) % p;
            m = (m + p) % p;

            var x = (m * m - x0 - x1) % p;
            x = (x + p) % p;
            var y = (m * (x0 - x) - y0) % p;  //(-m * m * m + 3 * m * x0 - y0) % p;
            y = (y + p) % p;

            EcModPoint ret = new EcModPoint { x = x, y = y };
            return ret;

        }

        private static EcModPoint pointDouble(EcModPoint point, BigInteger a, BigInteger p)
        {
            // compute m = dy/dx = dy * dx^-1 (mod p)
            var x0 = point.x;
            var y0 = point.y;

            BigInteger dy = (3 * x0 * x0 + a) % p;
            BigInteger dx = (2 * y0) % p;

            dy = (dy + p) % p;
            dx = (dx + p) % p;

            // compute inverse of dx mod p
            BigInteger inv_dx = dx.ModInv(p);

            // compute m mod p
            BigInteger m = BigInteger.Multiply(dy, inv_dx) % p;
            m = (m + p) % p;

            var x = (m * m - 2 * x0) % p;
            x = (x + p) % p;
            var y = (m * (x0 - x) - y0) % p;  //(-m * m * m + 3 * m * x0 - y0) % p;
            y = (y + p) % p;

            EcModPoint ret = new EcModPoint { x = x, y = y };
            return ret;
        }

        private static EcModPoint computeECPointMultiply(BigInteger a, EcModPoint g, BigInteger p, BigInteger sk)
        {
            var bitString = sk.ToBinaryString();
            EcModPoint q = g;
            foreach (var bit in bitString)
            {
                q = pointDouble(q, a, p);
                if (bit == '1')
                    q = pointAdd(q, g, p);
            }

            return q;

        }

        private static EcModPoint computePointSum(BigInteger a, EcModPoint pointP, EcModPoint pointQ, BigInteger p)
        {
            BigInteger x2 = 0;
            BigInteger y2 = 0;
            BigInteger m = 0;
            EcModPoint ret = null;

            if (null == pointQ) // P1 = P0, P2 = P0 + P1 = 2P0
                ret = pointDouble(pointP, a, p);
            else // P2 = P0 + P1
                ret = pointAdd(pointP, pointQ, p);

            return ret;
        }

        /// <summary>
        /// Returns the key pair
        /// </summary>
        /// <param name="P"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="G"></param>
        /// <param name="N"></param>
        /// <param name="SecretKey"></param>
        /// <returns></returns>
        public static EcModPoint ECKeyPairGenerator(BigInteger P, BigInteger A, BigInteger B, EcModPoint G, BigInteger N, BigInteger SecretKey)
        {
            // If private key Sk is not in the range 0 < Sk < N (order of G) then error
            if (SecretKey < 1 || SecretKey >= N)
                throw new Exception("Private key is not in the range.");

            // Check the discriminant for the eliptic curve y^2 = x^3 + a*x + b: -16(4a^3 + 27b^2) != 0
            var delta = 4 * A * A * A + 27 * B * B;
            if (0 == delta)
                throw new Exception("Delta cannot be zero.");

            return computeECPointMultiply(A, G, P, SecretKey);
        }

        /// <summary>
        /// Return point Q coords representing public key pair (see specs for secp256k1 parameters)
        /// </summary>
        /// <returns></returns>
        public static EcModPoint SecP256k1KeyPairGenerator(BigInteger Sk)
        {
            // Elliptic curve equation: y^2 = x^3 + 7 (short Weierstrass form)
            return ECKeyPairGenerator(SEC256K1_P, SEC256K1_A, SEC256K1_B, SEC256K1_G, SEC256K1_N, Sk);
        }

        /// <summary>
        /// Return point Q coords representing public key pair (see specs for M-383 parameters)
        /// </summary>
        /// <returns></returns>
        public static EcModPoint M383KeyPairGenerator(BigInteger Sk)
        {
            // Elliptic curve equation:
            // y^2 = x^3 + 6567...6908*x + 7296...48773 (short Weierstrass form, used in this context)
            // y^2 = x^3 + 2065150*x^2 + x (Montgomery form)

            return ECKeyPairGenerator(M383_P, M383_A, M383_B, M383_G, M383_N, Sk);
        }


        /// <summary>
        /// Return point Q coords representing public key pair (see specs for M-383 parameters)
        /// </summary>
        /// <returns></returns>
        public static EcModPoint E521KeyPairGenerator(BigInteger Sk)
        {
            // Elliptic curve equation:
            // y^2 = x^3 + 6567...6908*x + 7296...48773 (short Weierstrass form, used in this context)
            // y^2 = x^3 + 2065150*x^2 + x (Montgomery form)

            return ECKeyPairGenerator(E521_P, E521_A, E521_B, E521_G, E521_N, Sk);
        }

        /// <summary>
        /// Returns the final point via summation. This method is very slow. Do not use big value for Sk.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="G"></param>
        /// <param name="Sk"></param>
        /// <returns></returns>
        public static EcModPoint EcSlowKeyPairGeneratorByClassicPointSum(BigInteger P, BigInteger A, BigInteger B, EcModPoint G, int Sk)
        {
            var delta = 4 * A * A * A + 27 * B * B;
            if (0 == delta)
                throw new Exception("Delta cannot be zero.");

            int cnt = 2;
            bool inf = false;
            EcModPoint qMod = null;
            while (cnt <= Sk && !inf)
            {
                qMod = computePointSum(A, G, qMod, P);
                inf = G.x == qMod.x;
                if (inf)
                {
                    qMod.IsInf = true;
                    qMod.OrderN = cnt;
                }
                ++cnt;
            }

            return qMod;
        }

        /// <summary>
        /// Returns the list of points by addition (P + Q). This method is very slow. Do not use big value for Sk.
        /// </summary>
        /// <param name="P"></param>
        /// <param name="A"></param>
        /// <param name="B></param>
        /// <param name="G"></param>
        /// <param name="Sk"></param>
        /// <returns></returns>
        public static List<EcModPoint> EcSlowPointListByAddition(BigInteger P, BigInteger A, BigInteger B, EcModPoint G, int Sk)
        {
            var delta = 4 * A * A * A + 27 * B * B;
            if (0 == delta)
                throw new Exception("Delta cannot be zero.");

            var ret = new List<EcModPoint>();
            ret.Add(G);
            int cnt = 2;
            bool inf = false;
            EcModPoint qMod = null;
            while (cnt <= Sk && !inf)
            {
                qMod = computePointSum(A, G, qMod, P);
                inf = G.x == qMod.x;
                if (inf)
                {
                    qMod.IsInf = true;
                    qMod.OrderN = cnt;
                }
                ret.Add(qMod);
                ++cnt;
            }

            return ret;
        }

        #endregion

        /// <summary>
        /// Encrypt text on elliptic curve and return a list of points on the elliptic curve
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="PubKey"></param>
        /// <returns></returns>
        public static List<EcModPoint> EcEncrypt(string Text, EcModPoint PubKey, EllipticCurveType EcType)
        {
            BigInteger p;
            BigInteger n;
            BigInteger a;
            BigInteger b;

            // Set params
            switch (EcType)
            {
                case EllipticCurveType.SEC256K1:
                    p = SEC256K1_P;
                    n = SEC256K1_N;
                    a = SEC256K1_A;
                    b = SEC256K1_B;
                    break;
                case EllipticCurveType.M383:
                    p = M383_P;
                    n = M383_N;
                    a = M383_A;
                    b = M383_B;
                    break;
                case EllipticCurveType.E521:
                    p = E521_P;
                    n = E521_N;
                    a = E521_A;
                    b = E521_B;
                    break;
                default:
                    break;
            }

            // return the digits of P base 65536
            ushort[] digits = Base65536Helper.ToArray(p);
            var partitionLen = digits.Length - 1;

            // return a string array with string spit in groups of digits
            var partitionStrings = Text.SplitInGroup(partitionLen);

            // 1) create a list of big integers out of partitioned message
            List<BigInteger> lstBiMsg = new List<BigInteger>();
            foreach (var partitionStr in partitionStrings)
            {
                // encode the partioned str in Unicode
                var encUtf8 = Encoding.Unicode.GetBytes(partitionStr);
                // convert to ushort array
                var arrShort = encUtf8.ToUShortArray();
                // convert to Base 65536 big integer
                var bi65536 = Base65536Helper.FromArray(arrShort);
                // add to the list
                lstBiMsg.Add(bi65536);
            }

            // 2) pair with ascii 32 (space) if list count is odd
            var lstCnt = lstBiMsg.Count;
            if (0 != lstCnt % 2)
            {
                lstBiMsg.Add(32);
                lstCnt++;
            }
            // 3) build the pairs
            List<EcModPoint> lstPm = new List<EcModPoint>();
            for (int i = 0; i < lstCnt; i += 2)
            {
                var pnt = new EcModPoint { x = lstBiMsg[i], y = lstBiMsg[i + 1] };
                lstPm.Add(pnt);
            }

            // get a random big integer k
            var k = BigIntegerExtensions.NextBigInteger(185, DateTime.Now.Millisecond);
            // 4) compute k*G for the chosen curve
            EcModPoint kG = null;
            switch (EcType)
            {
                case EllipticCurveType.SEC256K1:
                    kG = SecP256k1KeyPairGenerator(k);
                    break;
                case EllipticCurveType.M383:
                    kG = M383KeyPairGenerator(k);
                    break;
                case EllipticCurveType.E521:
                    kG = E521KeyPairGenerator(k);
                    break;
                default:
                    break;
            }

            // 5) compute kPb = k(SkG)
            var kPb = ECKeyPairGenerator(p, a, b, PubKey, n, k);

            // 6) compute Pm + kPb = Pc (encrypted data)
            List<EcModPoint> lstEncr = new List<EcModPoint>();
            // first row is kG
            lstEncr.Add(kG);
            foreach (var pnt in lstPm)
            {
                var pntAdded = pointAdd(pnt, kPb, p);
                lstEncr.Add(pntAdded);
            }

            return lstEncr;
        }

        /// <summary>
        /// Encrypt text on elliptic curve and return a list of points on the elliptic curve in Json format (string)
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="PubKey"></param>
        /// <returns></returns>
        public static string EcEncryptJson(string Text, EcModPoint PubKey, EllipticCurveType EcType)
        {
            var lstEncr = EcEncrypt(Text, PubKey, EcType);
            var json = JsonConvert.SerializeObject(lstEncr);

            return json;
        }

        /// <summary>
        /// Dencrypt text on elliptic curve and return the message
        /// </summary>
        /// <param name="LstEncr"></param>
        /// <param name="SecretKey"></param>
        /// <returns></returns>
        public static string EcDecrypt(List<EcModPoint> LstEncr, BigInteger SecretKey, EllipticCurveType EcType)
        {
            BigInteger p;
            BigInteger n;
            BigInteger a;
            BigInteger b;

            // Set params
            switch (EcType)
            {
                case EllipticCurveType.SEC256K1:
                    p = SEC256K1_P;
                    n = SEC256K1_N;
                    a = SEC256K1_A;
                    b = SEC256K1_B;
                    break;
                case EllipticCurveType.M383:
                    p = M383_P;
                    n = M383_N;
                    a = M383_A;
                    b = M383_B;
                    break;
                case EllipticCurveType.E521:
                    p = E521_P;
                    n = E521_N;
                    a = E521_A;
                    b = E521_B;
                    break;
                default:
                    break;
            }

            // 1) get kG
            var kG = LstEncr.First();
            // 2) compute kPb = k(SkG) = Sk(kG)
            var kPb = ECKeyPairGenerator(p, a, b, kG, n, SecretKey);
            // set -kPb
            var negkPb = new EcModPoint { x = kPb.x, y = -kPb.y };

            // remove kG from the list
            LstEncr.RemoveAt(0);

            // 3) decrypt - compute Pc - kPb = Pm (plain message)
            var strMsg = string.Empty;
            foreach (var pnt in LstEncr)
            {
                var decPnt = pointAdd(pnt, negkPb, p);
                // data from x coord
                var arrUShort = Base65536Helper.ToArray(decPnt.x);
                var bytes = arrUShort.ToByteArray();
                var str = Encoding.Unicode.GetString(bytes);
                strMsg = string.Concat(strMsg, str);
                // data from y coord
                arrUShort = Base65536Helper.ToArray(decPnt.y);
                bytes = arrUShort.ToByteArray();
                str = Encoding.Unicode.GetString(bytes);
                strMsg = string.Concat(strMsg, str);
            }

            return strMsg;
        }

        /// <summary>
        /// Decrypt text on elliptic curve using the list of points on the elliptic curve in Json format (string)
        /// </summary>
        /// <param name="JsonCrypt"></param>
        /// <param name="SecretKey"></param>
        /// <returns></returns>
        public static string EcDecryptJson(string JsonCrypt, BigInteger SecretKey, EllipticCurveType EcType)
        {
            var deserLst = JsonConvert.DeserializeObject<List<EcModPoint>>(JsonCrypt);
            var msg = EcDecrypt(deserLst, SecretKey, EcType);
            return msg;
        }
    }
}
