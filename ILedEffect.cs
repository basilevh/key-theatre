// Started 04-11-2016, Basile Van Hoorick
// Describes an effect that uses LedController

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyDecorator
{
    public interface ILedEffect
    {
        void Execute(LedController ledCont);
    }
}
