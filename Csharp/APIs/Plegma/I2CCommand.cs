using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Plegma
{
    public class I2CCommand
    {
        public byte devaddress;
        public byte register;
        public bool IsWrite;
        public int ReadLength;
        public byte[] value;
    }
}
