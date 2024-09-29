using System;

namespace TriSerializer
{
    public struct ObjectIdentifier : IEquatable<ObjectIdentifier>
    {
        public char A;
        public char B;
        public char C;

        public ObjectIdentifier(string value)
        {
            if (value.Length != 3)
            {
                A = '?';
                B = '?';
                C = '?';
                return;
            }
            A = value[0];
            B = value[1];
            C = value[2];
        }

        public ObjectIdentifier(char a, char b, char c)
        {
            A = a;
            B = b;
            C = c;
        }

        public static implicit operator ObjectIdentifier(string value)
        {
            return new ObjectIdentifier(value);
        }

        public bool Equals(ObjectIdentifier other)
        {
            return A == other.A && B == other.B && C == other.C;
        }

        public override bool Equals(object obj)
        {
            if (obj is ObjectIdentifier other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(A, B, C);
        }

        public static bool operator ==(ObjectIdentifier left, ObjectIdentifier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ObjectIdentifier left, ObjectIdentifier right)
        {
            return !(left == right);
        }
    }
}