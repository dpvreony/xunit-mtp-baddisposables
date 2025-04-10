# Xunit + MTP Bad Disposables
Minimal repro for a bug in XUnit + MTP and handling of bad disposables

This issue exists in XUnit v3 with the following scenario

* You are using MTP
* You are using a Theory with Theory Data that passes an IDisposable
* The dispose method of the object passed in throws an exception

The test runner will crash with a non descript error, looking in the logs will provide the error

```
Unhandled exception. System.InvalidOperationException: Sequence contains no elements
   at System.Linq.ThrowHelper.ThrowNoElementsException()
   at System.Linq.Enumerable.MaxInteger[TSource,TResult](IEnumerable`1 source, Func`2 selector)
   at System.Linq.Enumerable.Max[TSource](IEnumerable`1 source, Func`2 selector)
   at Xunit.Runner.Common.DefaultRunnerReporterMessageHandler.WriteDefaultSummary(IRunnerLogger logger, TestExecutionSummaries summaries) in /_/src/xunit.v3.runner.common/Reporters/Builtin/DefaultRunnerReporterMessageHandler.cs:line 709
   at Xunit.Runner.Common.DefaultRunnerReporterMessageHandler.HandleTestExecutionSummaries(MessageHandlerArgs`1 args) in /_/src/xunit.v3.runner.common/Reporters/Builtin/DefaultRunnerReporterMessageHandler.cs:line 531
   at Xunit.Runner.Common.RunnerEventSink.OnMessage(IMessageSinkMessage message) in /_/src/xunit.v3.runner.common/Sinks/EventSinks/RunnerEventSink.cs:line 43
   at Xunit.Runner.Common.AggregateMessageSink.OnMessage(IMessageSinkMessage message) in /_/src/xunit.v3.runner.common/Sinks/AggregateMessageSink.cs:line 61
   at Xunit.Runner.InProc.SystemConsole.TestingPlatform.TestPlatformExecutionMessageSink.OnMessage(IMessageSinkMessage message) in /_/src/xunit.v3.runner.inproc.console/TestingPlatform/TestPlatformExecutionMessageSink.cs:line 60
   at Xunit.Runner.InProc.SystemConsole.ProjectAssemblyRunner.Run(XunitProjectAssembly assembly, IMessageSink messageSink, IMessageSink diagnosticMessageSink, IRunnerLogger runnerLogger, ITestPipelineStartup pipelineStartup, HashSet`1 testCaseIDsToRun) in /_/src/xunit.v3.runner.inproc.console/Utility/ProjectAssemblyRunner.cs:line 327
   at Xunit.Runner.InProc.SystemConsole.TestingPlatform.TestPlatformTestFramework.<>c__DisplayClass17_0.<<OnExecute>b__0>d.MoveNext() in /_/src/xunit.v3.runner.inproc.console/TestingPlatform/TestPlatformTestFramework.cs:line 159
--- End of stack trace from previous location ---
   at Xunit.Runner.InProc.SystemConsole.TestingPlatform.TestPlatformTestFramework.OnRequest(SessionUid sessionUid, Action operationComplete, Func`3 callback, CancellationToken cancellationToken) in /_/src/xunit.v3.runner.inproc.console/TestingPlatform/TestPlatformTestFramework.cs:line 187
   at Xunit.Runner.InProc.SystemConsole.TestingPlatform.TestPlatformTestFramework.OnRequest(SessionUid sessionUid, Action operationComplete, Func`3 callback, CancellationToken cancellationToken) in /_/src/xunit.v3.runner.inproc.console/TestingPlatform/TestPlatformTestFramework.cs:line 193
   at Xunit.Runner.InProc.SystemConsole.TestingPlatform.TestPlatformTestFramework.Microsoft.Testing.Platform.Extensions.TestFramework.ITestFramework.ExecuteRequestAsync(ExecuteRequestContext context) in /_/src/xunit.v3.runner.inproc.console/TestingPlatform/TestPlatformTestFramework.cs:line 115
   at Microsoft.Testing.Platform.Requests.TestHostTestFrameworkInvoker.ExecuteRequestAsync(ITestFramework testFramework, TestExecutionRequest request, IMessageBus messageBus, CancellationToken cancellationToken) in /_/src/Platform/Microsoft.Testing.Platform/Requests/TestHostTestFrameworkInvoker.cs:line 72
   at Microsoft.Testing.Platform.Requests.TestHostTestFrameworkInvoker.ExecuteAsync(ITestFramework testFramework, ClientInfo client, CancellationToken cancellationToken) in /_/src/Platform/Microsoft.Testing.Platform/Requests/TestHostTestFrameworkInvoker.cs:line 61
   at Microsoft.Testing.Platform.Hosts.CommonTestHost.ExecuteRequestAsync(ProxyOutputDevice outputDevice, ITestSessionContext testSessionInfo, ServiceProvider serviceProvider, BaseMessageBus baseMessageBus, ITestFramework testFramework, ClientInfo client) in /_/src/Platform/Microsoft.Testing.Platform/Hosts/CommonTestHost.cs:line 136
   at Microsoft.Testing.Platform.Hosts.ConsoleTestHost.InternalRunAsync() in /_/src/Platform/Microsoft.Testing.Platform/Hosts/ConsoleTestHost.cs:line 83
   at Microsoft.Testing.Platform.Hosts.ConsoleTestHost.InternalRunAsync() in /_/src/Platform/Microsoft.Testing.Platform/Hosts/ConsoleTestHost.cs:line 115
   at Microsoft.Testing.Platform.Hosts.CommonTestHost.RunTestAppAsync(CancellationToken testApplicationCancellationToken) in /_/src/Platform/Microsoft.Testing.Platform/Hosts/CommonTestHost.cs:line 110
   at Microsoft.Testing.Platform.Hosts.CommonTestHost.RunAsync() in /_/src/Platform/Microsoft.Testing.Platform/Hosts/CommonTestHost.cs:line 34
   at Microsoft.Testing.Platform.Hosts.CommonTestHost.RunAsync() in /_/src/Platform/Microsoft.Testing.Platform/Hosts/CommonTestHost.cs:line 71
   at Microsoft.Testing.Platform.Builder.TestApplication.RunAsync() in /_/src/Platform/Microsoft.Testing.Platform/Builder/TestApplication.cs:line 237
   at Xunit.Runner.InProc.SystemConsole.TestingPlatform.TestPlatformTestFramework.RunAsync(String[] args, Action`2 extensionRegistration) in /_/src/xunit.v3.runner.inproc.console/TestingPlatform/TestPlatformTestFramework.cs:line 273
```

If you debug your XUnit V3 app and enable catching exceptions on external code you will see that it actually errors as:

```
   at ******.DisposeAsync() in somefile.cs:line 217
   at Xunit.Sdk.DisposalTracker.<DisposeAsync>d__10.MoveNext() in /_/src/xunit.v3.common/Utility/DisposalTracker.cs:line 137
   at Xunit.Sdk.DisposalTracker.<DisposeAsync>d__10.MoveNext() in /_/src/xunit.v3.common/Utility/DisposalTracker.cs:line 148
   at Xunit.v3.InProcessFrontController.<FindAndRun>d__12.MoveNext() in /_/src/xunit.v3.core/Framework/InProcessFrontController.cs:line 191
   at Xunit.Runner.InProc.SystemConsole.ProjectAssemblyRunner.<Run>d__13.MoveNext() in /_/src/xunit.v3.runner.inproc.console/Utility/ProjectAssemblyRunner.cs:line 279
   at Xunit.Runner.InProc.SystemConsole.ProjectAssemblyRunner.<Run>d__13.MoveNext() in /_/src/xunit.v3.runner.inproc.console/Utility/ProjectAssemblyRunner.cs:line 298
```

The result of the first exception is that no test summaries are populated, so the first exception is then raised and crashes the app.
