// ReSharper disable UnusedMember.Global
namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Text;

    /// <summary>
    /// A pooled <see cref="StringBuilder"/>.
    /// </summary>
    public static class StringBuilderPool
    {
        private static readonly ConcurrentQueue<PooledStringBuilder> Cache = new ConcurrentQueue<PooledStringBuilder>();

        /// <summary>
        /// Borrow an instance.
        /// </summary>
        /// <returns>A <see cref="PooledStringBuilder"/>.</returns>
        public static PooledStringBuilder Borrow()
        {
            if (Cache.TryDequeue(out var item))
            {
                return item;
            }

            return new PooledStringBuilder();
        }

        /// <summary>
        /// Wrapping a <see cref="StringBuilder"/>.
        /// </summary>
        public class PooledStringBuilder
        {
            private readonly StringBuilder inner = new StringBuilder();

            /// <summary>Gets the maximum capacity of this instance.</summary>
            /// <returns>The maximum number of characters this instance can hold.</returns>
            public int MaxCapacity => this.inner.MaxCapacity;

            /// <summary>Gets or sets the maximum number of characters that can be contained in the memory allocated by the current instance.</summary>
            /// <returns>The maximum number of characters that can be contained in the memory allocated by the current instance. Its value can range from <see cref="System.Text.StringBuilder.Length" /> to <see cref="System.Text.StringBuilder.MaxCapacity" />. </returns>
            /// <exception cref="System.ArgumentOutOfRangeException">The value specified for a set operation is less than the current length of this instance.-or- The value specified for a set operation is greater than the maximum capacity. </exception>
            public int Capacity
            {
                get => this.inner.Capacity;
                set => this.inner.Capacity = value;
            }

            /// <summary>Gets or sets the length of the current <see cref="System.Text.StringBuilder" /> object.</summary>
            /// <returns>The length of this instance.</returns>
            /// <exception cref="System.ArgumentOutOfRangeException">The value specified for a set operation is less than zero or greater than <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            public int Length
            {
                get => this.inner.Length;
                set => this.inner.Length = value;
            }

            /// <summary>Gets or sets the character at the specified character position in this instance.</summary>
            /// <returns>The Unicode character at position <paramref name="index" />.</returns>
            /// <param name="index">The position of the character. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is outside the bounds of this instance while setting a character. </exception>
            /// <exception cref="System.IndexOutOfRangeException">
            /// <paramref name="index" /> is outside the bounds of this instance while getting a character. </exception>
            public char this[int index]
            {
                get => this.inner[index];
                set => this.inner[index] = value;
            }

            /// <summary>Appends the string representation of a specified Boolean value to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The Boolean value to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            public PooledStringBuilder Append(bool value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends the string representation of a specified 8-bit unsigned integer to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The value to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(byte value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends the string representation of a specified Unicode character to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The Unicode character to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(char value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends a specified number of copies of the string representation of a Unicode character to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The character to append. </param>
            /// <param name="repeatCount">The number of times to append <paramref name="value" />. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="repeatCount" /> is less than zero.-or- Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <exception cref="System.OutOfMemoryException">Out of memory.</exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(char value, int repeatCount)
            {
                this.inner.Append(value, repeatCount);
                return this;
            }

            /// <summary>Appends the string representation of the Unicode characters in a specified array to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The array of characters to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(char[] value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends the string representation of a specified subarray of Unicode characters to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">A character array. </param>
            /// <param name="startIndex">The starting position in <paramref name="value" />. </param>
            /// <param name="charCount">The number of characters to append. </param>
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="value" /> is null, and <paramref name="startIndex" /> and <paramref name="charCount" /> are not zero. </exception>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="charCount" /> is less than zero.-or- <paramref name="startIndex" /> is less than zero.-or- <paramref name="startIndex" /> + <paramref name="charCount" /> is greater than the length of <paramref name="value" />.-or- Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(char[] value, int startIndex, int charCount)
            {
                this.inner.Append(value, startIndex, charCount);
                return this;
            }

            /// <summary>Appends the string representation of a specified decimal number to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The value to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(decimal value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends the string representation of a specified double-precision floating-point number to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The value to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(double value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends the string representation of a specified 16-bit signed integer to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The value to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(short value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends the string representation of a specified 32-bit signed integer to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The value to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(int value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends the string representation of a specified 64-bit signed integer to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The value to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(long value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends the string representation of a specified object to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The object to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(object value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends the string representation of a specified 8-bit signed integer to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The value to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(sbyte value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends the string representation of a specified single-precision floating-point number to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The value to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(float value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends a copy of the specified string to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The string to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(string value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends a copy of a specified substring to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The string that contains the substring to append. </param>
            /// <param name="startIndex">The starting position of the substring within <paramref name="value" />. </param>
            /// <param name="count">The number of characters in <paramref name="value" /> to append. </param>
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="value" /> is null, and <paramref name="startIndex" /> and <paramref name="count" /> are not zero. </exception>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="count" /> less than zero.-or- <paramref name="startIndex" /> less than zero.-or- <paramref name="startIndex" /> + <paramref name="count" /> is greater than the length of <paramref name="value" />.-or- Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(string value, int startIndex, int count)
            {
                this.inner.Append(value, startIndex, count);
                return this;
            }

            /// <summary>Appends the string representation of a specified 16-bit unsigned integer to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The value to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(ushort value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends the string representation of a specified 32-bit unsigned integer to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The value to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(uint value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends the string representation of a specified 64-bit unsigned integer to this instance.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The value to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Append(ulong value)
            {
                this.inner.Append(value);
                return this;
            }

            /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a single argument using a specified format provider. </summary>
            /// <returns>A reference to this instance after the append operation has completed. After the append operation, this instance contains any data that existed before the operation, suffixed by a copy of <paramref name="format" /> in which any format specification is replaced by the string representation of <paramref name="arg0" />. </returns>
            /// <param name="provider">An object that supplies culture-specific formatting information. </param>
            /// <param name="format">A composite format string (see Remarks). </param>
            /// <param name="arg0">The object to format. </param>
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="format" /> is null. </exception>
            /// <exception cref="System.FormatException">
            /// <paramref name="format" /> is invalid. -or-The index of a format item is less than 0 (zero), or greater than or equal to one (1). </exception>
            /// <exception cref="System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            public PooledStringBuilder AppendFormat(IFormatProvider provider, string format, object arg0)
            {
                this.inner.AppendFormat(provider, format, arg0);
                return this;
            }

            /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of either of two arguments using a specified format provider.</summary>
            /// <returns>A reference to this instance after the append operation has completed. After the append operation, this instance contains any data that existed before the operation, suffixed by a copy of <paramref name="format" /> where any format specification is replaced by the string representation of the corresponding object argument. </returns>
            /// <param name="provider">An object that supplies culture-specific formatting information. </param>
            /// <param name="format">A composite format string (see Remarks). </param>
            /// <param name="arg0">The first object to format. </param>
            /// <param name="arg1">The second object to format. </param>
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="format" /> is null. </exception>
            /// <exception cref="System.FormatException">
            /// <paramref name="format" /> is invalid. -or-The index of a format item is less than 0 (zero), or greater than or equal to 2 (two). </exception>
            /// <exception cref="System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            public PooledStringBuilder AppendFormat(IFormatProvider provider, string format, object arg0, object arg1)
            {
                this.inner.AppendFormat(provider, format, arg0, arg1);
                return this;
            }

            /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of either of three arguments using a specified format provider.</summary>
            /// <returns>A reference to this instance after the append operation has completed. After the append operation, this instance contains any data that existed before the operation, suffixed by a copy of <paramref name="format" /> where any format specification is replaced by the string representation of the corresponding object argument. </returns>
            /// <param name="provider">An object that supplies culture-specific formatting information. </param>
            /// <param name="format">A composite format string (see Remarks). </param>
            /// <param name="arg0">The first object to format. </param>
            /// <param name="arg1">The second object to format. </param>
            /// <param name="arg2">The third object to format. </param>
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="format" /> is null. </exception>
            /// <exception cref="System.FormatException">
            /// <paramref name="format" /> is invalid. -or-The index of a format item is less than 0 (zero), or greater than or equal to 3 (three). </exception>
            /// <exception cref="System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            public PooledStringBuilder AppendFormat(IFormatProvider provider, string format, object arg0, object arg1, object arg2)
            {
                this.inner.AppendFormat(provider, format, arg0, arg1, arg2);
                return this;
            }

            /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a corresponding argument in a parameter array using a specified format provider.</summary>
            /// <returns>A reference to this instance after the append operation has completed. After the append operation, this instance contains any data that existed before the operation, suffixed by a copy of <paramref name="format" /> where any format specification is replaced by the string representation of the corresponding object argument. </returns>
            /// <param name="provider">An object that supplies culture-specific formatting information. </param>
            /// <param name="format">A composite format string (see Remarks). </param>
            /// <param name="args">An array of objects to format.</param>
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="format" /> is null. </exception>
            /// <exception cref="System.FormatException">
            /// <paramref name="format" /> is invalid. -or-The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args" /> array.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>2.</filterpriority>
            public PooledStringBuilder AppendFormat(IFormatProvider provider, string format, params object[] args)
            {
                this.inner.AppendFormat(provider, format, args);
                return this;
            }

            /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a single argument.</summary>
            /// <returns>A reference to this instance with <paramref name="format" /> appended. Each format item in <paramref name="format" /> is replaced by the string representation of <paramref name="arg0" />.</returns>
            /// <param name="format">A composite format string (see Remarks). </param>
            /// <param name="arg0">An object to format. </param>
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="format" /> is null. </exception>
            /// <exception cref="System.FormatException">
            /// <paramref name="format" /> is invalid. -or-The index of a format item is less than 0 (zero), or greater than or equal to 1.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>2.</filterpriority>
            public PooledStringBuilder AppendFormat(string format, object arg0)
            {
#pragma warning disable CA1305 // Specify IFormatProvider
                this.inner.AppendFormat(format, arg0);
#pragma warning restore CA1305 // Specify IFormatProvider
                return this;
            }

            /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of either of two arguments.</summary>
            /// <returns>A reference to this instance with <paramref name="format" /> appended. Each format item in <paramref name="format" /> is replaced by the string representation of the corresponding object argument.</returns>
            /// <param name="format">A composite format string (see Remarks). </param>
            /// <param name="arg0">The first object to format. </param>
            /// <param name="arg1">The second object to format. </param>
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="format" /> is null. </exception>
            /// <exception cref="System.FormatException">
            /// <paramref name="format" /> is invalid.-or-The index of a format item is less than 0 (zero), or greater than or equal to 2. </exception>
            /// <exception cref="System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>2.</filterpriority>
            public PooledStringBuilder AppendFormat(string format, object arg0, object arg1)
            {
#pragma warning disable CA1305 // Specify IFormatProvider
                this.inner.AppendFormat(format, arg0, arg1);
#pragma warning restore CA1305 // Specify IFormatProvider
                return this;
            }

            /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of either of three arguments.</summary>
            /// <returns>A reference to this instance with <paramref name="format" /> appended. Each format item in <paramref name="format" /> is replaced by the string representation of the corresponding object argument.</returns>
            /// <param name="format">A composite format string (see Remarks). </param>
            /// <param name="arg0">The first object to format. </param>
            /// <param name="arg1">The second object to format. </param>
            /// <param name="arg2">The third object to format. </param>
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="format" /> is null. </exception>
            /// <exception cref="System.FormatException">
            /// <paramref name="format" /> is invalid.-or-The index of a format item is less than 0 (zero), or greater than or equal to 3.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            /// <filterpriority>2.</filterpriority>
            public PooledStringBuilder AppendFormat(string format, object arg0, object arg1, object arg2)
            {
#pragma warning disable CA1305 // Specify IFormatProvider
                this.inner.AppendFormat(format, arg0, arg1, arg2);
#pragma warning restore CA1305 // Specify IFormatProvider
                return this;
            }

            /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a corresponding argument in a parameter array.</summary>
            /// <returns>A reference to this instance with <paramref name="format" /> appended. Each format item in <paramref name="format" /> is replaced by the string representation of the corresponding object argument.</returns>
            /// <param name="format">A composite format string (see Remarks). </param>
            /// <param name="args">An array of objects to format. </param>
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="format" /> or <paramref name="args" /> is null. </exception>
            /// <exception cref="System.FormatException">
            /// <paramref name="format" /> is invalid. -or-The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args" /> array.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            public PooledStringBuilder AppendFormat(string format, params object[] args)
            {
#pragma warning disable CA1305 // Specify IFormatProvider
                this.inner.AppendFormat(format, args);
#pragma warning restore CA1305 // Specify IFormatProvider
                return this;
            }

            /// <summary>Appends the default line terminator to the end of the current <see cref="System.Text.StringBuilder" /> object.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            public PooledStringBuilder AppendLine()
            {
                this.inner.AppendLine();
                return this;
            }

            /// <summary>Appends a copy of the specified string followed by the default line terminator to the end of the current <see cref="System.Text.StringBuilder" /> object.</summary>
            /// <returns>A reference to this instance after the append operation has completed.</returns>
            /// <param name="value">The string to append. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            public PooledStringBuilder AppendLine(string value)
            {
                this.inner.AppendLine(value);
                return this;
            }

            /// <summary>Removes all characters from the current <see cref="System.Text.StringBuilder" /> instance.</summary>
            /// <returns>An object whose <see cref="System.Text.StringBuilder.Length" /> is 0 (zero).</returns>
            public PooledStringBuilder Clear()
            {
                this.inner.Clear();
                return this;
            }

            /// <summary>Copies the characters from a specified segment of this instance to a specified segment of a destination <see cref="char" /> array.</summary>
            /// <param name="sourceIndex">The starting position in this instance where characters will be copied from. The index is zero-based.</param>
            /// <param name="destination">The array where characters will be copied.</param>
            /// <param name="destinationIndex">The starting position in <paramref name="destination" /> where characters will be copied. The index is zero-based.</param>
            /// <param name="count">The number of characters to be copied.</param>
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="destination" /> is null.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="sourceIndex" />, <paramref name="destinationIndex" />, or <paramref name="count" />, is less than zero.-or-<paramref name="sourceIndex" /> is greater than the length of this instance.</exception>
            /// <exception cref="System.ArgumentException">
            /// <paramref name="sourceIndex" /> + <paramref name="count" /> is greater than the length of this instance.-or-<paramref name="destinationIndex" /> + <paramref name="count" /> is greater than the length of <paramref name="destination" />.</exception>
            public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
            {
                this.inner.CopyTo(sourceIndex, destination, destinationIndex, count);
            }

            /// <summary>Ensures that the capacity of this instance of <see cref="System.Text.StringBuilder" /> is at least the specified value.</summary>
            /// <returns>The new capacity of this instance.</returns>
            /// <param name="capacity">The minimum capacity to ensure. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="capacity" /> is less than zero.-or- Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            public int EnsureCapacity(int capacity) => this.inner.EnsureCapacity(capacity);

            /// <summary>Inserts the string representation of a Boolean value into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The value to insert. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the length of this instance.</exception>
            /// <exception cref="System.OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />.</exception>
            public PooledStringBuilder Insert(int index, bool value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Inserts the string representation of a specified 8-bit unsigned integer into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The value to insert. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the length of this instance. </exception>
            /// <exception cref="System.OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />.</exception>
            public PooledStringBuilder Insert(int index, byte value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Inserts the string representation of a specified Unicode character into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The value to insert. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the length of this instance.-or- Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            public PooledStringBuilder Insert(int index, char value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Inserts the string representation of a specified array of Unicode characters into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The character array to insert. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the length of this instance.-or- Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            public PooledStringBuilder Insert(int index, char[] value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Inserts the string representation of a specified subarray of Unicode characters into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">A character array. </param>
            /// <param name="startIndex">The starting index within <paramref name="value" />. </param>
            /// <param name="charCount">The number of characters to insert. </param>
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="value" /> is null, and <paramref name="startIndex" /> and <paramref name="charCount" /> are not zero. </exception>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" />, <paramref name="startIndex" />, or <paramref name="charCount" /> is less than zero.-or- <paramref name="index" /> is greater than the length of this instance.-or- <paramref name="startIndex" /> plus <paramref name="charCount" /> is not a position within <paramref name="value" />.-or- Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            public PooledStringBuilder Insert(int index, char[] value, int startIndex, int charCount)
            {
                this.inner.Insert(index, value, startIndex, charCount);
                return this;
            }

            /// <summary>Inserts the string representation of a decimal number into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The value to insert. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the length of this instance. </exception>
            /// <exception cref="System.OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />.</exception>
            public PooledStringBuilder Insert(int index, decimal value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Inserts the string representation of a double-precision floating-point number into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The value to insert. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the length of this instance. </exception>
            /// <exception cref="System.OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />.</exception>
            public PooledStringBuilder Insert(int index, double value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Inserts the string representation of a specified 16-bit signed integer into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The value to insert. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the length of this instance. </exception>
            /// <exception cref="System.OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />.</exception>
            public PooledStringBuilder Insert(int index, short value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Inserts the string representation of a specified 32-bit signed integer into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The value to insert. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the length of this instance. </exception>
            /// <exception cref="System.OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />.</exception>
            public PooledStringBuilder Insert(int index, int value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Inserts the string representation of a 64-bit signed integer into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The value to insert. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the length of this instance. </exception>
            /// <exception cref="System.OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />.</exception>
            public PooledStringBuilder Insert(int index, long value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Inserts the string representation of an object into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The object to insert, or null. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the length of this instance. </exception>
            /// <exception cref="System.OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />.</exception>
            public PooledStringBuilder Insert(int index, object value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Inserts the string representation of a specified 8-bit signed integer into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The value to insert. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the length of this instance. </exception>
            /// <exception cref="System.OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />.</exception>
            public PooledStringBuilder Insert(int index, sbyte value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Inserts the string representation of a single-precision floating point number into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The value to insert. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the length of this instance. </exception>
            /// <exception cref="System.OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />.</exception>
            public PooledStringBuilder Insert(int index, float value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Inserts a string into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The string to insert. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the current length of this instance. -or-The current length of this <see cref="System.Text.StringBuilder" /> object plus the length of <paramref name="value" /> exceeds <see cref="System.Text.StringBuilder.MaxCapacity" />.</exception>
            public PooledStringBuilder Insert(int index, string value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Inserts one or more copies of a specified string into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after insertion has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The string to insert. </param>
            /// <param name="count">The number of times to insert <paramref name="value" />. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the current length of this instance.-or- <paramref name="count" /> is less than zero. </exception>
            /// <exception cref="System.OutOfMemoryException">The current length of this <see cref="System.Text.StringBuilder" /> object plus the length of <paramref name="value" /> times <paramref name="count" /> exceeds <see cref="System.Text.StringBuilder.MaxCapacity" />.</exception>
            public PooledStringBuilder Insert(int index, string value, int count)
            {
                this.inner.Insert(index, value, count);
                return this;
            }

            /// <summary>Inserts the string representation of a 16-bit unsigned integer into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The value to insert. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the length of this instance. </exception>
            /// <exception cref="System.OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />.</exception>
            public PooledStringBuilder Insert(int index, ushort value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Inserts the string representation of a 32-bit unsigned integer into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The value to insert. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the length of this instance. </exception>
            /// <exception cref="System.OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />.</exception>
            public PooledStringBuilder Insert(int index, uint value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Inserts the string representation of a 64-bit unsigned integer into this instance at the specified character position.</summary>
            /// <returns>A reference to this instance after the insert operation has completed.</returns>
            /// <param name="index">The position in this instance where insertion begins. </param>
            /// <param name="value">The value to insert. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero or greater than the length of this instance. </exception>
            /// <exception cref="System.OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />.</exception>
            public PooledStringBuilder Insert(int index, ulong value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            /// <summary>Removes the specified range of characters from this instance.</summary>
            /// <returns>A reference to this instance after the excise operation has completed.</returns>
            /// <param name="startIndex">The zero-based position in this instance where removal begins. </param>
            /// <param name="length">The number of characters to remove. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">If <paramref name="startIndex" /> or <paramref name="length" /> is less than zero, or <paramref name="startIndex" /> + <paramref name="length" /> is greater than the length of this instance. </exception>
            public PooledStringBuilder Remove(int startIndex, int length)
            {
                this.inner.Remove(startIndex, length);
                return this;
            }

            /// <summary>Replaces all occurrences of a specified character in this instance with another specified character.</summary>
            /// <returns>A reference to this instance with <paramref name="oldChar" /> replaced by <paramref name="newChar" />.</returns>
            /// <param name="oldChar">The character to replace. </param>
            /// <param name="newChar">The character that replaces <paramref name="oldChar" />. </param>
            /// <filterpriority>1.</filterpriority>
            public PooledStringBuilder Replace(char oldChar, char newChar)
            {
                this.inner.Replace(oldChar, newChar);
                return this;
            }

            /// <summary>Replaces, within a substring of this instance, all occurrences of a specified character with another specified character.</summary>
            /// <returns>A reference to this instance with <paramref name="oldChar" /> replaced by <paramref name="newChar" /> in the range from <paramref name="startIndex" /> to <paramref name="startIndex" /> + <paramref name="count" /> -1.</returns>
            /// <param name="oldChar">The character to replace. </param>
            /// <param name="newChar">The character that replaces <paramref name="oldChar" />. </param>
            /// <param name="startIndex">The position in this instance where the substring begins. </param>
            /// <param name="count">The length of the substring. </param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="startIndex" /> + <paramref name="count" /> is greater than the length of the value of this instance.-or- <paramref name="startIndex" /> or <paramref name="count" /> is less than zero. </exception>
            public PooledStringBuilder Replace(char oldChar, char newChar, int startIndex, int count)
            {
                this.inner.Replace(oldChar, newChar, startIndex, count);
                return this;
            }

            /// <summary>Replaces all occurrences of a specified string in this instance with another specified string.</summary>
            /// <returns>A reference to this instance with all instances of <paramref name="oldValue" /> replaced by <paramref name="newValue" />.</returns>
            /// <param name="oldValue">The string to replace. </param>
            /// <param name="newValue">The string that replaces <paramref name="oldValue" />, or null. </param>
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="oldValue" /> is null. </exception>
            /// <exception cref="System.ArgumentException">The length of <paramref name="oldValue" /> is zero. </exception>
            /// <exception cref="System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            public PooledStringBuilder Replace(string oldValue, string newValue)
            {
                this.inner.Replace(oldValue, newValue);
                return this;
            }

            /// <summary>Replaces, within a substring of this instance, all occurrences of a specified string with another specified string.</summary>
            /// <returns>A reference to this instance with all instances of <paramref name="oldValue" /> replaced by <paramref name="newValue" /> in the range from <paramref name="startIndex" /> to <paramref name="startIndex" /> + <paramref name="count" /> - 1.</returns>
            /// <param name="oldValue">The string to replace. </param>
            /// <param name="newValue">The string that replaces <paramref name="oldValue" />, or null. </param>
            /// <param name="startIndex">The position in this instance where the substring begins. </param>
            /// <param name="count">The length of the substring. </param>
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="oldValue" /> is null. </exception>
            /// <exception cref="System.ArgumentException">The length of <paramref name="oldValue" /> is zero. </exception>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="startIndex" /> or <paramref name="count" /> is less than zero.-or- <paramref name="startIndex" /> plus <paramref name="count" /> indicates a character position not within this instance.-or- Enlarging the value of this instance would exceed <see cref="System.Text.StringBuilder.MaxCapacity" />. </exception>
            public PooledStringBuilder Replace(string oldValue, string newValue, int startIndex, int count)
            {
                this.inner.Replace(oldValue, newValue, startIndex, count);
                return this;
            }

            /// <inheritdoc/>
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
            [Obsolete("Use Return", error: true)]
            public override string ToString() => this.inner.ToString();
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

            /// <summary>
            /// Get the string and return this instance to the pool.
            /// </summary>
            /// <returns>The text.</returns>
            public string Return()
            {
                var text = this.inner.ToString();
                this.inner.Clear();
                Cache.Enqueue(this);
                return text;
            }
        }
    }
}
