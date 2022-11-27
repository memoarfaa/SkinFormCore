
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#pragma warning disable CS8765 
using System;
    internal readonly partial struct LRESULT : IEquatable<LRESULT>
    {
        internal readonly IntPtr Value;
        internal LRESULT(IntPtr value) => this.Value = value;
        public static implicit operator IntPtr(LRESULT value) => value.Value;
        public static explicit operator LRESULT(IntPtr value) => new LRESULT(value);
        public static bool operator ==(LRESULT left, LRESULT right) => left.Value == right.Value;
        public static bool operator !=(LRESULT left, LRESULT right) => !(left == right);

        public bool Equals(LRESULT other) => this.Value == other.Value;
    public override bool Equals(object obj) => obj is LRESULT other && this.Equals(other);
    public override int GetHashCode() => this.Value.GetHashCode();
    }
