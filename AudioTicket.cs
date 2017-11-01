﻿namespace Assets.Scripts.Audio
{
    public struct AudioTicket
    {
        public static readonly AudioTicket Invalid = new AudioTicket(0);

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public AudioTicket(uint id)
        {
            this.Id = id;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public readonly uint Id;

        public static bool operator ==(AudioTicket value1, AudioTicket value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(AudioTicket value1, AudioTicket value2)
        {
            return !(value1 == value2);
        }

        public override bool Equals(object obj)
        {
            var typed = (AudioTicket)obj;
            return typed.Id == this.Id;
        }

        public bool Equals(AudioTicket other)
        {
            return this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            return (int)this.Id;
        }
    }
}
