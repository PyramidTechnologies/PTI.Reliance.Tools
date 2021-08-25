namespace RelianceStatusPolling
{
    using PTIRelianceLib;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using PTIRelianceLib.Protocol;

    /// <summary>
    /// The following is a demo app that demonstrates status monitoring
    /// for a Reliance Printer.
    ///
    /// Starting Polling will publish events to the console <see cref="SubscribeToPollingEvents"/>.
    /// </summary>
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            using var printer = new ReliancePrinter();

            if (!printer.IsDeviceReady)
            {
                Console.WriteLine("No Reliance Printer found.");
                
                return;
            }
            
            var demo = new StatusPollingDemo();
            demo.SubscribeToPollingEvents();
                
            do
            {
                Console.WriteLine("Press a key to choose one of the following options: ");
                Console.WriteLine("s - Start polling thread\n" +
                                  "c - Cancel polling thread\n" +
                                  "e - Exit program");

                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.KeyChar == 's')
                {
                    demo.StartPolling(printer);
                }
                else if (key.KeyChar == 'c')
                {
                    demo.StopPolling();
                }
                else if (key.KeyChar == 'e')
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Unknown Command.");
                }
                
            } while (true);
        }
    }

    /// <summary>
    /// Demo application for Reliance Printer Status Polling
    /// </summary>
    public class StatusPollingDemo : IDisposable
    {
        private const int PollingPeriodMs = 1000;

        private static CancellationTokenSource _cts;
        
        private static event EventHandler PlatenOpenEvent;
        
        private readonly SemaphoreSlim _pollingLoopSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Raised when the Printer is opened
        /// </summary>
        private void RaisePlatenOpen()
        {
            PlatenOpenEvent?.Invoke(this , EventArgs.Empty);
        }

        /// <summary>
        /// Start polling the printer status
        /// </summary>
        /// <param name="printer">ReliancePrinter to poll</param>
        public void StartPolling(ReliancePrinter printer)
        {
            // Use a semaphore to guard against starting multiple
            // polling tasks
            if (!_pollingLoopSemaphore.Wait(0))
            {
                Console.WriteLine("Polling is already running.");
                return;
            }

            Console.WriteLine("Starting Polling.");

            // Cancellation Tokens can be used to stop the polling task
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            
            Task.Factory.StartNew(() =>
            {
                Status previousStatus = null;

                try
                {
                    Console.WriteLine("Polling Started.");
                    
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();

                        var currentStatus = printer.GetStatus();

                        if (currentStatus is null)
                        {
                            Console.WriteLine("Failed to poll printer.");
                            
                            Thread.Sleep(PollingPeriodMs);

                            continue;
                        }

                        // The printer is open ...
                        if (currentStatus.PrinterErrors.HasFlag(ErrorStatuses.PlatenOpen))
                        {
                            // ... and it wasn't open when previously polled - raise the event
                            if (previousStatus == null || !previousStatus.PrinterErrors.HasFlag(ErrorStatuses.PlatenOpen))
                            {
                                RaisePlatenOpen();
                            }
                        }

                        previousStatus = currentStatus;
                    
                        Thread.Sleep(PollingPeriodMs);
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Polling Stopped.");
                }
                
            }, token);
        }

        /// <summary>
        /// Stop polling
        /// </summary>
        public void StopPolling()
        {
            Console.WriteLine("Stopping Polling.");

            _cts.Cancel();
            _cts.Dispose();

            _pollingLoopSemaphore.Release();
        }

        /// <summary>
        /// Set up event handlers
        /// </summary>
        public void SubscribeToPollingEvents()
        {
            PlatenOpenEvent += (sender, eventArgs) =>
            {
                Console.WriteLine("Printer has been opened.");
            };
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _pollingLoopSemaphore?.Dispose();
            _cts?.Dispose();
            PlatenOpenEvent = null;
        }
    }
}