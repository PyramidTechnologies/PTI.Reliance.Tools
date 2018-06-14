---
uid: tut_flashupdate
---
# Firmware Flash Updates

This library is designed promote natural use of good design practices. One of the most
important patterns you will see in the samples, tests, and the core library itself is
extensive using of the ```using``` pattern. Following this design will prevent common
causes of memory leaks while making your code easier to maintain. Code that is easy
maintain gives you more time to focus on the features that bring value to your product.

## Code Sample
[!code-csharp[Main](Sample_02.cs)]

[!include[<Exceptions>](<exceptions.md>)]

> [!TIP]
> Flash udpating may be unreliable on Docker. See the [Docker](xref:tut_docker) section for more details.