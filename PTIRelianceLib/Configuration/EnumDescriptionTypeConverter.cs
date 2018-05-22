#region Header
// EnumDescriptionTypeConverter.cs
// PTIRelianceLib
// Cory Todd
// 18-05-2018
// 7:28 AM
#endregion

namespace PTIRelianceLib.Configuration
{
    using System;
    using System.ComponentModel;

    /// <inheritdoc />
    /// <summary>
    /// Converts a Type into a concrete enum
    /// </summary>
    internal class EnumDescriptionTypeConverter : EnumConverter
    {
        /// <inheritdoc />
        public EnumDescriptionTypeConverter(Type type)
            : base(type)
        {
        }

        /// <inheritdoc />
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            return destinationType == typeof(string) ? (value as Enum).GetEnumName() : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}