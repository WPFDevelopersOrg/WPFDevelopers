using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.Media
{
    public sealed class GeometryEffectConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => typeof(string).IsAssignableFrom(sourceType);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => typeof(string).IsAssignableFrom(destinationType);

        // Token: 0x0600023C RID: 572 RVA: 0x0000DBC0 File Offset: 0x0000BDC0
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string text = value as string;
            GeometryEffect geometryEffect;
            if (text != null && GeometryEffectConverter.registeredEffects.TryGetValue(text, out geometryEffect))
                return geometryEffect.CloneCurrentValue();

            return null;
        }

        // Token: 0x0600023D RID: 573 RVA: 0x0000DBF0 File Offset: 0x0000BDF0
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (typeof(string).IsAssignableFrom(destinationType))
            {
                foreach (KeyValuePair<string, GeometryEffect> keyValuePair in GeometryEffectConverter.registeredEffects)
                {
                    if ((keyValuePair.Value == null) ? (value == null) : keyValuePair.Value.Equals(value as GeometryEffect))
                        return keyValuePair.Key;
                }
            }
            return null;
        }

        // Token: 0x040000B4 RID: 180
        private static Dictionary<string, GeometryEffect> registeredEffects = new Dictionary<string, GeometryEffect>
        {
            {
                "None",
                GeometryEffect.DefaultGeometryEffect
            },
            {
                "Sketch",
                new SketchGeometryEffect()
            }
        };
    }
}
