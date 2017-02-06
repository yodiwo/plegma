using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    /// <summary>Dummy re-implementation of SecureString for compatibility/portability purposes</summary>    
    public class SecureString : IDisposable     //Maybe this could be like in netfx core but uap apps doesn't seem to need this kind of protection?
    {
        private bool _isReadOnly = false;
        internal string _value = string.Empty;

        public int Length { get { return _value?.Length ?? 0; } }
        public void AppendChar(char c) { _value += c; }
        public void Clear() { _value = string.Empty; }
        public SecureString Copy() { return MemberwiseClone() as SecureString; }
        public void Dispose() { _value = string.Empty; }
        public void InsertAt(int index, char c) { throw new NotImplementedException(); }
        public bool IsReadOnly() { return _isReadOnly; }
        public void MakeReadOnly() { _isReadOnly = true; }
        public void RemoveAt(int index) { throw new NotImplementedException(); }
        public void SetAt(int index, char c) { throw new NotImplementedException(); }
    }
}
