# Checking Printer Status

Your Reliance printer can report errors in a number ways. This tutorial demonstrates
how to use the method built into this library. It is easier to use that the 
ESC/POS version and is not OS-depdendent like the C++ version.

## Code Sample
[!code-csharp[Main](Sample_04.cs)]

## Exceptions
For clarity, exception handling has been elided. It is advisable to wrap any ReliancePrinter 
method calls in a try/catch block for <xref:PTIRelianceLib.PTIException>.