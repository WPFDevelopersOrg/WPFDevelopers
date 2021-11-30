using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Standard
{
    [StructLayout(LayoutKind.Explicit)]
    public struct HRESULT
    {
        public HRESULT(uint i)
        {
            this._value = i;
        }

        public static HRESULT Make(bool severe, Facility facility, int code)
        {
            return new HRESULT((uint)((severe ? (Facility)(-2147483648) : Facility.Null) | (Facility)((int)facility << 16) | (Facility)code));
        }

        public Facility Facility
        {
            get
            {
                return HRESULT.GetFacility((int)this._value);
            }
        }

        public static Facility GetFacility(int errorCode)
        {
            return (Facility)(errorCode >> 16 & 8191);
        }

        public int Code
        {
            get
            {
                return HRESULT.GetCode((int)this._value);
            }
        }

        public static int GetCode(int error)
        {
            return error & 65535;
        }

        public override string ToString()
        {
            foreach (FieldInfo fieldInfo in typeof(HRESULT).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (fieldInfo.FieldType == typeof(HRESULT))
                {
                    HRESULT hrLeft = (HRESULT)fieldInfo.GetValue(null);
                    if (hrLeft == this)
                    {
                        return fieldInfo.Name;
                    }
                }
            }
            if (this.Facility == Facility.Win32)
            {
                foreach (FieldInfo fieldInfo2 in typeof(Win32Error).GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    if (fieldInfo2.FieldType == typeof(Win32Error))
                    {
                        Win32Error error = (Win32Error)fieldInfo2.GetValue(null);
                        if ((HRESULT)error == this)
                        {
                            return "HRESULT_FROM_WIN32(" + fieldInfo2.Name + ")";
                        }
                    }
                }
            }
            return string.Format(CultureInfo.InvariantCulture, "0x{0:X8}", new object[]
            {
                this._value
            });
        }

        public override bool Equals(object obj)
        {
            bool result;
            try
            {
                result = (((HRESULT)obj)._value == this._value);
            }
            catch (InvalidCastException)
            {
                result = false;
            }
            return result;
        }

        public override int GetHashCode()
        {
            return this._value.GetHashCode();
        }

        public static bool operator ==(HRESULT hrLeft, HRESULT hrRight)
        {
            return hrLeft._value == hrRight._value;
        }

        public static bool operator !=(HRESULT hrLeft, HRESULT hrRight)
        {
            return !(hrLeft == hrRight);
        }

        public bool Succeeded
        {
            get
            {
                return this._value >= 0U;
            }
        }

        public bool Failed
        {
            get
            {
                return this._value < 0U;
            }
        }

        public void ThrowIfFailed()
        {
            this.ThrowIfFailed(null);
        }

        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Only recreating Exceptions that were already raised.")]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void ThrowIfFailed(string message)
        {
            if (this.Failed)
            {
                if (string.IsNullOrEmpty(message))
                {
                    message = this.ToString();
                }
                Exception ex = Marshal.GetExceptionForHR((int)this._value, new IntPtr(-1));
                if (ex.GetType() == typeof(COMException))
                {
                    Facility facility = this.Facility;
                    if (facility == Facility.Win32)
                    {
                        ex = new Win32Exception(this.Code, message);
                    }
                    else
                    {
                        ex = new COMException(message, (int)this._value);
                    }
                }
                else
                {
                    ConstructorInfo constructor = ex.GetType().GetConstructor(new Type[]
                    {
                        typeof(string)
                    });
                    if (null != constructor)
                    {
                        ex = (constructor.Invoke(new object[]
                        {
                            message
                        }) as Exception);
                    }
                }
                throw ex;
            }
        }

        public static void ThrowLastError()
        {
            ((HRESULT)Win32Error.GetLastError()).ThrowIfFailed();
        }

        [FieldOffset(0)]
        private readonly uint _value;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT S_OK = new HRESULT(0U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT S_FALSE = new HRESULT(1U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_PENDING = new HRESULT(2147483658U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_NOTIMPL = new HRESULT(2147500033U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_NOINTERFACE = new HRESULT(2147500034U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_POINTER = new HRESULT(2147500035U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_ABORT = new HRESULT(2147500036U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_FAIL = new HRESULT(2147500037U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_UNEXPECTED = new HRESULT(2147549183U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT STG_E_INVALIDFUNCTION = new HRESULT(2147680257U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT REGDB_E_CLASSNOTREG = new HRESULT(2147746132U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT DESTS_E_NO_MATCHING_ASSOC_HANDLER = new HRESULT(2147749635U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT DESTS_E_NORECDOCS = new HRESULT(2147749636U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT DESTS_E_NOTALLCLEARED = new HRESULT(2147749637U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_ACCESSDENIED = new HRESULT(2147942405U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_OUTOFMEMORY = new HRESULT(2147942414U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_INVALIDARG = new HRESULT(2147942487U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT INTSAFE_E_ARITHMETIC_OVERFLOW = new HRESULT(2147942934U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT COR_E_OBJECTDISPOSED = new HRESULT(2148734498U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT WC_E_GREATERTHAN = new HRESULT(3222072867U);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT WC_E_SYNTAX = new HRESULT(3222072877U);
    }
}
