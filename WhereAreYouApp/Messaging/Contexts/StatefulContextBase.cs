using Line.Messaging.Webhooks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WhereAreYouApp.Messaging.Contexts
{
    public abstract class StatefulContextBase<TEvent> : ContextBase<TEvent>
        where TEvent : WebhookEvent
    {
        private string _nextCallMethodName;
        protected override async Task ExecuteImplAsync(ContextState contextState, TEvent ev)
        {
            if (!RestoreState(contextState.SessionData.StatefulContext, out var snapshot))
            {
                await InitializeAsync(contextState, ev);
                await StoreStateAsync(contextState, ev);
                return;
            }

            JsonConvert.PopulateObject(snapshot.StatusJson, this);
            var targetMethod = GetType().GetMethod(snapshot.NextMethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (targetMethod == null)
            {
                throw new InvalidOperationException($"{snapshot.NextMethodName} not found");
            }

            var task = (Task)targetMethod.Invoke(this, new object[]
            {
                contextState, ev
            });
            await task;
            await StoreStateAsync(contextState, ev);
        }

        private bool RestoreState(string statefulContextJson, out StatefulContextSnapshot snapshot)
        {
            if (string.IsNullOrEmpty(statefulContextJson))
            {
                snapshot = null;
                return false;
            }

            snapshot = JsonConvert.DeserializeObject<StatefulContextSnapshot>(statefulContextJson);
            return snapshot.TypeName == GetType().FullName && !string.IsNullOrEmpty(snapshot.NextMethodName);
        }

        private Task StoreStateAsync(ContextState contextState, TEvent ev)
        {
            if (string.IsNullOrEmpty(_nextCallMethodName))
            {
                contextState.SessionData.StatefulContext = null;
                return Task.CompletedTask;
            }

            contextState.SessionData.StatefulContext = JsonConvert.SerializeObject(new StatefulContextSnapshot
            {
                TypeName = this.GetType().FullName,
                NextMethodName = _nextCallMethodName,
                StatusJson = JsonConvert.SerializeObject(this),
            });

            return Task.CompletedTask;
        }

        protected abstract Task InitializeAsync(ContextState contextState, TEvent ev);

        protected void SetNextCallMethod(string methodName)
        {
            _nextCallMethodName = methodName;
        }
    }
}
