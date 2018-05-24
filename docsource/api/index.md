# Reliance Thermal Printer API
This library was designed so that the majority of your tasks can be completed with a single ````using```` statement

We recommend that your create your <xref:PTIRelianceLib.ReliancePrinter> on as as-needed basis. That means we do
not recommend keeping your printer as a field or property inside your class.

> [!div class="tabbedCodeSnippets"]
> ```cs
>
>  public class MyApplication {
>
>      // Don't do this!!
>      private IPyramidDevice _mPrinter;
>
>      // Or this!!
>      public ReliancePrinter Printer { get; private set;}
>
>      // But do this
>      public void DoPrinterThings(MyData data) {
>      
>			// Printer is automatically discovered and connected
>			using(var printer = new ReliancePrinter() {
>			
>				var stuff = data.GetStuff();
>				...
>				// Proceed to work with printer				
>
>			}
>
>      }
>
>  } 
>
> ```

You can also do some cool things with the data types we provide such as <xref:PTIRelianceLib.Status> and <xref:PTIRelianceLib.Revlev>. They
are automatically disposable so you are free to bind to their properties without having to worry about memory leaks.