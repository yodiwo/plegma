using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic
{
    public struct BuildValidationResult
    {
        public int BlockId;
        // public IO IO;
        public MemberInfo Member;
        public enum ResultType
        {
            Info,
            Warning,
            Error,
        }
        public ResultType Type;
        public string Message;

        public BuildValidationResult(int blockId, ResultType type, string message) { this.BlockId = blockId; this.Member = null; this.Type = type; this.Message = message; }
        // public BuildValidationResult(int blockId, IO io, ResultType type, string message) { this.BlockId = blockId; this.IO = io; this.Member = null; this.Type = type; this.Message = message; }
        public BuildValidationResult(int blockId, MemberInfo member, ResultType type, string message) { this.BlockId = blockId; this.Member = member; this.Type = type; this.Message = message; }
    }
}
