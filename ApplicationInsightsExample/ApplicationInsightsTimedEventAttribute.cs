using PostSharp.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ApplicationInsights.Telemetry.Services;
using System.Reflection;
using PostSharp.Serialization;

namespace ApplicationInsightsExample
{
    [PSerializable]
    public class ApplicationInsightsTimedEventAttribute : OnMethodBoundaryAspect
    {
        string methodPath;
        string[] parameterNames;

        public bool IncludeArguments { get; private set; }

        public ApplicationInsightsTimedEventAttribute(bool includeArguments = true)
        {
            this.IncludeArguments = includeArguments;
            this.ApplyToStateMachine = true;
        }

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            this.methodPath = method.DeclaringType.FullName
                                .Replace('.', '/').Replace('+', '/')
                                + "/" + method.Name;
            this.parameterNames = method.GetParameters().Select(p => p.Name).ToArray();
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            IAnalyticsRequest request = ServerAnalytics.CurrentRequest;
            if (request != null)
            {
                ITimedAnalyticsEvent timedEvent;
                if (this.IncludeArguments)
                {
                    List<KeyValuePair<string, object>> arguments = new List<KeyValuePair<string, object>>();
                    for (int i = 0; i < this.parameterNames.Length; i++)
                    {
                        arguments.Add(new KeyValuePair<string,object>( this.parameterNames[i], args.Arguments[i]));
                    }
                    timedEvent = request.StartTimedEvent(this.methodPath, arguments);
                }
                else
                {
                    timedEvent = request.StartTimedEvent(this.methodPath);
                }

                args.MethodExecutionTag = timedEvent;
            }
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            ITimedAnalyticsEvent timedEvent = (ITimedAnalyticsEvent) args.MethodExecutionTag;
            if ( timedEvent != null )
            {
                timedEvent.End();
            }
        }

    }
}