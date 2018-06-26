# Telemetry
The Reliance Thermal printer tracks the metrics that matter. We have counters for just about any even you can imagine and they are all accessible through this API. With this information, you can track paper consumption, ticket pull habits, error patterns, and many other metrics.

There are two groups for telemetry:

1. Lifetime : This is a cumulative record of all events since the printer left our factory.
2. Powerup : This is a record of all events since the last power cycle.

See <xref:PTIRelianceLib.Telemetry.LifetimeTelemetry> and <xref:PTIRelianceLib.Telemetry.PowerupTelemetry> for more details.

> [!Warning]
> Require Firmware 1.28+. Calling this API on older firmware will return null.

## Ticket Pull
The best way to detect a ticket pull is request the telemetry information and inspect the <xref:PTIRelianceLib.Telemetry.LastTicketState> property. This records the most recent action taken on a printed ticket along with the ticket's length in millimeters. We recommend that you do not poll the printer more than 4 times a second in order to prevent unecessary blocking read requests. After a ticket is printed, poll the printer about once a second and watch for the TicketCount property to increment. Once it increments, you can read the LastTicketState property and examine how the ticket was handled.


## Ticket Lengths
Tickets lengths are tracked by binning ticket lengths into 9 groups. The exact lengths groups are enumerated in <xref:PTIRelianceLib.Telemetry.TicketLengthGroups> in millimeters. The telemetry object tracks the count of each ticket by these groups. All tickets with the exception of startup and push-button diagnostic tickets are counted in this metric.

## Ticket Pull Time
Tickets pull time is the time in second it took for a customer to pull the ticket from the printer. If a ticket is never pulled and is instead ejected or retracted, no measurement will be taken. The exact time groups are enumerated in <xref:PTIRelianceLib.Telemetry.TicketPullTimeGroups> in second.

## Code Sample
[!code-csharp[Main](Sample_06.cs)]

[!include[<Exceptions>](<exceptions.md>)]