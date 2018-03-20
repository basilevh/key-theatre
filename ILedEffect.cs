// Started 04-11-2016, Basile Van Hoorick

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyDecorator
{
    /// <summary>
    /// Describes an effect that uses LedController.
    /// </summary>
    public interface ILedEffect
    {
        void Execute(LedController ledCont);
    }
}
