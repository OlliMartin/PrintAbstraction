using PrintAbstraction.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintAbstractionConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      
      Console.WriteLine(PrintAbstractionCore.GetDefaultPrinter());

      Console.WriteLine("");

      Console.WriteLine(string.Join(Environment.NewLine, PrintAbstractionCore.GetAvailablePrinters()));

      var generatedList = new List<string>();
      for(int i=0;i<1000;++i)
      {
        generatedList.Add($"This is a template: {i}");
      }

      var contentDoubleLinebreak = "this is a sample content with a double linebreak exactly here:" + Environment.NewLine + Environment.NewLine + "and here." + Environment.NewLine + "that was the last one";
      var contentSingleLinebreak = "this is a sample content with a linebreak exactly here:" + Environment.NewLine + "and here." + Environment.NewLine + "that was the last one";

      var contentNoLb = "This is a really long text to check if the linebreaks are working as intended and the text breaks without destroying the format. SO i had a steak today and will visit max in landau and thiasodhaosidjaosjd fuck this shit....... What the shit is going on, I am expecting you to linebreak, so DO SO!!!!"
        + Environment.NewLine + "This is a really long text to check if the linebreaks are working as intended and the text breaks without destroying the format. SO i had a steak today and will visit max in landau and thiasodhaosidjaosjd fuck this shit....... What the shit is going on, I am expecting you to linebreak, so DO SO!!!!";

      //PrintAbstractionCore.PrintList(generatedList);
      PrintAbstractionCore.PrintString(contentSingleLinebreak, false);
      //PrintAbstractionCore.PrintString(contentDoubleLinebreak, false);
      //PrintAbstractionCore.PrintString(contentNoLb);

    }
  }
}
