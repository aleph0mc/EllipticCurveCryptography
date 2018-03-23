# Elliptic Curve Cryptography (Sample Console Application)

This is an implementation of text encryption using Elliptic Curve Cryptography.

The encryption and decryption algorithm implemented in this sample c# console application is based on the technique proposed by two researchers from the National Institute of Technology in Manipur, India.

The abstract and the full document in pdf format can be found at the following link

https://www.sciencedirect.com/science/article/pii/S1877050915013332

The curve I have use in my console application is not the same as the one used by the two authors, but the type known as sec256k1, more information at the following link

http://www.secg.org/ (visit SEC 2)

I have also added the possibility to use the elliptic curve known as M-383 which is a Montgomery curve (B*y^2 = x^3 + A*x^2 + x), but I have applied the coordinate changes suggested in the following link

http://safecurves.cr.yp.to/equation.html

in order to get the curve in the short Weierstrass form (y^2 = x^3 + a*x + b).

For the generation of a BigInteger random number I have adapted a bit the code found at the following link

https://gist.github.com/rharkanson/50fe61655e80488fcfec7d2ee8eff568

The algorithms used for the modular arithmetic can be easily found in the internet (Wikipedia) or many books about Number Theory.

Any suggestion and/or advice is welcome.

Good bytes.
