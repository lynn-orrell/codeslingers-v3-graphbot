using Autofac;
using Microsoft.Bot.Builder.Internals.Fibers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Codeslingers.Bots.v3.GraphBot.Dialogs
{
    [Serializable]
    public class DialogFactory : IDialogFactory
    {
        protected readonly IComponentContext _scope;

        public DialogFactory(IComponentContext scope)
        {
            SetField.NotNull(out this._scope, nameof(scope), scope);
        }

        public T Create<T>()
        {
            return this._scope.Resolve<T>();
        }

        public T Create<T>(IDictionary<string, object> parameters)
        {
            return this._scope.Resolve<T>(parameters.Select(kv => new NamedParameter(kv.Key, kv.Value)));
        }
    }
}