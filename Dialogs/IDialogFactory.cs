using System.Collections.Generic;

namespace Codeslingers.Bots.v3.GraphBot.Dialogs
{
    public interface IDialogFactory
    {
        T Create<T>();
        T Create<T>(IDictionary<string, object> parameters);
    }
}
