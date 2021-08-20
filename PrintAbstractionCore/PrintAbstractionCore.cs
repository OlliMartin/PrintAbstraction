using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintAbstraction.Core
{
  public class PrintAbstractionCore
  {
    private static Font printFont = new Font("Arial", 12);
    private static StreamReader streamToPrint;

    public static List<string> GetAvailablePrinters()
    {
      var result = new List<string>();

      for(int i=0; i<PrinterSettings.InstalledPrinters.Count; ++i)
      {
        result.Add(PrinterSettings.InstalledPrinters[i]);
      }

      return result;
    }

    public static string GetDefaultPrinter()
    {
      var printerSettings = new PrinterSettings();
      return printerSettings.PrinterName;
    }

    public static void OverwriteFont(Font font)
    {
      _ = font ?? throw new ArgumentNullException(nameof(font));
      printFont = font;
    }

    public static void PrintString(string content, bool removeEmptyLines = false)
    {
      _ = content ?? throw new ArgumentNullException(nameof(content));
      PrintString(content, GetDefaultPrinter(), removeEmptyLines);
    }

    public static void PrintString(string content, string printerName, bool removeEmptyLines = false)
    {
      _ = content ?? throw new ArgumentNullException(nameof(content));

      var listOfLines = content.Split(
        Environment.NewLine.ToCharArray(), 
        removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None
      );

      PrintList(listOfLines, printerName);
    }

    public static void PrintList(IEnumerable<string> listOfLines)
    {
      _ = listOfLines ?? throw new ArgumentNullException(nameof(listOfLines));
      PrintList(listOfLines, GetDefaultPrinter());
    }

    public static void PrintList(IEnumerable<string> listOfLines, string printerName)
    {
      _ = listOfLines ?? throw new ArgumentNullException(nameof(listOfLines));
      _ = printerName ?? throw new ArgumentNullException(nameof(printerName));

      if (!GetAvailablePrinters().Contains(printerName, StringComparer.InvariantCultureIgnoreCase))
      {
        throw new InvalidOperationException($"There is no printer called {printerName} installed.");
      }

      PrintListInternal(listOfLines, 
        GetAvailablePrinters().Single(p => p.Equals(printerName, StringComparison.InvariantCultureIgnoreCase))
      );
    }

    private static void PrintListInternal(IEnumerable<string> listOfLines, string printerName)
    {
      streamToPrint = new StreamReader(
        new MemoryStream(
          Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, listOfLines))
        )
      );

      try
      {
        PrintDocument pd = new PrintDocument();
        pd.PrintPage += new PrintPageEventHandler(PrintCallbackInternal);
        pd.PrinterSettings = new PrinterSettings { PrinterName = printerName };
        pd.Print();
      }
      finally
      {
        streamToPrint.Close();
        streamToPrint.Dispose();
      }
    }

    private static void PrintCallbackInternal(object sender, PrintPageEventArgs ev)
    {
      float linesPerPage = 0;
      float yPos = 0;
      int count = 0;
      float leftMargin = ev.MarginBounds.Left;
      float topMargin = ev.MarginBounds.Top;
      String line = null;

      linesPerPage = ev.MarginBounds.Height / printFont.GetHeight(ev.Graphics);

      while (count < linesPerPage && ((line = streamToPrint.ReadLine()) != null))
      {
        var lineMeasure = ev.Graphics.MeasureString(line, printFont);

        var linesToRender = (int)Math.Ceiling(lineMeasure.Width / ev.MarginBounds.Right);

        if(linesToRender == 0)
        {
          count++;
          continue;
        }

        var lineLength = line.Length / linesToRender;

        for(int i=0; i<linesToRender; ++i)
        {
          var lineToRender = line.Substring(
            i * lineLength, 
            Math.Min(lineLength, line.Length - i * lineLength)
          );

          yPos = topMargin + ((count+i) * printFont.GetHeight(ev.Graphics));
          ev.Graphics.DrawString(lineToRender, printFont, Brushes.Black,
             leftMargin, yPos, new StringFormat());
        }

        count += linesToRender;
      }

      // If more lines exist, print another page.
      if (line != null)
        ev.HasMorePages = true;
      else
        ev.HasMorePages = false;
    }
  }
}
